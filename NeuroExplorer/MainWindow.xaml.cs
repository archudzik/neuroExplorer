using NeuroExplorer.MainController;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace NeuroExplorer
{

    public partial class MainWindow : Window
    {

        private Process dashboard;
        private Process experiment;
        private MainController.MainController controller;

        public MainWindow()
        {
            InitializeComponent();

            controller = new MainController.MainController();
            Closing += controller.Closing;

            dashboard = StartViewer("dashboard", false);
            experiment = StartViewer("experiment", true);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                dashboard.Exited -= Process_Exited;
                experiment.Exited -= Process_Exited;
                if (!dashboard.HasExited)
                {
                    dashboard.CloseMainWindow();
                    dashboard.Close();
                }
                if (!experiment.HasExited)
                {
                    experiment.CloseMainWindow();
                    experiment.Close();
                }
                Application.Current.Shutdown();
            }));
        }

        private Process StartViewer(string layout, bool keyboardHook)
        {
            string viewerPath = Path.Combine(Directory.GetCurrentDirectory(), "Connectors", "Cef", "NeuroExplorerViewer.exe");
            string layoutPath = Path.Combine(Directory.GetCurrentDirectory(), "Layout", layout + ".html");

            string arguments = "--url=\"" + layoutPath + "\"";
            if (keyboardHook)
            {
                arguments += " --fs=true";
            }

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = viewerPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            Process process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = info,
            };
            process.Exited += Process_Exited;
            process.Start();
            return process;
        }
    }
}
