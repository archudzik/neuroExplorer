/*
 * Copyright (c) 2013-present, The Eye Tribe. 
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree. 
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Size = System.Drawing.Size;
using WndSize = System.Windows.Size;
using PointF = System.Drawing.PointF;

namespace EyeTribe.Controls.Calibration
{
    public partial class CalibrationRunner : ICalibrationProcessHandler, ITrackerStateListener
    {
        #region Variables

        private const string MESSAGE_FOLLOW = "Follow the circle..";
        private const string MESSAGE_COMPUTING = "Processing calibration, please wait.";

        private const double FADE_IN_TIME = 2.0; //sec
        private const double FADE_OUT_TIME = 0.5; //sec
        private DoubleAnimation animateIn;
        private readonly DoubleAnimation animateOut;
        private CalibrationPointWpf calPointWpf;

        private Screen screen = Screen.PrimaryScreen;
        private Size calibrationAreaSize;
        private static Visibility helpVisibility = Visibility.Collapsed;
        private static VerticalAlignment verticalAlignment = VerticalAlignment.Center;
        private static HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;

        private const double TARGET_PADDING = 0.1;
        private const int NUM_MAX_CALIBRATION_ATTEMPTS = 3;
        private const int NUM_MAX_RESAMPLE_POINTS = 3;

        private int count = 9;
        private int latencyMs = 500;
        private int transitionTimeMs = 1000;
        private int pointRecordingTime = 1000;
        private int reSamplingCount;
        private bool calibrationFormReady = false;

        private DispatcherTimer timerLatency;
        private DispatcherTimer timerRecording;
        private Queue<PointF> points;
        private PointF currentPoint;
        private static readonly Random Random = new Random();

        private double scaleW = 1.0;
        private double scaleH = 1.0;
        private double offsetX;
        private double offsetY;

        private bool trackerStateOK = false;
        private bool isAborting = false;

        private double DPI;

        #endregion

        #region Events

        public event EventHandler<CalibrationRunnerEventArgs> OnResult;

        #endregion

        #region Get/Set

        public Screen Screen
        {
            get { return screen; }
            set { screen = value; }
        }

        public Size CalibrationAreaSize
        {
            get { return calibrationAreaSize; }
            set { calibrationAreaSize = value; }
        }

        public SolidColorBrush BackgroundColor
        {
            get { return CalibrationCanvas.Background as SolidColorBrush; }
            set { CalibrationCanvas.Background = value; }
        }

        public VerticalAlignment Vertical_Alignment
        {
            get { return verticalAlignment; }
            set { verticalAlignment = value; }
        }

        public HorizontalAlignment Horizontal_Alignment
        {
            get { return horizontalAlignment; }
            set { horizontalAlignment = value; }
        }

        public Visibility HelpVisibility
        {
            get { return CalibrationHelp.Visibility; }
            set { CalibrationHelp.Visibility = value; }
        }

        public int PointCount
        {
            get { return count; }
            set { count = value; }
        }

        public int PointLatencyTime
        {
            get { return latencyMs; }
            set { latencyMs = value; }
        }

        public int PointTransitionTime
        {
            get { return transitionTimeMs; }
            set { transitionTimeMs = value; }
        }
        public int PointRecordingTime
        {
            get
            {
                return pointRecordingTime;
            }
            set
            {
                pointRecordingTime = value;
                calPointWpf.AnimationTimeMilliseconds = pointRecordingTime;
            }
        }

        public SolidColorBrush PointColor
        {
            get { return calPointWpf.PointColor; }
            set { calPointWpf.PointColor = value; }
        }

        #endregion

        #region Constructor

        public CalibrationRunner() : this(Screen.PrimaryScreen, Screen.PrimaryScreen.Bounds.Size, 9) { }

        public CalibrationRunner(Screen screen, Size calibrationAreaSize, int pointCount)
        {
            this.screen = screen;
            this.calibrationAreaSize = calibrationAreaSize;
            this.count = pointCount;

            DPI = Utility.GetMonitorScaleDpi(screen);


            InitializeComponent();

            // Listen for changes in device/connectivity state 
            GazeManager.Instance.AddTrackerStateListener(this);
            OnTrackerStateChanged(GazeManager.Instance.Trackerstate);

            // Create the calibration point and add it to the canvas
            calPointWpf = new CalibrationPointWpf(new WndSize(DPI * screen.Bounds.Width, DPI * screen.Bounds.Height));
            CalibrationCanvas.Children.Add(calPointWpf);

            // Set the properties of the CalibrationWindow
            BackgroundColor = new SolidColorBrush(Colors.DarkGray);
            PointColor = new SolidColorBrush(Colors.White);

            PointRecordingTime = 1000;
            Opacity = 0;

            // Create the animation-out object and close form when completed
            animateOut = new DoubleAnimation(0, TimeSpan.FromSeconds(FADE_OUT_TIME));
            animateOut.From = 1.0;
            animateOut.To = 0.0;
            animateOut.AutoReverse = false;
            animateOut.Completed += delegate { Close(); };

            this.Cursor = System.Windows.Input.Cursors.None;
        }

        #endregion

        #region Public methods

        public void Start()
        {
            if (trackerStateOK != true)
            {
                RaiseResult(CalibrationRunnerResult.Error, "Device is not in a valid state, cannot calibrate.");
                return;
            }

            try
            {
                Show();
            }
            catch (Exception ex)
            {
                RaiseResult(CalibrationRunnerResult.Error, "Unable to show the calibration window. Please try again.");
                return;
            }

            // Place the "fake" center target on canvas
            double winWidth = Math.Round(DPI * screen.Bounds.Width, 0);
            double winHeight = Math.Round(DPI * screen.Bounds.Height, 0);
            Canvas.SetLeft(calPointWpf, winWidth / 2);
            Canvas.SetTop(calPointWpf, winHeight / 2);

            // Start the calibration process
            DoStart();
        }

        public void ShowMessage(string message)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => ShowMessage(message)));
                return;
            }

            labelMessage.Content = message;
        }

        #region Interface Implementation

        public void OnScreenStatesChanged(int screenIndex, int screenResolutionWidth, int screenResolutionHeight, float screenPhysicalWidth, float screenPhysicalHeight)
        { }

        public void OnTrackerStateChanged(GazeManager.TrackerState trackerState)
        {
            trackerStateOK = false;
            string errorMessage = string.Empty;

            switch (trackerState)
            {
                case GazeManager.TrackerState.TRACKER_CONNECTED:
                    trackerStateOK = true;
                    break;

                case GazeManager.TrackerState.TRACKER_CONNECTED_NOUSB3:
                    errorMessage = "Device connected to a USB2.0 port";
                    break;

                case GazeManager.TrackerState.TRACKER_CONNECTED_BADFW:
                    errorMessage = "A firmware updated is required.";
                    break;

                case GazeManager.TrackerState.TRACKER_NOT_CONNECTED:
                    errorMessage = "Device not connected.";
                    break;

                case GazeManager.TrackerState.TRACKER_CONNECTED_NOSTREAM:
                    errorMessage = "No data coming out of the sensor.";
                    break;
            }

            // All good
            if (trackerStateOK || isAborting)
                return;

            // Invalid connection/device state, abort calibration and notify user
            if (trackerStateOK == false)
                AbortCalibration(errorMessage);
        }

        public void OnCalibrationStarted()
        {
            // tracker engine is ready to calibrate - check if we can start to calibrate
            if (calibrationFormReady && currentPoint != null)
                DrawCalibrationPoint(currentPoint);
        }

        public void OnCalibrationProgress(double progress)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new MethodInvoker(() => OnCalibrationProgress(progress)));
                return;
            }

            // TODO: API Client provides the wrong value when resampling more than one point.
            // Progress is always 1 regardless if theres more points to sample
            //if (progress.Equals(1.0)) // done
            //    return;

            // transition to next point
            PointF curPos = currentPoint;
            PointF nextPos = PickNextPoint();

            if ((int)nextPos.X == -1 && (int)nextPos.Y == -1) // no more points?
                return;

            // Store next point as current (global)
            currentPoint = nextPos;

            // Animate transition to next position
            Console.Out.WriteLine("Transition set to " + transitionTimeMs);
            DoubleAnimation cX = CreateTransitionAnimation(DPI * curPos.X, DPI * nextPos.X, transitionTimeMs);
            DoubleAnimation cY = CreateTransitionAnimation(DPI * curPos.Y, DPI * nextPos.Y, transitionTimeMs);
            cX.Completed += delegate { DrawCalibrationPoint(currentPoint); };

            calPointWpf.BeginAnimation(Canvas.LeftProperty, cX);
            calPointWpf.BeginAnimation(Canvas.TopProperty, cY);
        }

        public void OnCalibrationProcessing()
        {
            ShowMessage(MESSAGE_COMPUTING);
        }

        public void OnCalibrationResult(CalibrationResult res)
        {
            // Invoke on UI thread
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => OnCalibrationResult(res)));
                return;
            }

            // No result?
            if (res == null || res.Calibpoints == null)
            {
                RaiseResult(CalibrationRunnerResult.Error, "Calibration result is empty.");
                StopAndClose();
                return;
            }

            Console.Out.WriteLine("CalibrationResult, avg: " + res.AverageErrorDegree + " left: " + res.AverageErrorDegreeLeft + " right: " + res.AverageErrorDegreeRight);

            // Success, check results for bad points
            foreach (CalibrationPoint cp in res.Calibpoints)
            {
                if (cp == null || cp.Coordinates == null)
                    continue;

                // Tracker tells us to resample this point, enque it
                if (cp.State == CalibrationPoint.STATE_RESAMPLE || cp.State == CalibrationPoint.STATE_NO_DATA)
                {
                    points.Enqueue(new PointF((float)cp.Coordinates.X, (float)cp.Coordinates.Y));
                    Console.Out.WriteLine("Recal adding " + cp.Coordinates.X + " " + cp.Coordinates.Y + " PointsInQueue: " + points.Count);
                }
            }

            // Time to stop?
            if (reSamplingCount > NUM_MAX_CALIBRATION_ATTEMPTS || points.Count > NUM_MAX_RESAMPLE_POINTS)
            {
                AbortCalibration(CalibrationRunnerResult.Failure, "Unable to calibrate.");
                StopAndClose();
                return;
            }

            // Resample?
            if (points != null && points.Count > 0)
            {
                reSamplingCount++;
                // Transition from last point to first resample point
                PointF nextPos = points.Peek(); // peek here, RunPointSequence pulls out of queue
                DoubleAnimation cX = CreateTransitionAnimation(DPI * currentPoint.X, DPI * nextPos.X, transitionTimeMs);
                DoubleAnimation cY = CreateTransitionAnimation(DPI * currentPoint.Y, DPI * nextPos.Y, transitionTimeMs);
                cX.Completed += delegate { RunPointSequence(); }; // once moved, start sequence
                calPointWpf.BeginAnimation(Canvas.LeftProperty, cX);
                calPointWpf.BeginAnimation(Canvas.TopProperty, cY);
                return;
            }

            RaiseResult(CalibrationRunnerResult.Success, string.Empty, res);
            StopAndClose();
        }

        #endregion

        #endregion

        #region Private

        DateTime pointStart;

        private void DoStart()
        {
            ShowMessage(MESSAGE_FOLLOW);
            calPointWpf.Visibility = Visibility.Visible;

            reSamplingCount = 0;
            points = CreatePointList();

            // run the fade-in animation
            animateIn = new DoubleAnimation(0, TimeSpan.FromSeconds(FADE_IN_TIME))
            {
                From = 0.0,
                To = 1.0,
                AutoReverse = false
            };

            animateIn.Completed += AnimateInCompleted;

            // Start window fade in
            BeginAnimation(OpacityProperty, animateIn);
        }

        private void AnimateInCompleted(object sender, EventArgs e)
        {
            calibrationFormReady = true;
            CalibrationMessage.Visibility = Visibility.Hidden;
            Focus();

            // windows faded in, now animate from center to first point
            PointF firstPos = points.Peek();
            double centerX = DPI * screen.Bounds.Width / 2;
            double centerY = DPI * screen.Bounds.Height / 2;
            DoubleAnimation cX = CreateTransitionAnimation(centerX, DPI * firstPos.X, transitionTimeMs);
            DoubleAnimation cY = CreateTransitionAnimation(centerY, DPI * firstPos.Y, transitionTimeMs);
            cX.Completed += delegate { RunPointSequence(); }; // once moved to first point, start sequence

            calPointWpf.BeginAnimation(Canvas.LeftProperty, cX);
            calPointWpf.BeginAnimation(Canvas.TopProperty, cY);
        }

        #region Timers

        private void CreateTimerRecording()
        {
            if (timerRecording != null)
                return;

            timerRecording = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, pointRecordingTime) };

            timerRecording.Tick += delegate
            {
                TimeSpan duration = DateTime.Now.Subtract(pointStart);
                Console.Out.WriteLine("PointEnd: " + currentPoint.X + " " + currentPoint.Y + " Duration: " + duration.TotalMilliseconds);

                timerRecording.Stop();
                GazeManager.Instance.CalibrationPointEnd();

                // tracker server callbacks to interface methods, e.g. OnCalibrationProgressUpdate
                // which proceeds to MoveToPoint until OnCalibrationResults (the end) is called.
            };
        }

        private void CreateTimerLatency()
        {
            if (timerLatency != null)
                return;

            timerLatency = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, latencyMs) };
            timerLatency.Stop();

            timerLatency.Tick += delegate
            {
                timerLatency.Stop();

                if (currentPoint != null)
                {
                    // Signal tracker server that a point is starting, do the shrink animation and start timerRecording
                    pointStart = DateTime.Now;
                    Console.WriteLine("PointStart: " + currentPoint.X + " " + currentPoint.Y);

                    GazeManager.Instance.CalibrationPointStart((int)currentPoint.X, (int)currentPoint.Y);
                    AnimateCalibrationPoint();
                    timerRecording.Start();
                }
            };
        }

        #endregion

        #region Setup

        private void WindowContentRendered(object sender, EventArgs e)
        {
            // Adjust for DPI scaling
            Width = screen.Bounds.Width * DPI;
            Height = screen.Bounds.Height * DPI;
            Top = screen.Bounds.Y * DPI;
            Left = screen.Bounds.X * DPI;

            // Scale active area or use full screen if empty
            ActiveArea.Width = (int)(DPI * (ActiveArea.Width == 0 ? screen.Bounds.Width : ActiveArea.Width));
            ActiveArea.Height = (int)(DPI * (ActiveArea.Height == 0 ? screen.Bounds.Height : ActiveArea.Height));

            Show();
            Focus();
        }

        private void RunPointSequence()
        {
            isAborting = false;

            try
            {
                // Set up two timers, one for recording delay and another for recording duration
                // 1. When point is shown we start timerLatency, on tick we signal tracker to start sampling (for duration of timerRecording)
                // 2. A point is sampled for the duration of the timerRecording
                CreateTimerLatency();
                CreateTimerRecording();

                // Signal tracker server that we're about to start (not when recalibrating points)
                if (points.Count == PointCount)
                {
                    if (!GazeManager.Instance.IsCalibrating)
                    {
                        GazeManager.Instance.CalibrationStart((short)PointCount, this);
                    }
                    else
                    {
                        throw new Exception("Calibration is already running!?");
                    }
                }

                // Get first point, draw it, start timers etc.
                currentPoint = PickNextPoint();
                DrawCalibrationPoint(currentPoint);
            }
            catch (Exception ex)
            {
                RaiseResult(CalibrationRunnerResult.Error, "An error occured in the calibration. Message: " + ex.Message);
            }
        }

        private void RaiseResult(CalibrationRunnerResult result, string message)
        {
            if (OnResult != null)
                OnResult(this, new CalibrationRunnerEventArgs(result, message));
        }

        private void RaiseResult(CalibrationRunnerResult result, string message, CalibrationResult calibrationReport)
        {
            if (OnResult != null)
                OnResult(this, new CalibrationRunnerEventArgs(result, message, calibrationReport));
        }

        #endregion

        #region Drawing

        private void DrawCalibrationPoint(PointF position)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => DrawCalibrationPoint(position)));
                return;
            }

            if (position == null)
                return;

            Canvas.SetLeft(calPointWpf, DPI * position.X);
            Canvas.SetTop(calPointWpf, DPI * position.Y);
            calPointWpf.Visibility = Visibility.Visible;

            timerLatency.Start(); // Will issue PointStart and start timerRecording on tick
        }

        private void AnimateCalibrationPoint()
        {
            Dispatcher.Invoke(new Action(() => calPointWpf.StartAnimate()));
        }

        private DoubleAnimation CreateTransitionAnimation(double from, double to, int durationMs)
        {
            return new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                AccelerationRatio = 0.4,
                DecelerationRatio = 0.6
            };
        }

        #endregion

        #region Targets Logic

        private PointF PickNextPoint()
        {
            if (points == null)
                points = CreatePointList();

            if (points.Count != 0)
                return points.Dequeue();

            return new PointF(-1, -1);
        }

        private Queue<PointF> CreatePointList()
        {
            if (screen == null)
                screen = Screen.PrimaryScreen; // default to primary

            Size size = Screen.Bounds.Size;

            // if we are using a subset of the screen as calibration area
            if (!CalibrationAreaSize.IsEmpty)
            {
                scaleW = CalibrationAreaSize.Width / (double)size.Width;
                scaleH = CalibrationAreaSize.Height / (double)size.Height;

                offsetX = GetHorizontalAlignmentOffset();
                offsetY = GetVerticalAlignmentOffset();
            }

            // add some padding 
            double paddingHeight = TARGET_PADDING;
            double paddingWidth = (size.Height * TARGET_PADDING) / size.Width; // use the same distance for the width padding

            int columns = (int)Math.Round(Math.Sqrt(PointCount), 0);
            int rows = columns;
            if (PointCount == 12)
                columns += 1;

            PointF[] calibPoints = new PointF[PointCount];
            for (int dirX = 0; dirX < columns; dirX++)
            {
                for (int dirY = 0; dirY < rows; dirY++)
                {
                    double x = Lerp(paddingWidth, 1 - paddingWidth, (double)dirX / (columns - 1));
                    double y = Lerp(paddingHeight, 1 - paddingHeight, (double)dirY / (rows - 1));
                    calibPoints[(dirX * rows) + dirY] = new PointF(
                        (float)(offsetX + x * scaleW) * Screen.Bounds.Width,
                        (float)(offsetY + y * scaleH) * Screen.Bounds.Height
                    );
                }
            }

            // Shuffle point order
            calibPoints = calibPoints.OrderBy(x => Random.Next()).ToArray();

            return new Queue<PointF>(calibPoints);
        }

        private static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        private double GetVerticalAlignmentOffset()
        {
            double offsetY = 0.0;

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch: // center
                    offsetY = ((Screen.Bounds.Size.Height - CalibrationAreaSize.Height) / 2d) / (double)Screen.Bounds.Size.Height;
                    break;
                case VerticalAlignment.Bottom:
                    offsetY = (Screen.Bounds.Size.Height - CalibrationAreaSize.Height) / (double)Screen.Bounds.Size.Height;
                    break;
                case VerticalAlignment.Top:
                    offsetY = 0.0;
                    break;
            }
            return offsetY;
        }

        private double GetHorizontalAlignmentOffset()
        {
            double offsetX = 0.0;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch: // center
                    offsetX = ((Screen.Bounds.Size.Width - CalibrationAreaSize.Width) / 2d) / (double)Screen.Bounds.Size.Width;
                    break;
                case HorizontalAlignment.Right:
                    offsetX = (Screen.Bounds.Size.Width - CalibrationAreaSize.Width) / (double)Screen.Bounds.Size.Width;
                    break;
                case HorizontalAlignment.Left:
                    offsetX = 0.0;
                    break;
            }
            return offsetX;
        }

        #endregion

        #region Abort, Close

        private void MouseDbClick(object sender, MouseEventArgs e)
        {
            AbortCalibration(CalibrationRunnerResult.Abort, "User aborted calibration");
        }

        private void KeyUpDetected(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                AbortCalibration(CalibrationRunnerResult.Abort, "User aborted calibration");
        }

        private void AbortCalibration(string errorMessage)
        {
            AbortCalibration(CalibrationRunnerResult.Abort, errorMessage);
        }

        private void AbortCalibration(CalibrationRunnerResult type, string errorMessage)
        {
            if (isAborting)
                return; // Only one call is needed

            isAborting = true;
            GazeManager.Instance.CalibrationAbort();
            StopAndClose();
            RaiseResult(type, errorMessage);
        }

        private void StopAndClose()
        {
            animateIn.Completed -= AnimateInCompleted;

            if (timerLatency != null)
                timerLatency.Stop();

            if (timerRecording != null)
                timerRecording.Stop();

            CloseWindow();
        }

        private void CloseWindow()
        {
            Dispatcher.Invoke(new Action(() => BeginAnimation(OpacityProperty, animateOut)));
        }

        #endregion

        #endregion
    }
}
