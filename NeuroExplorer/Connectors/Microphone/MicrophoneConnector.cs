using NAudio.Wave; // for sound card access
using NeuroExplorer.Connectors.Microphone.UI;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NeuroExplorer.Connectors.Microphone
{
    class MicrophoneConnector : IWebSocketPropagator
    {

        private WebSocketConnector webSocketConnector;
        private WaveIn waveIn;
        private WaveFileWriter writer;
        private object __locker = new object();
        private object __writer = new object();
        private volatile bool threadRunning = true;
        Thread processingThread;
        MicrophoneSelection micSel;
        float peak;
        private string status;
        private bool writerDisposed = true;

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/mic", new List<string>());
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
            SetStatus(Const.STATUS_UNCONFIGURED);
        }

        public void Connect()
        {
            InitDeviceSelector();
        }

        public void Configure()
        {
            InitDeviceSelector();
        }

        public void Disconnect()
        {
            StopPropagation();
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void Init(int deviceNumber)
        {
            StopRecording();
            StopPropagation();

            waveIn = new WaveIn
            {
                WaveFormat = new WaveFormat(44100, 16, 1),
                DeviceNumber = deviceNumber
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();

            threadRunning = true;
            processingThread = new Thread(() => PropagateValues())
            {
                Name = "Microphone processing"
            };
            processingThread.Start();
            SetStatus(Const.STATUS_CONNECTED);
        }

        public void InitDeviceSelector()
        {
            micSel = new MicrophoneSelection()
            {
                Visibility = System.Windows.Visibility.Visible
            };

            List<string> devicesList = new List<string>();
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                devicesList.Add(string.Format("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels));
            }

            micSel.devices.ItemsSource = devicesList;
            micSel.devices.SelectedIndex = 0;
            micSel.select.Click += Select_Click;
        }

        private void Select_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Init(micSel.devices.SelectedIndex);
            micSel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            lock (__writer)
            {
                if (!writerDisposed)
                {
                    writer.Write(e.Buffer, 0, e.BytesRecorded);

                    if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 60)
                    {
                        StopRecording();
                    }
                }
            }

            float max = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                var sample32 = sample / 32768f;
                if (sample32 < 0)
                {
                    sample32 = -sample32;
                }

                if (sample32 > max)
                {
                    max = sample32;
                }
            }
            peak = max;
        }

        private void PropagateValues()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            while (threadRunning)
            {
                lock (__locker)
                {
                    webSocketConnector.Propagate("/mic", new JObject(new JProperty("peak", peak * 100)).ToString());
                }
                Thread.Sleep(16);
            }
        }

        private void StopPropagation()
        {
            if (processingThread != null)
            {
                threadRunning = false;
                processingThread.Join();
            }
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
            }
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
            }
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            StopPropagation();
        }

        public void StartRecording(string outputFolder)
        {
            if (waveIn != null) { 
                string filename = DateTime.Now.ToString("yyyy-MM-dd_HH'-'mm'-'ss") + ".wav";
                writer = new WaveFileWriter(Path.Combine(outputFolder, filename), waveIn.WaveFormat);
                writerDisposed = false;
            }
        }

        public void StopRecording()
        {
            lock (__writer)
            {
                if (writer == null)
                {
                    return;
                }
                writer.Close();
                writer.Dispose();
                writerDisposed = true;
            }
        }
    }
}
