using Leap;
using NeuroExplorer.LogWriter;
using NeuroExplorer.WebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NeuroExplorer.Connectors.LeapMotion
{
    class LeapMotionConnector : IWebSocketPropagator
    {
        private Controller controller;
        private List<DeviceData> devices;
        private WebSocketConnector webSocketConnector;
        private string status;

        private readonly LogStreamer logStreamer = new LogStreamer();
        private readonly string logStreamerFilename = "leap.jsonl";

        public LeapMotionConnector()
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

        public void Init()
        {
            SetStatus(Const.STATUS_READY);
        }

        public void SetWebSocket(WebSocketConnector ws)
        {
            webSocketConnector = ws;
            webSocketConnector.RegisterEndpoint("/leap", new List<string>()
            {
                "{\"serviceVersion\":\"3.2.1 + 45911\",\"version\":6}",
                "{\"event\":{\"state\":{\"attached\":true}}}"
            });
        }

        public void Connect()
        {
            controller = new Controller
            {
                EventContext = SynchronizationContext.Current,
            };

            devices = new List<DeviceData>();
            foreach (Leap.Device device in controller.Devices)
            {
                devices.Add(new DeviceData
                {
                    Baseline = device.Baseline,
                    HorizontalViewAngle = device.HorizontalViewAngle,
                    IsLightingBad = device.IsLightingBad,
                    IsSmudged = device.IsSmudged,
                    IsStreaming = device.IsStreaming,
                    Range = device.Range,
                    SerialNumber = device.SerialNumber,
                    VerticalViewAngle = device.VerticalViewAngle
                });
            }

            controller.FrameReady += NewFrameHandler;
            SetStatus(Const.STATUS_CONNECTED);
        }

        public void Configure()
        {
            System.Windows.MessageBox.Show("For configuration, use Leap software", "Leap Motion Configuration", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public void Disconnect()
        {
            if (controller != null)
            {
                controller.StopConnection();
                controller.Dispose();
            }
            SetStatus(Const.STATUS_DISCONNECTED);
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopRecording();
            Disconnect();
        }

        void NewFrameHandler(object sender, FrameEventArgs eventArgs)
        {
            Leap.Frame frame = eventArgs.frame;

            List<Hand> hands = new List<Hand>();
            List<Gesture> gestures = new List<Gesture>();
            List<Pointable> pointables = new List<Pointable>();
            Interaction interaction = new Interaction
            {
                Center = LeapVectorToList(frame.InteractionBox.Center),
                Size = LeapVectorToList(frame.InteractionBox.Size),
            };

            foreach (Leap.Hand leapHand in frame.Hands)
            {

                Vector handXBasis = leapHand.PalmNormal.Cross(leapHand.Direction).Normalized;
                Vector handYBasis = -leapHand.PalmNormal;
                Vector handZBasis = -leapHand.Direction;
                Vector handOrigin = leapHand.PalmPosition;

                // Matrix handTransform = new Matrix(handXBasis, handYBasis, handZBasis, handOrigin);
                // handTransform = handTransform.RigidInverse();

                hands.Add(new Hand
                {
                    ArmBasis = LeapTransformToList(leapHand.Arm.Basis),
                    ArmWidth = leapHand.Arm.Width,
                    Confidence = leapHand.Confidence,
                    Direction = LeapVectorToList(leapHand.Direction),
                    Elbow = LeapVectorToList(leapHand.Arm.ElbowPosition),
                    GrabAngle = leapHand.GrabAngle,
                    GrabStrength = leapHand.GrabStrength,
                    Id = leapHand.Id,
                    PalmNormal = LeapVectorToList(leapHand.PalmNormal),
                    PalmPosition = LeapVectorToList(leapHand.PalmPosition),
                    PalmVelocity = LeapVectorToList(leapHand.PalmVelocity),
                    PalmWidth = leapHand.PalmWidth,
                    PinchDistance = leapHand.PinchDistance,
                    PinchStrength = leapHand.PinchStrength,
                    // R = LeapTransformToList(hand.Rotation),
                    // S = hand.ScaleFactor,
                    // SphereCenter,
                    // SphereRadius,
                    StabilizedPalmPosition = LeapVectorToList(leapHand.StabilizedPalmPosition),
                    // T = hand.translation,
                    TimeVisible = leapHand.TimeVisible,
                    Type = leapHand.IsLeft ? "left" : "right",
                    Wrist = LeapVectorToList(leapHand.WristPosition)
                });

                foreach (Leap.Finger leapFinger in leapHand.Fingers)
                {
                    List<IList<IList<double>>> bases = new List<IList<IList<double>>>();
                    IList<double> btipPosition = new List<double>();
                    IList<double> carpPosition = new List<double>();
                    IList<double> dipPosition = new List<double>();
                    IList<double> mcpPosition = new List<double>();
                    IList<double> pipPosition = new List<double>();
                    foreach (Bone leapBone in leapFinger.bones)
                    {
                        bases.Add(LeapTransformToList(leapBone.Basis));
                        switch ((int)leapBone.Type)
                        {
                            case 0:
                                carpPosition = LeapVectorToList(leapBone.PrevJoint);
                                break;
                            case 1:
                                mcpPosition = LeapVectorToList(leapBone.PrevJoint);
                                break;
                            case 2:
                                pipPosition = LeapVectorToList(leapBone.PrevJoint);
                                break;
                            case 3:
                                dipPosition = LeapVectorToList(leapBone.PrevJoint);
                                btipPosition = LeapVectorToList(leapBone.NextJoint);
                                break;
                        }

                    }

                    pointables.Add(new Pointable
                    {
                        Bases = bases,
                        BtipPosition = btipPosition,
                        CarpPosition = carpPosition,
                        DipPosition = dipPosition,
                        Direction = LeapVectorToList(leapFinger.Direction),
                        Extended = leapFinger.IsExtended,
                        HandId = leapFinger.HandId,
                        Id = leapFinger.Id,
                        Length = leapFinger.Length,
                        McpPosition = mcpPosition,
                        PipPosition = pipPosition,
                        StabilizedTipPosition = LeapVectorToList(leapFinger.StabilizedTipPosition),
                        TimeVisible = leapFinger.TimeVisible,
                        TipPosition = LeapVectorToList(leapFinger.TipPosition),
                        TipVelocity = LeapVectorToList(leapFinger.TipVelocity),
                        // Tool = false,
                        // TouchDistance = 1
                        // touchZone = "none"
                        Type = (int)leapFinger.Type,
                        Width = leapFinger.Width,
                    });
                }

            }

            TrackingData trackingData = new TrackingData
            {
                CurrentFramesPerSecond = frame.CurrentFramesPerSecond,
                Devices = devices,
                Gestures = gestures,
                Hands = hands,
                Id = frame.Id,
                InteractionBox = interaction,
                Pointables = pointables,
                // R,
                // S,
                // T
                Timestamp = frame.Timestamp,
                TS = Stopwatch.GetTimestamp()
            };

            string stringMessage = JsonConvert.SerializeObject(trackingData).ToString();
            logStreamer.Write(stringMessage);

            if (webSocketConnector != null)
            {
                webSocketConnector.Propagate("/leap", stringMessage);
            }
        }

        IList<double> LeapVectorToList(Vector vector)
        {
            return new List<double>{
                    vector.x,
                    vector.y,
                    vector.z
                };
        }

        IList<IList<double>> LeapTransformToList(LeapTransform transform)
        {
            Vector xBasis = transform.xBasis;
            Vector yBasis = transform.yBasis;
            Vector zBasis = transform.zBasis;
            return new List<IList<double>>
            {
                new List<double>{
                    xBasis.x,
                    xBasis.y,
                    xBasis.z
                },
                new List<double>{
                    yBasis.x,
                    yBasis.y,
                    yBasis.z
                },
                new List<double>{
                    zBasis.x,
                    zBasis.y,
                    zBasis.z
                }
            };
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

