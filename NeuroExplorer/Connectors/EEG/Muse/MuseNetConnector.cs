using NeuroExplorer.WebSocket;
using System;
using System.ComponentModel;
using NeuroExplorer.LogWriter;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Harthoorn.MuseClient;
using System.Threading;
using NeuroExplorer.Connectors.EEG.UI;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace NeuroExplorer.Connectors.EEG
{

    public class MuseData
    {
        public string Name;
        public string Mac;
        public ulong Address;
        public short SignalStrengh;
        public override string ToString()
        {
            return $"Device: {Name} Address: {Address} - Signal: {SignalStrengh} dBmW";
        }
    }

    class MuseNetConnector : IWebSocketPropagator
    {

        private WebSocketConnector webSocketConnector;
        private string status;
        private MuseSelection deviceSelection;
        private ObservableCollection<string> devicesList { get; set; }
        private List<MuseData> devicesListTyped;
        private BleScanner scanner;
        private MuseClient client = new MuseClient();

        private int selectedMuse = 0;
        private MuseData defaultMuse;

        Thread processingThread;

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "eeg.jsonl";

        public MuseNetConnector()
        {
            defaultMuse = new MuseData()
             {
                 Name = "Muse-58D3",
                 Mac = LongToMac(368741210323),
                 Address = 368741210323,
                 SignalStrengh = 0
             };
        }

        public string LongToMac(ulong lMacAddr)
        {
            return String.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
            (lMacAddr >> (8 * 5)) & 0xff,
            (lMacAddr >> (8 * 4)) & 0xff,
            (lMacAddr >> (8 * 3)) & 0xff,
            (lMacAddr >> (8 * 2)) & 0xff,
            (lMacAddr >> (8 * 1)) & 0xff,
            (lMacAddr >> (8 * 0)) & 0xff);
        }

        public void Init()
        {
            client.NotifyEeg += Client_NotifyEeg;
            client.NotifyTelemetry += Client_NotifyTelemetry;
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

        public void InitDeviceSelector()
        {
            scanner = new BleScanner();
            scanner.OnAdvertise += ScanAdvertisement;
            scanner.ScanStart();

            deviceSelection = new MuseSelection()
            {
                Visibility = Visibility.Visible,
            };

            deviceSelection.Closed += DeviceSelection_Closed;

            devicesListTyped = new List<MuseData>();
            devicesList = new ObservableCollection<string>();

            deviceSelection.devices.ItemsSource = devicesList;
            deviceSelection.select.Click += Select_Click;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            selectedMuse = deviceSelection.devices.SelectedIndex;
            HideDeviceSelector();
        }

        private void DeviceSelection_Closed(object sender, EventArgs e)
        {
            scanner.ScanStop();
        }

        public void HideDeviceSelector()
        {
            scanner.ScanStop();
            deviceSelection.Visibility = Visibility.Hidden;

            if (deviceSelection.devices.SelectedIndex == -1)
            {
                return;
            }

            processingThread = new Thread(() => StartMuseClientAsync())
            {
                Name = "EEG connection"
            };

            processingThread.Start();

        }

        private void StartMuseClientAsync()
        {
            Task.Run(async () =>
            {
                bool ok = await client.Connect(devicesListTyped[selectedMuse].Address);
                if (ok)
                {
                    SetStatus(Const.STATUS_CONNECTED);
                    await client.Subscribe(
                        Channel.EEG_AF7,
                        Channel.EEG_AF8,
                        Channel.EEG_TP10,
                        Channel.EEG_TP9,
                        Channel.EEG_AUX,
                        Channel.Telemetry,
                        Channel.Control);

                    await client.Resume();
                } else
                {
                    SetStatus(Const.STATUS_UNAVAILABLE);
                }
            }).Wait();
        }

        private void Client_NotifyEeg(Channel channel, Encefalogram encefalogram)
        {
            bool found = false;
            string channelString = "";
            switch (channel)
            {
                case Channel.EEG_TP9:
                    channelString = "tp9";
                    found = true;
                    break;
                case Channel.EEG_AF7:
                    channelString = "af7";
                    found = true;
                    break;
                case Channel.EEG_AF8:
                    channelString = "af8";
                    found = true;
                    break;
                case Channel.EEG_TP10:
                    channelString = "tp10";
                    found = true;
                    break;
            }
            if(!found)
            {
                return;
            }
            JObject message = new JObject(
                new JProperty("ts", Stopwatch.GetTimestamp()),
                new JProperty(channelString, encefalogram.Samples)
            );
            string stringMessage = message.ToString(Formatting.None);
            logStreamer.Write(stringMessage);
            webSocketConnector.Propagate("/eeg", stringMessage);
        }

        private void Client_NotifyTelemetry(Telemetry obj)
        {
            JObject message = new JObject(
                new JProperty("ts", Stopwatch.GetTimestamp()),
                new JProperty("batt", obj.BatteryLevel)
            );
            string stringMessage = message.ToString(Formatting.None);
            logStreamer.Write(stringMessage);
            webSocketConnector.Propagate("/eeg", stringMessage);
        }

        private void ScanAdvertisement(Advertisement adv)
        {
            if(adv.Name.ToLower().Contains("muse") == false)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MuseData newMuse = new MuseData()
                    {
                        Name = adv.Name,
                        Address = adv.Address,
                        SignalStrengh = adv.SignalStrengh
                    };
                    devicesListTyped.Add(newMuse);
                    devicesList.Add(newMuse.ToString());
            });
        }

        public void SetStatus(string newStatus)
        {
            status = newStatus;
        }

        public string GetStatus()
        {
            return status;
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        public void Disconnect()
        {
            if (client.Connected)
            {
                Task.Run(async () => { await client.Disconnect(); }).Wait();
                processingThread.Join();
            }

            SetStatus(Const.STATUS_DISCONNECTED);
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
