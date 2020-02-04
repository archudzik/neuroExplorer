using libStreamSDK;
using NeuroExplorer.Helpers.SerialPortWrapper;
using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NeuroExplorer.Connectors.EEG.NeuroSky
{
    class NeuroSkyConnector : IWebSocketPropagator
    {
        private string comPort = "";
        private int connectionID = 0;
        private WebSocketConnector webSocketConnector;
        private string status;
        private Thread processingThread;
        private bool threadRunning = false;
        private bool isDisposed = true;
        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "eeg.jsonl";

        public void Configure()
        {
            MessageBox.Show("No options available", "NeuroSky EEG", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Connect()
        {
            connectionID = NativeThinkgear.TG_GetNewConnectionId();

            if (connectionID < 0)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            if (String.IsNullOrEmpty(comPort))
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            int errCode = NativeThinkgear.TG_Connect(connectionID,
                          "\\\\.\\" + comPort,
                          NativeThinkgear.Baudrate.TG_BAUD_57600,
                          NativeThinkgear.SerialDataFormat.TG_STREAM_PACKETS);

            if (errCode < 0)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            processingThread = new Thread(() => ReadValues())
            {
                Name = "NeuroSky EEG"
            };

            processingThread.Start();

        }

        private void ReadValues()
        {
            isDisposed = false;
            threadRunning = true;
            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            Dictionary<NativeThinkgear.DataType, string> dataType = new Dictionary<NativeThinkgear.DataType, string>()
            {
                {NativeThinkgear.DataType.TG_DATA_BATTERY, "battery" },
                {NativeThinkgear.DataType.TG_DATA_POOR_SIGNAL, "poorSignal" },
                {NativeThinkgear.DataType.TG_DATA_ATTENTION, "attention" },
                {NativeThinkgear.DataType.TG_DATA_MEDITATION, "meditation" },
                {NativeThinkgear.DataType.TG_DATA_RAW, "raw" },
                {NativeThinkgear.DataType.TG_DATA_DELTA, "delta" },
                {NativeThinkgear.DataType.TG_DATA_THETA, "theta" },
                {NativeThinkgear.DataType.TG_DATA_ALPHA1, "alpha1" },
                {NativeThinkgear.DataType.TG_DATA_ALPHA2, "alpha2" },
                {NativeThinkgear.DataType.TG_DATA_BETA1, "beta1" },
                {NativeThinkgear.DataType.TG_DATA_BETA2, "beta2" },
                {NativeThinkgear.DataType.TG_DATA_GAMMA1, "gamma1" },
                {NativeThinkgear.DataType.TG_DATA_GAMMA2, "gamma2" },
                {NativeThinkgear.DataType.MWM15_DATA_FILTER_TYPE, "filter" }
            };


        int errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 1);
            if (errCode == 0)
            {

                SetStatus(Const.STATUS_CONNECTED);

                errCode = NativeThinkgear.MWM15_setFilterType(connectionID, NativeThinkgear.FilterType.MWM15_FILTER_TYPE_50HZ);

                while (threadRunning)
                {
                    foreach (KeyValuePair<NativeThinkgear.DataType, string> data in dataType)
                    {
                        if (NativeThinkgear.TG_GetValueStatus(connectionID, data.Key) != 0)
                        {
                            JObject message = new JObject(
                                new JProperty("ts", Stopwatch.GetTimestamp()),
                                new JProperty("param", data.Value),
                                new JProperty("value", NativeThinkgear.TG_GetValue(connectionID, data.Key))
                            );
                            string stringMessage = message.ToString(Formatting.None);
                            logStreamer.Write(stringMessage);
                            webSocketConnector.Propagate("/eeg", stringMessage);
                        }
                    }
                        
                }

                errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 0);

            } else
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
            }

            NativeThinkgear.TG_Disconnect(connectionID); 
            NativeThinkgear.TG_FreeConnection(connectionID);
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void Disconnect()
        {
            if (isDisposed)
            {
                return;
            }
            if (processingThread != null)
            {
                threadRunning = false;
                processingThread.Join();
            }

            SetStatus(Const.STATUS_DISCONNECTED);
            isDisposed = true;
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
            comPort = "";
            if (String.IsNullOrEmpty(comPort))
            {
                List<KeyValuePair<string, string>> devices = SerialPortController.GetBluetoothPorts();
                foreach (KeyValuePair<string, string> entry in devices)
                {
                    if (entry.Key.IndexOf("MindWave") > -1)
                    {
                        comPort = entry.Value;
                    }
                }
            }

            if (String.IsNullOrEmpty(comPort))
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            SetStatus(Const.STATUS_READY);
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/eeg", new List<string>());
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
