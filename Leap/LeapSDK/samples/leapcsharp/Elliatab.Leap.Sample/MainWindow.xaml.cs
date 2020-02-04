using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Elliatab.Leap.Sample
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Controller leapController;
        private readonly ObservableCollection<string> logs;

        public MainWindow()
        {
            InitializeComponent();
            logs = new ObservableCollection<string>();
            this.DataContext = this.logs;
        }

        private void ButtonClick1(object sender, RoutedEventArgs e)
        {
            if (this.leapController != null)
            {
                this.leapController.FrameAcquired -= OnFrameAcquired;
                this.leapController.Dispose();
                this.leapController = null;
            }
            else
            {
            
                this.leapController = new Controller();
                this.leapController.FrameAcquired += OnFrameAcquired;
            }
        }

        private void OnFrameAcquired(object sender, Frame frame)
        {
            var newLine = "Frame id: " + frame.Id +
                       ", timestamp: " + frame.Timestamp +
                       ", hands: " + frame.Hands.Count +
                       ", fingers: " + frame.Fingers.Count +
                       ", tools: " + frame.Tools.Count;

            this.logs.Add(newLine);
        }
    }
}
