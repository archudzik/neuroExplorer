using NeuroExplorer.Helpers.SerialPortWrapper;
using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace NeuroExplorer.Connectors.PulseOximetry
{
    class Cms50eConnector : IWebSocketPropagator
    {

        private SerialPortController serialController;
        private WebSocketConnector webSocketConnector;

        private string status;
        private int currentIndex = 0;
        private byte[] currentPackage = new byte[5];

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "pulse.jsonl";

        public Cms50eConnector()
        {

        }

        public void SetStatus(string newStatus)
        {
            status = newStatus;
        }

        public string GetStatus()
        {
            return status;
        }

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/pulse", new List<string>());
        }

        public void Init()
        {
            string comPort = "";
            if (String.IsNullOrEmpty(comPort))
            {
                List<KeyValuePair<string, string>> devices = SerialPortController.GetComPortsByVID("10C4", "EA60");
                foreach (KeyValuePair<string, string> entry in devices)
                {
                    if (entry.Key.IndexOf("000") > -1)
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

            serialController = new SerialPortController();
            SerialSettings settings = serialController.CurrentSerialSettings;

            settings.BaudRate = 19200;
            settings.PortName = comPort;
            settings.Parity = Parity.Odd;
            settings.DataBits = 8;
            settings.StopBits = StopBits.One;

            serialController.NewSerialDataRecieved += PortManager_NewSerialDataRecieved;
            SetStatus(Const.STATUS_READY);
        }

        public void Connect()
        {
            if (serialController == null)
            {
                SetStatus(Const.STATUS_UNCONFIGURED);
                return;
            }
            serialController.StartListening();
            SetStatus(Const.STATUS_CONNECTED);
        }

        public void Configure()
        {
            System.Windows.MessageBox.Show("This hardware has no configuration options", "Pulse Oximeter Configuration", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public void Disconnect()
        {
            if (serialController == null)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }
            serialController.StopListening();
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        public static bool IsFirstByteOfPacket(byte b)
        {
            return GetIntFromByte(b, 7, 7) == 1;
        }

        public static int GetIntFromByte(byte b, int from, int to)
        {
            string binary = Convert.ToString(b, 2).PadLeft(8, '0');
            binary = Reverse(binary);
            string pulseBinary = binary.Substring(from, to - from + 1);
            pulseBinary = Reverse(pulseBinary);
            return Convert.ToInt32(pulseBinary, 2);
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void PropagateRates(string status, int signalStrength = -1, int pulseWaveform = -1, int barGraph = -1, int pulse = -1, int spo2 = -1)
        {
            JObject message = new JObject(
                new JProperty("ts", Stopwatch.GetTimestamp()),
                new JProperty("status", status),
                new JProperty("signalStrength", signalStrength),
                new JProperty("pulseWaveform", pulseWaveform),
                new JProperty("barGraph", barGraph),
                new JProperty("pulse", pulse),
                new JProperty("spo2", spo2)
            );
            string stringMessage = message.ToString(Formatting.None);
            logStreamer.Write(stringMessage);
            webSocketConnector.Propagate("/pulse", stringMessage);
        }

        void PortManager_NewSerialDataRecieved(object sender, SerialDataEventArgs received)
        {
            var data = received.Data;

            for (int i = 0; i < data.Length; i++)
            {
                var currentByte = data[i];

                if (currentIndex == 0 && IsFirstByteOfPacket(currentByte))
                {
                    // First package
                    currentPackage[currentIndex] = currentByte;
                    currentIndex++;
                }
                else if (currentIndex > 0 && !IsFirstByteOfPacket(currentByte))
                {
                    // Next package
                    currentPackage[currentIndex] = currentByte;
                    currentIndex++;
                }
                else if (currentIndex > 0 && IsFirstByteOfPacket(currentByte))
                {
                    // Bad package
                    currentIndex = 0;
                    PropagateRates("BAD_PACKET");
                }

                if (currentIndex == 5)
                {
                    currentIndex = 0;

                    // Submit package
                    int signal_strength = GetIntFromByte(currentPackage[0], 0, 3);
                    int searching_time_status = GetIntFromByte(currentPackage[0], 4, 4); // 1=searching too long，0=OK
                    int spo2_status = GetIntFromByte(currentPackage[0], 5, 5); // 1=dropping of SpO2，0=OK
                    int beep_status = GetIntFromByte(currentPackage[0], 6, 6); // 1=beep flag
                    int probe_status = GetIntFromByte(currentPackage[2], 4, 4); // 1=probe error，0=OK
                    int searching_status = GetIntFromByte(currentPackage[2], 4, 4); //1=searching，0=OK

                    int pulse_waveform = GetIntFromByte(currentPackage[1], 0, 6);
                    int bar_graph = GetIntFromByte(currentPackage[2], 0, 6);
                    int pulse = GetIntFromByte(currentPackage[3], 0, 6);
                    int spo2 = GetIntFromByte(currentPackage[4], 0, 6);

                    if (signal_strength > 8)
                    {
                        signal_strength = 8;
                    }

                    if ((spo2 == 0) || (pulse == 0))
                    {
                        PropagateRates("NO_FINGER");
                        continue;
                    }
                    else
                    {
                        double signal_strength_percent = signal_strength * 12.5;
                        signal_strength_percent = System.Math.Round(signal_strength_percent, 0);
                        PropagateRates("OK", signal_strength, pulse_waveform, bar_graph, pulse, spo2);
                    }
                }
            }
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
