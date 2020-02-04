using CppInterop.LandmarkDetector;
using FaceAnalyser_Interop;
using FaceDetectorInterop;
using GazeAnalyser_Interop;
using NeuroExplorer.Connectors.OpenFace;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json.Linq;
// OpenFace
using OpenCVWrappers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UtilitiesOF;

namespace NeuroExplorer.Connectors
{
    class OpenFaceConnector : IWebSocketPropagator
    {

        private WebSocketConnector webSocketConnector;
        private string status;
        private bool recorderDisposed = true;

        // Timing for measuring FPS
        static DateTime startTime = new DateTime();
        static Stopwatch sw = new Stopwatch();
        public static DateTime CurrentTime
        {
            get { return startTime + sw.Elapsed; }
        }

        // -----------------------------------------------------------------
        // Members
        // -----------------------------------------------------------------

        Thread processingThread;
        Thread recordingThread;

        // Some members for displaying the results
        private WriteableBitmap latestImg;
        private WriteableBitmap latestAlignedFace;
        private WriteableBitmap latestHOGdescriptor;

        // Managing the running of the analysis system
        private volatile bool threadRunning;
        private volatile bool threadPaused = false;
        // Allows for going forward in time step by step
        // Useful for visualising things
        private volatile int skipFrames = 0;

        FpsTracker processingFPS;

        // For selecting webcams
        CameraSelection camSelector;

        // For tracking
        FaceDetector faceDetector;
        FaceModelParameters faceModelParams;
        CLNF landmarkDetector;

        // For face analysis
        FaceAnalyserManaged faceAnalyser;
        GazeAnalyserManaged gazeAnalyser;

        public bool RecordAligned { get; set; } = false; // Aligned face images
        public bool RecordHOG { get; set; } = false; // HOG features extracted from face images
        public bool Record2DLandmarks { get; set; } = true; // 2D locations of facial landmarks (in pixels)
        public bool Record3DLandmarks { get; set; } = true; // 3D locations of facial landmarks (in pixels)
        public bool RecordModelParameters { get; set; } = true; // Facial shape parameters (rigid and non-rigid geometry)
        public bool RecordPose { get; set; } = true; // Head pose (position and orientation)
        public bool RecordAUs { get; set; } = true; // Facial action units
        public bool RecordGaze { get; set; } = true; // Eye gaze
        public bool RecordTracked { get; set; } = true; // Recording tracked videos or images

        // Visualisation options
        public bool ShowTrackedVideo { get; set; } = true; // Showing the actual tracking
        public bool ShowAppearance { get; set; } = false; // Showing appeaance features like HOG
        public bool ShowGeometry { get; set; } = true; // Showing geometry features, pose, gaze, and non-rigid
        public bool ShowAUs { get; set; } = true; // Showing Facial Action Units

        int imageOutputSize = 112;
        public bool MaskAligned { get; set; } = true; // Should the aligned images be masked

        // Selecting which face detector will be used
        public bool DetectorHaar { get; set; } = false;
        public bool DetectorHOG { get; set; } = false;
        public bool DetectorCNN { get; set; } = true;

        // Selecting which landmark detector will be used
        public bool LandmarkDetectorCLM { get; set; } = false;
        public bool LandmarkDetectorCLNF { get; set; } = false;
        public bool LandmarkDetectorCECLM { get; set; } = true;

        // For AU prediction, if videos are long dynamic models should be used
        public bool DynamicAUModels { get; set; } = true;

        // Camera calibration parameters
        public float fx = -1, fy = -1, cx = -1, cy = -1;

        // Virtual overlay image
        OverlayImage overlayImage;

        // Recording
        SequenceReader reader;
        RecorderOpenFace recorder;
        private Object recording_lock = new Object();
        private ConcurrentQueue<Tuple<RawImage>> recording_objects;
        private bool recording = false;
        private int cameraId;
        private int frameWidth;
        private int frameHeight;
        private readonly string logStreamerFilename = "openface.parameters.csv";
        private readonly string videoStreamerFilename = "openface._ID_.avi";

        public OpenFaceConnector()
        {
            processingFPS = new FpsTracker();
            overlayImage = new OverlayImage();
        }

        public void SetStatus(string newStatus)
        {
            status = newStatus;
        }

        public string GetStatus()
        {
            return status;
        }

        public void Init()
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                faceModelParams = new FaceModelParameters(root, LandmarkDetectorCECLM, LandmarkDetectorCLNF, LandmarkDetectorCLM);
                // Initialize the face detector
                faceDetector = new FaceDetector(faceModelParams.GetHaarLocation(), faceModelParams.GetMTCNNLocation());

                // If MTCNN model not available, use HOG
                if (!faceDetector.IsMTCNNLoaded())
                {
                    DetectorCNN = false;
                    DetectorHOG = true;
                }
                faceModelParams.SetFaceDetector(DetectorHaar, DetectorHOG, DetectorCNN);

                landmarkDetector = new CLNF(faceModelParams);

                gazeAnalyser = new GazeAnalyserManaged();

                SetStatus(Const.STATUS_READY);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SetStatus(Const.STATUS_UNAVAILABLE);
            }
        }

        public void Connect()
        {
            SelectWebcam();
        }

        public void Configure()
        {
            SelectWebcam();
        }

        public void Disconnect()
        {
            if (processingThread != null)
            {
                // Stop capture and tracking
                threadRunning = false;
                processingThread.Join();
            }
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/openface", new List<string>());
        }

        private void ReloadLandmarkDetector()
        {
            bool reload = false;
            if (faceModelParams.IsCECLM() && !LandmarkDetectorCECLM)
            {
                reload = true;
            }
            else if (faceModelParams.IsCLNF() && !LandmarkDetectorCLNF)
            {
                reload = true;
            }
            else if (faceModelParams.IsCLM() && !LandmarkDetectorCLM)
            {
                reload = true;
            }

            if (reload)
            {
                String root = AppDomain.CurrentDomain.BaseDirectory;

                faceModelParams = new FaceModelParameters(root, LandmarkDetectorCECLM, LandmarkDetectorCLNF, LandmarkDetectorCLM);
                landmarkDetector = new CLNF(faceModelParams);
            }
        }

        private void StopTracking()
        {
            // First complete the running of the thread
            if (processingThread != null)
            {
                // Tell the other thread to finish
                threadRunning = false;
                processingThread.Join();
            }
        }


        public void SelectWebcam()
        {
            StopTracking();

            // If camera selection has already been done, no need to re-populate the list as it is quite slow
            if (camSelector == null)
            {
                camSelector = new CameraSelection();
            }
            else
            {
                camSelector = new CameraSelection(camSelector.cams)
                {
                    Visibility = System.Windows.Visibility.Visible
                };
            }

            // Set the icon
            //Uri iconUri = new Uri("logo1.ico", UriKind.RelativeOrAbsolute);
            //camSelector.Icon = BitmapFrame.Create(iconUri);

            if (!camSelector.noCamerasFound)
            {
                camSelector.ShowDialog();
                SetStatus(Const.STATUS_UNAVAILABLE);
            }

            if (camSelector.cameraSelected)
            {
                cameraId = camSelector.selectedCamera.Item1;
                frameWidth = camSelector.selectedCamera.Item2;
                frameHeight = camSelector.selectedCamera.Item3;

                reader = new SequenceReader(cameraId, frameWidth, frameHeight, fx, fy, cx, cy);

                processingThread = new Thread(() => ProcessSequence(reader));
                processingThread.Name = "Webcam processing";
                processingThread.Start();

                SetStatus(Const.STATUS_CONNECTED);
            }
            else
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
            }
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        private void DetectorNotFoundWarning()
        {
            string messageBoxText = "Could not open the landmark detector model file. For instructions of how to download them, see https://github.com/TadasBaltrusaitis/OpenFace/wiki/Model-download";
            string caption = "Model file not found or corrupt";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

        }

        // When the processing is done re-enable the components
        private void EndMode()
        {
            latestImg = null;
            skipFrames = 0;
        }

        // The main function call for processing sequences
        private void ProcessSequence(SequenceReader reader)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            threadRunning = true;

            // Reload the face landmark detector if needed
            ReloadLandmarkDetector();

            if (!landmarkDetector.isLoaded())
            {
                DetectorNotFoundWarning();
                EndMode();
                threadRunning = false;
                return;
            }

            // Set the face detector
            faceModelParams.SetFaceDetector(DetectorHaar, DetectorHOG, DetectorCNN);
            faceModelParams.optimiseForVideo();

            // Setup the visualization
            Visualizer visualizer_of = new Visualizer(ShowTrackedVideo || RecordTracked, ShowAppearance, ShowAppearance, false);

            // Initialize the face analyser
            faceAnalyser = new FaceAnalyserManaged(AppDomain.CurrentDomain.BaseDirectory, DynamicAUModels, imageOutputSize, MaskAligned);

            // Reset the tracker
            landmarkDetector.Reset();

            // Loading an image file
            var frame = reader.GetNextImage();
            var gray_frame = reader.GetCurrentFrameGray();

            // For FPS tracking
            DateTime? startTime = CurrentTime;
            var lastFrameTime = CurrentTime;

            // Empty image would indicate that the stream is over
            while (!gray_frame.IsEmpty)
            {

                if (!threadRunning)
                {
                    break;
                }

                double progress = reader.GetProgress();

                bool detection_succeeding = landmarkDetector.DetectLandmarksInVideo(frame, faceModelParams, gray_frame);

                // The face analysis step (for AUs and eye gaze)
                faceAnalyser.AddNextFrame(frame, landmarkDetector.CalculateAllLandmarks(), detection_succeeding, false);

                gazeAnalyser.AddNextFrame(landmarkDetector, detection_succeeding, reader.GetFx(), reader.GetFy(), reader.GetCx(), reader.GetCy());

                // Only the final face will contain the details
                VisualizeFeatures(frame, visualizer_of, landmarkDetector.CalculateAllLandmarks(), landmarkDetector.GetVisibilities(), detection_succeeding, true, false, reader.GetFx(), reader.GetFy(), reader.GetCx(), reader.GetCy(), progress);

                // Record an observation
                if(recorder != null && !recorderDisposed)
                {
                    RecordObservation(recorder, visualizer_of.GetVisImage(), 0, detection_succeeding, reader.GetFx(), reader.GetFy(), reader.GetCx(), reader.GetCy(), reader.GetTimestamp(), reader.GetFrameNumber());

                    if (recording_objects != null && recording)
                    {
                        // Record video
                        lock (recording_lock)
                        {
                            // Add objects to recording queues
                            recording_objects.Enqueue(new Tuple<RawImage>(frame));
                        }
                    }
                }
                
                // Next frame
                while (threadRunning & threadPaused && skipFrames == 0)
                {
                    Thread.Sleep(10);
                }

                if (skipFrames > 0)
                {
                    skipFrames--;
                }

                frame = reader.GetNextImage();
                gray_frame = reader.GetCurrentFrameGray();

                lastFrameTime = CurrentTime;
                processingFPS.AddFrame();

            }

            // Close the open video/webcam
            reader.Close();

            EndMode();

        }

        private void VisualizeFeatures(RawImage frame, Visualizer visualizer, List<Tuple<float, float>> landmarks, List<bool> visibilities, bool detection_succeeding,
           bool new_image, bool multi_face, float fx, float fy, float cx, float cy, double progress)
        {

            List<Tuple<Point, Point>> lines = null;
            List<Tuple<float, float>> eye_landmarks = null;
            List<Tuple<Point, Point>> gaze_lines = null;
            Tuple<float, float> gaze_angle = new Tuple<float, float>(0, 0);

            List<float> pose = new List<float>();
            landmarkDetector.GetPose(pose, fx, fy, cx, cy);
            List<float> non_rigid_params = landmarkDetector.GetNonRigidParams();

            double confidence = landmarkDetector.GetConfidence();

            if (confidence < 0)
            {
                confidence = 0;
            }
            else if (confidence > 1)
            {
                confidence = 1;
            }

            double scale = landmarkDetector.GetRigidParams()[0];

            // Helps with recording and showing the visualizations
            if (new_image)
            {
                visualizer.SetImage(frame, fx, fy, cx, cy);
            }
            visualizer.SetObservationHOG(faceAnalyser.GetLatestHOGFeature(), faceAnalyser.GetHOGRows(), faceAnalyser.GetHOGCols());
            visualizer.SetObservationLandmarks(landmarks, confidence, visibilities);
            visualizer.SetObservationPose(pose, confidence);
            visualizer.SetObservationGaze(gazeAnalyser.GetGazeCamera().Item1, gazeAnalyser.GetGazeCamera().Item2, landmarkDetector.CalculateAllEyeLandmarks(), landmarkDetector.CalculateAllEyeLandmarks3D(fx, fy, cx, cy), confidence);

            eye_landmarks = landmarkDetector.CalculateVisibleEyeLandmarks();
            lines = landmarkDetector.CalculateBox(fx, fy, cx, cy);

            gaze_lines = gazeAnalyser.CalculateGazeLines(fx, fy, cx, cy);
            gaze_angle = gazeAnalyser.GetGazeAngle();

            // Visualisation (as a separate function)
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new TimeSpan(0, 0, 0, 0, 200), (Action)(() =>
            {
                Dictionary<string, double> au_classes = new Dictionary<String, double>();
                Dictionary<string, double> au_regs = new Dictionary<String, double>();

                if (ShowAUs)
                {
                    au_classes = faceAnalyser.GetCurrentAUsClass();
                    au_regs = faceAnalyser.GetCurrentAUsReg();

                    var au_regs_scaled = new Dictionary<String, double>();
                    foreach (var au_reg in au_regs)
                    {
                        au_regs_scaled[au_reg.Key] = au_reg.Value / 5.0;
                        if (au_regs_scaled[au_reg.Key] < 0)
                        {
                            au_regs_scaled[au_reg.Key] = 0;
                        }

                        if (au_regs_scaled[au_reg.Key] > 1)
                        {
                            au_regs_scaled[au_reg.Key] = 1;
                        }
                    }
                }

                if (ShowGeometry)
                {
                    int yaw = (int)(pose[4] * 180 / Math.PI + 0.5);
                    int roll = (int)(pose[5] * 180 / Math.PI + 0.5);
                    int pitch = (int)(pose[3] * 180 / Math.PI + 0.5);

                    // Update eye gaze
                    String x_angle = String.Format("{0:F0}°", gaze_angle.Item1 * (180.0 / Math.PI));
                    String y_angle = String.Format("{0:F0}°", gaze_angle.Item2 * (180.0 / Math.PI));
                }

                if (ShowTrackedVideo)
                {
                    if (new_image)
                    {
                        latestImg = frame.CreateWriteableBitmap();
                        overlayImage.Clear();
                    }

                    frame.UpdateWriteableBitmap(latestImg);

                    // Clear results from previous image
                    overlayImage.Source = latestImg;
                    overlayImage.Confidence.Add(confidence);
                    //overlayImage.FPS = processingFPS.GetFPS();
                    overlayImage.Progress = progress;
                    overlayImage.FaceScale.Add(scale);

                    // Update results even if it is not succeeding when in multi-face mode
                    if (detection_succeeding || multi_face)
                    {

                        List<Point> landmark_points = new List<Point>();
                        foreach (var p in landmarks)
                        {
                            landmark_points.Add(new Point(p.Item1, p.Item2));
                        }

                        List<Point> eye_landmark_points = new List<Point>();
                        foreach (var p in eye_landmarks)
                        {
                            eye_landmark_points.Add(new Point(p.Item1, p.Item2));
                        }

                        overlayImage.OverlayLines.Add(lines);
                        overlayImage.OverlayPoints.Add(landmark_points);
                        overlayImage.OverlayPointsVisibility.Add(visibilities);
                        overlayImage.OverlayEyePoints.Add(eye_landmark_points);
                        overlayImage.GazeLines.Add(gaze_lines);

                        PropagateRates(landmarks, eye_landmarks, au_classes, au_regs);

                    }
                }

                if (ShowAppearance)
                {
                    RawImage aligned_face = faceAnalyser.GetLatestAlignedFace();
                    RawImage hog_face = visualizer.GetHOGVis();

                    if (latestAlignedFace == null)
                    {
                        latestAlignedFace = aligned_face.CreateWriteableBitmap();
                        latestHOGdescriptor = hog_face.CreateWriteableBitmap();
                    }

                    aligned_face.UpdateWriteableBitmap(latestAlignedFace);
                    hog_face.UpdateWriteableBitmap(latestHOGdescriptor);
                }


                // Propagate
                byte[] bitmapData = overlayImage.GetJPGFromImageControl();
                webSocketConnector.Propagate("/openface", bitmapData);

            }));

        }

        private void PropagateRates(List<Tuple<float, float>> OverlayPoints, List<Tuple<float, float>> GazePoints, Dictionary<string, double> actionUnitsClasses, Dictionary<string, double> actionUnitsValues)
        {
            JArray jOverlayPoints = new JArray();
            foreach (var p in OverlayPoints)
            {
                jOverlayPoints.Add(new JObject(
                    new JProperty("x", p.Item1),
                    new JProperty("y", p.Item2)
                ));
            }

            JArray jAusRegs = new JArray();
            foreach(var cl in actionUnitsClasses)
            {
                if (actionUnitsValues.ContainsKey(cl.Key))
                {
                    jAusRegs.Add(new JObject(
                        new JProperty("au", cl.Key),
                        new JProperty("presence", cl.Value),
                        new JProperty("value", actionUnitsValues[cl.Key])
                    ));
                }
            }

            JObject message = new JObject(
                new JProperty("overlayPoints", jOverlayPoints),
                new JProperty("actionUnits", jAusRegs)
            );

            webSocketConnector.Propagate("/openface", message.ToString());
        }

        // Capturing and processing the video frame by frame
        private void RecordingLoop(string outputFolder)
        {
            // Set up the recording objects first
            Thread.CurrentThread.IsBackground = true;
            VideoWriter video_writer = null;

            double fps = processingFPS.GetFPS();
            String filename_video = outputFolder + "/" + videoStreamerFilename.Replace("_ID_", Stopwatch.GetTimestamp().ToString());
            video_writer = new VideoWriter(filename_video, frameWidth, frameHeight, fps, true);

            while (recording)
            {
                if (recording_objects.TryDequeue(out Tuple<RawImage> recording_object))
                {
                    video_writer.Write(recording_object.Item1);
                }
                Thread.Sleep(10);
            }

        }

        private void RecordObservation(RecorderOpenFace recorder, RawImage vis_image, int face_id, bool success, float fx, float fy, float cx, float cy, double timestamp, int frame_number)
        {

            //recorder.SetObservationTimestamp(timestamp);
            recorder.SetObservationTimestamp(Stopwatch.GetTimestamp());

            double confidence = landmarkDetector.GetConfidence();

            List<float> pose = new List<float>();
            landmarkDetector.GetPose(pose, fx, fy, cx, cy);
            recorder.SetObservationPose(pose);

            List<Tuple<float, float>> landmarks_2D = landmarkDetector.CalculateAllLandmarks();
            List<Tuple<float, float, float>> landmarks_3D = landmarkDetector.Calculate3DLandmarks(fx, fy, cx, cy);
            List<float> global_params = landmarkDetector.GetRigidParams();
            List<float> local_params = landmarkDetector.GetNonRigidParams();

            recorder.SetObservationLandmarks(landmarks_2D, landmarks_3D, global_params, local_params, confidence, success);

            var gaze = gazeAnalyser.GetGazeCamera();
            var gaze_angle = gazeAnalyser.GetGazeAngle();

            var landmarks_2d_eyes = landmarkDetector.CalculateAllEyeLandmarks();
            var landmarks_3d_eyes = landmarkDetector.CalculateAllEyeLandmarks3D(fx, fy, cx, cy);
            recorder.SetObservationGaze(gaze.Item1, gaze.Item2, gaze_angle, landmarks_2d_eyes, landmarks_3d_eyes);

            var au_regs = faceAnalyser.GetCurrentAUsReg();
            var au_classes = faceAnalyser.GetCurrentAUsClass();
            recorder.SetObservationActionUnits(au_regs, au_classes);

            recorder.SetObservationFaceID(face_id);
            recorder.SetObservationFrameNumber(frame_number);

            recorder.SetObservationFaceAlign(faceAnalyser.GetLatestAlignedFace());

            var hog_feature = faceAnalyser.GetLatestHOGFeature();
            recorder.SetObservationHOG(success, hog_feature, faceAnalyser.GetHOGRows(), faceAnalyser.GetHOGCols(), faceAnalyser.GetHOGChannels());

            recorder.SetObservationVisualization(vis_image);

            recorder.WriteObservation();

        }

        public void StartRecording(string outputFolder)
        {
            if(reader == null)
            {
                return;
            }

            RecorderOpenFaceParameters rec_params = new RecorderOpenFaceParameters(true, true, true, true,
                true, true, true, true, false, true, false, false, 
                reader.GetFx(), reader.GetFy(), reader.GetCx(), reader.GetCy(), reader.GetFPS());
            recorder = new RecorderOpenFace(logStreamerFilename, rec_params, outputFolder);
            recorderDisposed = false;

            recording = true;
            recording_objects = new ConcurrentQueue<Tuple<RawImage>>();
            recordingThread = new Thread(() => RecordingLoop(outputFolder))
            {
                Name = "Recording loop"
            };
            recordingThread.Start();
        }

        public void StopRecording()
        {
            if(recorder == null || recorderDisposed)
            {
                return;
            }
            recorder.Close();
            recorder.Dispose();
            recorderDisposed = true;
            recording = false;
            recordingThread.Join();
        }
    }
}
