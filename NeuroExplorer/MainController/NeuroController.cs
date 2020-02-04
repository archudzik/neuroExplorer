using NeuroExplorer.Connectors;
using NeuroExplorer.Connectors.Dynamometer;
using NeuroExplorer.Connectors.EEG;
using NeuroExplorer.Connectors.EEG.NeuroSky;
using NeuroExplorer.Connectors.EyeTracker;
using NeuroExplorer.Connectors.GalvanicSkinResponse;
using NeuroExplorer.Connectors.LeapMotion;
using NeuroExplorer.Connectors.Microphone;
using NeuroExplorer.Connectors.PulseOximetry;
using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace NeuroExplorer.MainController
{

    class NeuroController
    {
        IDictionary<string, IWebSocketPropagator> connectors = new Dictionary<string, IWebSocketPropagator>();
        WebSocketConnector webSocket = new WebSocketConnector();
        readonly LogStreamer logStreamer = new LogStreamer();
        readonly string logStreamerFilename = "annotations.jsonl";
        readonly string metadataFilename = "metadata.json";
        string examinationId = "";
        string outputFolder = "";
        GeoCoordinateWatcher watcher;
        GeoPosition<GeoCoordinate> currentPosition;

        public NeuroController()
        {
            webSocket.Connect("ws://127.0.0.1:7654");
            webSocket.RegisterEndpoint("/cmd", new List<string>());
            webSocket.onMessage += OnMessage;

            connectors.Add("leap", new LeapMotionConnector());
            connectors.Add("pulse", new Cms50eConnector());
            connectors.Add("gsr", new ThoughtstreamConnector());
            connectors.Add("eeg", new NeuroSkyConnector());
            connectors.Add("openface", new OpenFaceConnector());
            connectors.Add("mic", new MicrophoneConnector());
            connectors.Add("eyetracker", new EyeTribeConnector());
            connectors.Add("dynamometer", new VernierConnector());

            foreach (KeyValuePair<string, IWebSocketPropagator> connector in connectors)
            {
                connector.Value.SetWebSocket(webSocket);
                connector.Value.Init();
            }

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.TryStart(false, TimeSpan.FromMilliseconds(5000));
            watcher.PositionChanged += Watcher_PositionChanged;

        }

        public void Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (KeyValuePair<string, IWebSocketPropagator> connector in connectors)
            {
                connector.Value.OnClosing(sender, e);
            }
        }

        public void SendMessage(string msg)
        {
            webSocket.Propagate("/cmd", msg);
        }

        public void OnMessage(string msg)
        {
            JObject obj = JObject.Parse(msg);
            if (!obj.ContainsKey("to"))
            {
                return;
            }
            if (obj["to"].ToString() != "controller" && obj["to"].ToString() != "all")
            {
                return;
            }
            switch (obj["action"].ToString())
            {
                case "connect":
                case "configure":
                case "disconnect":
                    {
                        if (connectors.TryGetValue(obj["value"].ToString(), out IWebSocketPropagator connector))
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                switch (obj["action"].ToString())
                                {
                                    case "connect":
                                        connector.Connect();
                                        break;

                                    case "configure":
                                        connector.Configure();
                                        break;

                                    case "disconnect":
                                        connector.Disconnect();
                                        break;
                                }
                            }));
                        }
                    }
                    break;

                case "status":
                    {
                        IDictionary<string, string> statusList = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, IWebSocketPropagator> connector in connectors)
                        {
                            statusList.Add(connector.Key, connector.Value.GetStatus());
                        }
                        JObject message = new JObject(
                            new JProperty("from", "controller"),
                            new JProperty("to", obj["from"].ToString()),
                            new JProperty("action", "status"),
                            new JProperty("value", JObject.FromObject(statusList))
                        );
                        SendMessage(message.ToString());
                    }
                    break;

                case "record":
                    {
                        switch (obj["value"]["trigger"].ToString())
                        {
                            case "start":
                                {
                                    JObject message = new JObject(
                                        new JProperty("from", "controller"),
                                        new JProperty("to", obj["from"].ToString()),
                                        new JProperty("action", "recording"),
                                        new JProperty("value", true)
                                    );
                                    SendMessage(message.ToString());
                                    StartRecording(obj["value"]["details"]);
                                }
                                break;

                            case "stop":
                                {
                                    JObject message = new JObject(
                                        new JProperty("from", "controller"),
                                        new JProperty("to", obj["from"].ToString()),
                                        new JProperty("action", "recording"),
                                        new JProperty("value", false)
                                    );
                                    SendMessage(message.ToString());
                                    StopRecording();
                                }
                                break;
                        }
                    }
                    break;

                case "metadata":
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (!obj.ContainsKey("value") || (obj["value"]["examinationId"].ToString() == ""))
                            {
                                return;
                            }

                            examinationId = obj["value"]["examinationId"].ToString();
                            outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NeuroExplorer", "Examinations", examinationId);
                            Directory.CreateDirectory(outputFolder);

                            string filePath = Path.Combine(outputFolder, metadataFilename);
                            string stringMessage = obj["value"].ToString();
                            TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(filePath, FileMode.OpenOrCreate)));
                            writer.WriteLine(stringMessage);
                            writer.Close();
                            writer.Dispose();
                        }));
                    }
                    break;

                case "annotate":
                    {
                        if (!obj.ContainsKey("value"))
                        {
                            return;
                        }
                        if (examinationId == "" || outputFolder == "")
                        {
                            return;
                        }
                        JObject message = new JObject(
                            new JProperty("ts", Stopwatch.GetTimestamp()),
                            new JProperty("data", obj["value"]["data"])
                        );
                        string stringMessage = message.ToString(Formatting.None);
                        logStreamer.Write(stringMessage);
                    }
                    break;

                case "patients":
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            JObject patients = GetPatientsHistory();
                            JObject message = new JObject(
                                        new JProperty("from", "controller"),
                                        new JProperty("to", obj["from"].ToString()),
                                        new JProperty("action", "patients"),
                                        new JProperty("value", patients)
                                    );
                            SendMessage(message.ToString());
                        }));
                    }
                    break;

                case "folder":
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NeuroExplorer", "Examinations", obj["value"].ToString());
                    if (Directory.Exists(path))
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = path,
                                UseShellExecute = true,
                                Verb = "open"
                            });
                        }));
                    }
                    else
                    {
                        MessageBox.Show("Directory not found", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;

            }
        }

        private void StartRecording(JToken jToken)
        {
            examinationId = jToken["examinationId"].ToString();
            if (examinationId == "")
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NeuroExplorer", "Examinations", examinationId);
                Directory.CreateDirectory(outputFolder);
                logStreamer.Init(Path.Combine(outputFolder, logStreamerFilename));
                foreach (KeyValuePair<string, IWebSocketPropagator> connector in connectors)
                {
                    connector.Value.StartRecording(outputFolder);
                }
            }));
        }

        private void StopRecording()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (KeyValuePair<string, IWebSocketPropagator> connector in connectors)
                {
                    connector.Value.StopRecording();
                }
                logStreamer.Terminate();
            }));
        }

        private JObject GetPatientsHistory()
        {
            JObject patients = new JObject();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NeuroExplorer", "Examinations");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] directories = Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NeuroExplorer", "Examinations"));
            foreach (string dir in directories)
            {
                string metaPath = Path.Combine(dir, metadataFilename);
                if (!File.Exists(metaPath))
                {
                    continue;
                }
                using (StreamReader file = File.OpenText(metaPath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject metaData = (JObject)JToken.ReadFrom(reader);
                    if (metaData.ContainsKey("patientId"))
                    {
                        string patientId = metaData["patientId"].ToString();
                        if (!patients.ContainsKey(patientId))
                        {
                            patients.Add(new JProperty(patientId,
                                new JObject(
                                    new JProperty("id", patientId),
                                    new JProperty("selected", false),
                                    new JProperty("history", new JArray())
                                )
                            ));
                        }
                        (patients[patientId]["history"] as JArray).Add(metaData);
                    }
                }
            }
            return patients;
        }

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            currentPosition = e.Position;
            JObject message =
                new JObject(
                        new JProperty("from", "controller"),
                        new JProperty("to", "all"),
                        new JProperty("action", "location"),
                        new JProperty("value",
                            new JObject(
                                new JProperty("lat", e.Position.Location.Latitude),
                                new JProperty("lng", e.Position.Location.Longitude)
                            )
                        )
                );

             SendMessage(message.ToString());
        }
    }
}
