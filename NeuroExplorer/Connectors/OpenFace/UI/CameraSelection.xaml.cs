using NeuroExplorer.Connectors.OpenFace.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NeuroExplorer
{
    /// <summary>
    /// Interaction logic for CameraSelection.xaml
    /// </summary>
    public partial class CameraSelection : Window
    {

        List<Border> sampleImages;
        List<ComboBox> comboBoxes;

        // id, width, height
        public Tuple<int, int, int> selectedCamera;

        List<List<Tuple<int, int>>> resolutionsAll;
        int selectedCameraIndex = -1;

        // indicate if user clicked on camera
        public bool cameraSelected = false;

        public bool noCamerasFound = false;

        public List<Tuple<int, String, List<Tuple<int, int>>, OpenCVWrappers.RawImage>> cams;

        public void PopulateCameraSelections()
        {
            // Finding the cameras here
            if (cams == null)
            {
                String root = AppDomain.CurrentDomain.BaseDirectory;
                //cams = CameraInterop.Capture.GetCameras(root);
                cams = UtilitiesOF.SequenceReader.GetCameras(root);
            }

            int i = 0;

            sampleImages = new List<Border>();

            // Each cameras corresponding resolutions
            resolutionsAll = new List<List<Tuple<int, int>>>();
            comboBoxes = new List<ComboBox>();

            foreach (var s in cams)
            {

                if(s.Item2 == "TheEyeTribe" || s.Item2 == "Leap Dev Kit")
                {
                    continue;
                }

                WriteableBitmap b;
                try
                {
                    b = s.Item4.CreateWriteableBitmap();
                    s.Item4.UpdateWriteableBitmap(b);
                    b.Freeze();
                }
                catch (Exception)
                {
                    continue;
                }

                Dispatcher.Invoke(() =>
                {

                    CameraItem cameraItem = new CameraItem();
                    cameraItem.CameraIndex = i;
                    cameraItem.preview.Source = b;
                    cameraItem.description.Content = s.Item2;
                    cameraItem.SetValue(Grid.ColumnProperty, i);
                    cameraItem.SetValue(Grid.RowProperty, 0);

                    foreach (var r in s.Item3)
                    {
                        cameraItem.resolutions.Items.Add(r.Item1 + " x " + r.Item2);
                    }

                    cameraItem.select.Click += (object sender, RoutedEventArgs e) =>
                    {
                        if (cameraItem.resolutions.SelectedIndex == -1)
                        {
                            cameraItem.messageSnackbar.IsActive = true;
                            return;
                        }
                        string[] selectedRes = cameraItem.resolutions.SelectedValue.ToString().Split(new[] { " x " }, StringSplitOptions.None);
                        bool parse1 = Int32.TryParse(selectedRes[0], out int width);
                        bool parse2 = Int32.TryParse(selectedRes[1], out int height);
                        if (parse1 && parse2)
                        {
                            Select(cameraItem.CameraIndex, width, height);
                        }
                    };

                    ColumnDefinition col_def = new ColumnDefinition();
                    ThumbnailPanel.ColumnDefinitions.Add(col_def);
                    ThumbnailPanel.Children.Add(cameraItem);

                });

                i++;

            }

            Dispatcher.Invoke(() =>
            {
                CenterWindowOnScreen();
            });

            if (cams.Count > 0)
            {
                noCamerasFound = false;
            }
            else
            {
                string messageBoxText = "No cameras detected, please connect a webcam";
                string caption = "Camera error!";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
                selectedCameraIndex = -1;
                noCamerasFound = true;
                Dispatcher.Invoke(DispatcherPriority.Render, new TimeSpan(0, 0, 0, 0, 200), (Action)(() =>
                {
                    this.Close();
                }));
            }
        }

        private void Select(int camIndex, int resWidth, int resHeight)
        {
            cameraSelected = true;
            selectedCamera = new Tuple<int, int, int>(camIndex, resWidth, resHeight);
            this.Close();
        }

        public CameraSelection()
        {
            InitializeComponent();
            // We want to display the loading screen first
            Thread load_cameras = new Thread(LoadCameras);
            load_cameras.Start();
        }

        public void LoadCameras()
        {
            Thread.CurrentThread.IsBackground = true;
            PopulateCameraSelections();

            Dispatcher.Invoke(DispatcherPriority.Render, new TimeSpan(0, 0, 0, 0, 200), (Action)(() =>
            {
                LoadingGrid.Visibility = System.Windows.Visibility.Hidden;
                camerasPanel.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        public CameraSelection(List<Tuple<int, String, List<Tuple<int, int>>, OpenCVWrappers.RawImage>> cams)
        {
            InitializeComponent();
            this.cams = cams;
            PopulateCameraSelections();

            Dispatcher.Invoke(DispatcherPriority.Render, new TimeSpan(0, 0, 0, 0, 200), (Action)(() =>
            {
                LoadingGrid.Visibility = System.Windows.Visibility.Hidden;
                camerasPanel.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        private void CenterWindowOnScreen()
        {
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
            this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
        }
    }
}
