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
using System.Linq;
using System.Windows;

namespace NeuroExplorer.Connectors.GalvanicSkinResponse
{
    class ThoughtstreamConnector : IWebSocketPropagator
    {

        private SerialPortController serialController;
        private WebSocketConnector webSocketConnector;

        private static int resistancesRequired = 100;
        private double[] firstResistanceValues = new double[resistancesRequired];
        private double averageResistanceFactor = 0;
        private int resistanceSample = 0;
        private string status;

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "gsr.jsonl";

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/gsr", new List<string>());
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
            string comPort = "";
            if (String.IsNullOrEmpty(comPort))
            {
                List<KeyValuePair<string, string>> devices = SerialPortController.GetComPortsByVID("10C4", "EA60");
                foreach (KeyValuePair<string, string> entry in devices)
                {
                    if (entry.Key.IndexOf("ThoughtStream") > -1)
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
            settings.Parity = Parity.None;
            settings.DataBits = 8;
            settings.StopBits = StopBits.One;
            settings.DtrEnable = false;
            settings.RtsEnable = true;
            settings.ReceivedBytesThreshold = 1;

            serialController.NewSerialDataRecieved += PortManager_NewSerialDataRecieved;

            SetStatus(Const.STATUS_READY);
        }

        public void Connect()
        {
            if (serialController == null)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }
            serialController.StartListening();
            SetStatus(Const.STATUS_CONNECTED);
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

        public void Configure()
        {
            MessageBox.Show("No options available", "Thoughtstream GSR Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        private void PropagateRates(string status, int adcValue, double resistanceValue, double percentChange, double resistance_kOhm, double conductivity_uSiemens)
        {
            JObject message = new JObject(
                new JProperty("ts", Stopwatch.GetTimestamp()),
                new JProperty("status", status),
                new JProperty("adcValue", adcValue),
                new JProperty("resistanceValue", resistanceValue),
                new JProperty("percentChange", percentChange),
                new JProperty("resistance_kOhm", resistance_kOhm),
                new JProperty("conductivity_uSiemens", conductivity_uSiemens)
            );
            string stringMessage = message.ToString(Formatting.None);
            logStreamer.Write(stringMessage);
            webSocketConnector.Propagate("/gsr", stringMessage);
        }

        void PortManager_NewSerialDataRecieved(object sender, SerialDataEventArgs received)
        {
            var data = received.Data;
            if ((data[0] != 0xa3) || (data[1] != 0x5b) || (data[2] != 8))
            {
                return;
            }
            else
            {
                int adcValue = (data[3] << 8) + data[4];
                double resistanceValue = (7700010000 / adcValue) - 470000;
                byte bits = data[5];
                int probeError = (bits & 1) != 0 ? 1 : 0;
                int lowBattery = (bits & 2) != 0 ? 1 : 0;
                int newData = (bits & 4) != 0 ? 1 : 0;
                int recalculationOccurred = (bits & 8) != 0 ? 1 : 0;

                int checksumByte1 = data[6];
                int checksumByte2 = data[7];
                int checksum = (checksumByte1 << 8) + checksumByte2;
                int success = 0xa3 + 0x5b + 0x8 + data[3] + data[4] + bits == checksum ? 1 : 0;
                double percentChange = 0;
                double resistance_kOhm = resistanceValue / 1000;
                double conductivity_uSiemens = (1 / resistanceValue) * 1000000;

                if (success == 1)
                {
                    if (resistanceSample < resistancesRequired)
                    {
                        firstResistanceValues[resistanceSample++] = resistanceValue;
                    }
                    else if (averageResistanceFactor == 0)
                    {
                        averageResistanceFactor = 100 / firstResistanceValues.Average();
                    }
                    else if (averageResistanceFactor > 0)
                    {
                        percentChange = 100 - (resistanceValue * averageResistanceFactor);
                    }
                    string status = "OK";
                    if(probeError > 0)
                    {
                        status = "PROBE_ERROR";
                    } 
                    if(lowBattery > 0)
                    {
                        status = "LOW_BATTERY";
                    }
                    PropagateRates(status, adcValue, resistanceValue, percentChange, resistance_kOhm, conductivity_uSiemens);
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
