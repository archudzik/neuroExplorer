using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpOSC;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace NeuroExplorer.Connectors.EEG
{
    class MuseOscConnector : IWebSocketPropagator
    {
        private WebSocketConnector webSocketConnector;
        private UDPListener udpListener;
        private string status;
        private bool isDisposed = true;

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "eeg.jsonl";

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/eeg", new List<string>());
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
            SetStatus(Const.STATUS_READY);
        }

        public void Connect()
        {
            if (udpListener != null || !isDisposed)
            {
                udpListener.Dispose();
            }
            void callback(OscPacket packet)
            {
                OscMessage messageReceived = (OscMessage)packet;
                string addr = messageReceived.Address.Substring(messageReceived.Address.IndexOf('/') + 1);
                List<object> args = messageReceived.Arguments;
                JObject message = new JObject(
                    new JProperty("ts", Stopwatch.GetTimestamp()),
                    new JProperty(addr, args)
                );
                string stringMessage = message.ToString(Formatting.None);
                logStreamer.Write(stringMessage);
                webSocketConnector.Propagate("/eeg", stringMessage);
            }
            udpListener = new UDPListener(7000, callback);
            SetStatus(Const.STATUS_CONNECTED);
            isDisposed = false;
        }

        public void Configure()
        {
            MessageBox.Show("Muse listens on port 7000. For configuration, use Muse Direct software", "Muse EEG Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Disconnect()
        {
            if (udpListener == null || isDisposed)
            {
                return;
            }
            isDisposed = true;
            udpListener.Close();
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
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
