using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GoIOdotNET;
using VSTCoreDefsdotNET;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace NeuroExplorer.Connectors.Dynamometer
{
    class VernierConnector : IWebSocketPropagator
    {
        private WebSocketConnector webSocketConnector;
        private string status;
        private bool isDisposed = true;

        private IntPtr sensorHandle;
        private bool sensorCalibrated;
        private double calibrationBias;
        private volatile bool threadRunning = true;
        private Thread processingThread;
        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "dynamometer.jsonl";

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/dynamometer", new List<string>());
        }

        public void Init()
        {
            SetStatus(Const.STATUS_READY);
        }

        public void Connect()
        {
            processingThread = new Thread(() => ReadValues())
            {
                Name = "Vernier processing"
            };
            processingThread.Start();
        }

        private void ReadValues()
        {
            isDisposed = false;
            threadRunning = true;
            sensorCalibrated = false;
            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            IntPtr initResult = GoIO.Init();
            if (initResult.ToInt32() != 0)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            int numGoFound = GoIO.UpdateListOfAvailableDevices(
                VST_USB_defs.VENDOR_ID, VST_USB_defs.PRODUCT_ID_GO_LINK);

            StringBuilder deviceName = new StringBuilder(GoIO.MAX_SIZE_SENSOR_NAME);

            int status = GoIO.GetNthAvailableDeviceName(deviceName, deviceName.Capacity,
                VST_USB_defs.VENDOR_ID, VST_USB_defs.PRODUCT_ID_GO_LINK, 0);

            if (status != 0)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            sensorHandle = GoIO.Sensor_Open(deviceName.ToString(), VST_USB_defs.VENDOR_ID,
                VST_USB_defs.PRODUCT_ID_GO_LINK, 0);

            if (sensorHandle == IntPtr.Zero)
            {
                SetStatus(Const.STATUS_UNAVAILABLE);
                return;
            }

            GoIOSetLedStateParams ledParams = new GoIOSetLedStateParams
            {
                brightness = GoIOSetLedStateParams.LED_BRIGHTNESS_MAX,
                color = GoIOSetLedStateParams.LED_COLOR_GREEN
            };

            int ledResult = GoIO.Sensor_SendCmdAndGetResponse2(sensorHandle,
                GoIO_ParmBlk.CMD_ID_SET_LED_STATE, ledParams, GoIO.TIMEOUT_MS_DEFAULT);

            SetStatus(Const.STATUS_CONNECTED);

            GoIO.Sensor_ClearIO(sensorHandle);
            int result = GoIO.Sensor_SendCmdAndGetResponse4(sensorHandle, GoIO_ParmBlk.CMD_ID_START_MEASUREMENTS, GoIO.TIMEOUT_MS_DEFAULT);
            if (result != 0)
            {
                SetStatus(Const.STATUS_DISCONNECTED);
            }

            while (threadRunning)
            {
                int[] raw = new int[100];
                int numMeasurements = GoIO.Sensor_ReadRawMeasurements(sensorHandle, raw, (uint)raw.Length);
                if(numMeasurements > 0)
                {
                    for (int i = 0; i < numMeasurements; i++)
                    {
                        double currentValue = Convert.ToDouble((float)GoIO.Sensor_CalibrateData(sensorHandle, GoIO.Sensor_ConvertToVoltage(sensorHandle, raw[i]))); // N (Newton)
                        currentValue *= 0.101971621; // Kgf (Kilogram force)
                        if (!sensorCalibrated)
                        {
                            calibrationBias = -currentValue;
                            sensorCalibrated = true;
                        }
                        currentValue += calibrationBias;

                        JObject message = new JObject(
                            new JProperty("ts", Stopwatch.GetTimestamp()),
                            new JProperty("value", currentValue)
                        );

                        string stringMessage = message.ToString(Formatting.None);
                        logStreamer.Write(stringMessage);
                        webSocketConnector.Propagate("/dynamometer", stringMessage);
                    }
                }
                Thread.Sleep(50);
            }
            GoIO.Sensor_SendCmdAndGetResponse4(sensorHandle, GoIO_ParmBlk.CMD_ID_STOP_MEASUREMENTS, GoIO.TIMEOUT_MS_DEFAULT);

            ledParams = new GoIOSetLedStateParams
            {
                brightness = GoIOSetLedStateParams.LED_BRIGHTNESS_ORANGE,
                color = GoIOSetLedStateParams.LED_COLOR_ORANGE
            };
            GoIO.Sensor_SendCmdAndGetResponse2(sensorHandle,
                GoIO_ParmBlk.CMD_ID_SET_LED_STATE, ledParams, GoIO.TIMEOUT_MS_DEFAULT);
            GoIO.Sensor_Close(sensorHandle);
            GoIO.Uninit();
        }

        public void Configure()
        {
            sensorCalibrated = false;
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

        public void StartRecording(string outputFolder)
        {
            logStreamer.Init(Path.Combine(outputFolder, logStreamerFilename));
        }

        public void StopRecording()
        {
            logStreamer.Terminate();
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }
    }
}
