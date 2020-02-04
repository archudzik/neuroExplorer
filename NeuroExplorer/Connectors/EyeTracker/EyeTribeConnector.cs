using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using EyeTribe.Controls.Calibration;
using NeuroExplorer.Connectors.EyeTracker.UI;
using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroExplorer.Connectors.EyeTracker
{
    class EyeTribeConnector : IWebSocketPropagator, ICalibrationStateListener, ICalibrationResultListener, IGazeListener
    {
        private WebSocketConnector webSocketConnector;
        private Process serverProcess = null;
        private List<Tuple<string, string>> serverProcessMessages = new List<Tuple<string, string>>();
        private Screen ActiveScreen = null;
        private EyeTribeUI eyeTribeUI = null;
        Thread processingThread;
        private string status;

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "eyetracker.jsonl";

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/eyetracker", new List<string>());
            SetStatus(Const.STATUS_DISCONNECTED);
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
            RunServer(30);
            SetStatus(Const.STATUS_READY);
        }

        public void Connect()
        {
            GazeManager.Instance.Activate(apiVersion: GazeManagerCore.ApiVersion.VERSION_1_0);
            GazeManager.Instance.AddGazeListener(this);
            GazeManager.Instance.AddCalibrationResultListener(this);
            GazeManager.Instance.AddCalibrationStateListener(this);

            if (!GazeManager.Instance.IsActivated)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                System.Windows.MessageBox.Show("EyeTribe Server has not been started", "Eyetracker", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetStatus(Const.STATUS_CONNECTED);
        }

        public void Configure()
        {
            if (eyeTribeUI != null)
            {
                eyeTribeUI.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            eyeTribeUI = new EyeTribeUI()
            {
                Visibility = System.Windows.Visibility.Visible
            };
            UpdateScreensList();
            eyeTribeUI.calibrate.Click += Calibrate_Click;
            if (GazeManager.Instance.IsCalibrated)
            {
                eyeTribeUI.rating.Value = RatingFunction(GazeManager.Instance.LastCalibrationResult);
            }
        }

        public void Disconnect()
        {
            GazeManager.Instance.RemoveGazeListener(this);
            GazeManager.Instance.RemoveCalibrationResultListener(this);
            GazeManager.Instance.RemoveCalibrationStateListener(this);
            GazeManager.Instance.Deactivate();
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void RunServer(int framerate = 30)
        {

            foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
            {
                if (myProc.ProcessName == "EyeTribe")
                {
                    try
                    {
                        myProc.Kill();
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Please, turn off the Eye Tribe Server");
                        return;
                    }
                }
            }

            string serverPath = Path.Combine(Directory.GetCurrentDirectory(), "Connectors", "EyeTracker", "EyeTribeServer", "EyeTribe.exe");

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = serverPath,
                Arguments = string.Format("--framerate={0}", framerate),
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            void start()
            {
                serverProcess = Process.Start(info);
                serverProcess.WaitForExit();
            }
            processingThread = new Thread(start)
            {
                Name = "Eye tribe output"
            };
            processingThread.Start();
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();

            if (serverProcess != null)
            {
                try
                {
                    serverProcess.Kill();
                }
                catch { }
            }
            if (processingThread != null)
            {
                processingThread.Join();
            }
        }

        public void UpdateScreensList()
        {
            int i = 0;
            List<string> screensList = new List<string>();
            foreach (Screen S in Screen.AllScreens)
            {
                string primary = S.Primary ? "primary" : "seconday";
                string workingArea = string.Format("{0} x {1}", S.WorkingArea.Width, S.WorkingArea.Height);
                screensList.Add(string.Format("Display {0}, {1}, {2}", ++i, primary, workingArea));
            }
            eyeTribeUI.screens.ItemsSource = screensList;
            eyeTribeUI.screens.SelectedIndex = 0;
        }

        public void Calibrate()
        {
            if (GazeManager.Instance.Trackerstate.ToString() == "TRACKER_NOT_CONNECTED")
            {
                return;
            }
            if (eyeTribeUI != null)
            {
                eyeTribeUI.Visibility = System.Windows.Visibility.Hidden;
            }
            ActiveScreen = Screen.AllScreens[eyeTribeUI.screens.SelectedIndex];
            int calibrationPoints = int.Parse(eyeTribeUI.calibrationPoints.Text);
            CalibrationRunner calRunner = new CalibrationRunner(ActiveScreen, ActiveScreen.Bounds.Size, calibrationPoints)
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0)),
                PointColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 141, 191)),
                PointLatencyTime = 350,
                PointRecordingTime = 550,
                PointTransitionTime = 250
            };
            calRunner.OnResult += CalRunner_OnResult;
            calRunner.Start();
        }

        private void CalRunner_OnResult(object sender, CalibrationRunnerEventArgs e)
        {
            switch (e.Result)
            {
                case CalibrationRunnerResult.Success:
                    eyeTribeUI.messageSnackbarContent.Content = "Calibration success " + e.CalibrationResult.AverageErrorDegree + "°";
                    break;

                case CalibrationRunnerResult.Abort:
                    eyeTribeUI.messageSnackbarContent.Content = "The calibration was aborted. Reason: " + e.Message;
                    break;

                case CalibrationRunnerResult.Error:
                    eyeTribeUI.messageSnackbarContent.Content = "An error occured during calibration. Reason: " + e.Message;
                    break;

                case CalibrationRunnerResult.Failure:
                    eyeTribeUI.messageSnackbarContent.Content = "Calibration failed. Reason: " + e.Message;
                    break;

                case CalibrationRunnerResult.Unknown:
                    eyeTribeUI.messageSnackbarContent.Content = "Calibration exited with unknown state. Reason: " + e.Message;
                    break;
            }
            eyeTribeUI.messageSnackbar.IsActive = true;
        }

        private void Calibrate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Calibrate();
        }

        public void OnCalibrationChanged(bool isCalibrated, CalibrationResult calibResult)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                if (eyeTribeUI != null)
                {
                    eyeTribeUI.Visibility = System.Windows.Visibility.Visible;
                    eyeTribeUI.rating.Value = RatingFunction(calibResult);
                }
            }));
        }

        public int RatingFunction(CalibrationResult result)
        {
            if (result == null)
            {
                return 0;
            }
            var accuracy = result.AverageErrorDegree;
            if (accuracy < 0.5)
            {
                return 5;
            }
            if (accuracy < 0.7)
            {
                return 4;
            }
            if (accuracy < 1)
            {
                return 3;
            }
            if (accuracy < 1.5)
            {
                return 2;
            }
            return 1;
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            JObject message = new JObject(
                new JProperty("ts", Stopwatch.GetTimestamp()),
                new JProperty("state", gazeData.StateToString()),
                new JProperty("gaze",
                    new JObject(
                        new JProperty("isFixated", gazeData.IsFixated),
                        new JProperty("smooth",
                            new JObject(
                                new JProperty("x", gazeData.SmoothedCoordinates.X),
                                new JProperty("y", gazeData.SmoothedCoordinates.Y)
                            )
                        ),
                        new JProperty("raw",
                            new JObject(
                                new JProperty("x", gazeData.RawCoordinates.X),
                                new JProperty("y", gazeData.RawCoordinates.Y)
                            )
                        ),
                        new JProperty("leftEye",
                            new JObject(
                                new JProperty("pupil",
                                    new JObject(
                                        new JProperty("x", gazeData.LeftEye.PupilCenterCoordinates.X),
                                        new JProperty("y", gazeData.LeftEye.PupilCenterCoordinates.Y),
                                        new JProperty("size", gazeData.LeftEye.PupilSize)
                                    )
                                ),
                                new JProperty("raw",
                                    new JObject(
                                        new JProperty("x", gazeData.LeftEye.RawCoordinates.X),
                                        new JProperty("y", gazeData.LeftEye.RawCoordinates.Y)
                                    )
                                ),
                                new JProperty("smooth",
                                    new JObject(
                                        new JProperty("x", gazeData.LeftEye.SmoothedCoordinates.X),
                                        new JProperty("y", gazeData.LeftEye.SmoothedCoordinates.Y)
                                    )
                                )
                            )
                        ),
                        new JProperty("rightEye",
                            new JObject(
                                new JProperty("pupil",
                                    new JObject(
                                        new JProperty("x", gazeData.RightEye.PupilCenterCoordinates.X),
                                        new JProperty("y", gazeData.RightEye.PupilCenterCoordinates.Y),
                                        new JProperty("size", gazeData.RightEye.PupilSize)
                                    )
                                ),
                                new JProperty("raw",
                                    new JObject(
                                        new JProperty("x", gazeData.RightEye.RawCoordinates.X),
                                        new JProperty("y", gazeData.RightEye.RawCoordinates.Y)
                                    )
                                ),
                                new JProperty("smooth",
                                    new JObject(
                                        new JProperty("x", gazeData.RightEye.SmoothedCoordinates.X),
                                        new JProperty("y", gazeData.RightEye.SmoothedCoordinates.Y)
                                    )
                                )
                            )
                        )
                    )
                )
            );
            string stringMessage = message.ToString(Formatting.None);
            logStreamer.Write(stringMessage);
            webSocketConnector.Propagate("/eyetracker", stringMessage);
        }

        public void OnCalibrationStateChanged(bool isCalibrating, bool isCalibrated)
        {

        }

        public void StartRecording(string outputFolder)
        {
            logStreamer.Init(Path.Combine(outputFolder, logStreamerFilename));
        }

        public void StopRecording()
        {
            logStreamer.Terminate();
        }
    }
}