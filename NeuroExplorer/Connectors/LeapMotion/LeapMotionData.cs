using Newtonsoft.Json;
using System.Collections.Generic;

namespace NeuroExplorer.Connectors.LeapMotion
{
    public class TrackingData
    {
        [JsonProperty(PropertyName = "ts")]
        public long TS { get; set; }
        [JsonProperty(PropertyName = "id")]
        public double Id { get; set; }
        [JsonProperty(PropertyName = "currentFrameRate")]
        public double CurrentFramesPerSecond { get; set; }
        [JsonProperty(PropertyName = "r")]
        public IList<IList<double>> R { get; set; }
        [JsonProperty(PropertyName = "s")]
        public double S { get; set; }
        [JsonProperty(PropertyName = "t")]
        public IList<double> T { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty(PropertyName = "devices")]
        public IList<DeviceData> Devices { get; set; }
        [JsonProperty(PropertyName = "gestures")]
        public IList<Gesture> Gestures { get; set; }
        [JsonProperty(PropertyName = "hands")]
        public IList<Hand> Hands { get; set; }
        [JsonProperty(PropertyName = "interactionBox")]
        public Interaction InteractionBox { get; set; }
        [JsonProperty(PropertyName = "pointables")]
        public IList<Pointable> Pointables { get; set; }
    }

    public class DeviceData
    {
        [JsonProperty(PropertyName = "baseline")]
        public double Baseline { get; set; }
        [JsonProperty(PropertyName = "currentFrameRate")]
        public double CurrentFrameRate { get; set; }
        [JsonProperty(PropertyName = "horizontalViewAngle")]
        public double HorizontalViewAngle { get; set; }
        [JsonProperty(PropertyName = "isLightingBade")]
        public bool IsLightingBad { get; set; }
        [JsonProperty(PropertyName = "isSmudged")]
        public bool IsSmudged { get; set; }
        [JsonProperty(PropertyName = "isStreaming")]
        public bool IsStreaming { get; set; }
        [JsonProperty(PropertyName = "range")]
        public double Range { get; set; }
        [JsonProperty(PropertyName = "serialNumber")]
        public string SerialNumber { get; set; }
        [JsonProperty(PropertyName = "verticalViewAngle")]
        public double VerticalViewAngle { get; set; }
    }

    public class Gesture
    {
        [JsonProperty(PropertyName = "center")]
        public IList<double?> Center { get; set; }
        [JsonProperty(PropertyName = "direction")]
        public IList<double?> Direction { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
        [JsonProperty(PropertyName = "handIDs")]
        public IList<int> HandIDs { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "normal")]
        public IList<double?> Normal { get; set; }
        [JsonProperty(PropertyName = "pintableIds")]
        public IList<int> PintableIds { get; set; }
        [JsonProperty(PropertyName = "position")]
        public IList<double?> Position { get; set; }
        [JsonProperty(PropertyName = "progress")]
        public double? Progress { get; set; }
        [JsonProperty(PropertyName = "radius")]
        public double? Radius { get; set; }
        [JsonProperty(PropertyName = "speed")]
        public double? Speed { get; set; }
        [JsonProperty(PropertyName = "startPosition")]
        public IList<double?> StartPosition { get; set; }
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class Hand
    {
        [JsonProperty(PropertyName = "armBasis")]
        public IList<IList<double>> ArmBasis { get; set; }
        [JsonProperty(PropertyName = "armWidth")]
        public float ArmWidth { get; set; }
        [JsonProperty(PropertyName = "confidence")]
        public float Confidence { get; set; }
        [JsonProperty(PropertyName = "direction")]
        public IList<double> Direction { get; set; }
        [JsonProperty(PropertyName = "elbow")]
        public IList<double> Elbow { get; set; }
        [JsonProperty(PropertyName = "grabStrength")]
        public double GrabStrength { get; set; }
        [JsonProperty(PropertyName = "grabAngle")]
        public double GrabAngle { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "palmNormal")]
        public IList<double> PalmNormal { get; set; }
        [JsonProperty(PropertyName = "palmPosition")]
        public IList<double> PalmPosition { get; set; }
        [JsonProperty(PropertyName = "palmVelocity")]
        public IList<double> PalmVelocity { get; set; }
        [JsonProperty(PropertyName = "palmWidth")]
        public double PalmWidth { get; set; }
        [JsonProperty(PropertyName = "pinchDistance")]
        public double PinchDistance { get; set; }
        [JsonProperty(PropertyName = "pinchStrength")]
        public double PinchStrength { get; set; }
        [JsonProperty(PropertyName = "r")]
        public IList<IList<double>> R { get; set; }
        [JsonProperty(PropertyName = "s")]
        public double S { get; set; }
        [JsonProperty(PropertyName = "sphereCenter")]
        public IList<double> SphereCenter { get; set; }
        [JsonProperty(PropertyName = "sphereRadius")]
        public double SphereRadius { get; set; }
        [JsonProperty(PropertyName = "stabilizedPalmPosition")]
        public IList<double> StabilizedPalmPosition { get; set; }
        [JsonProperty(PropertyName = "t")]
        public IList<double> T { get; set; }
        [JsonProperty(PropertyName = "timeVisible")]
        public double TimeVisible { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "wrist")]
        public IList<double> Wrist { get; set; }
    }

    public class Interaction
    {
        [JsonProperty(PropertyName = "center")]
        public IList<double> Center { get; set; }
        [JsonProperty(PropertyName = "size")]
        public IList<double> Size { get; set; }
    }

    public class Pointable
    {
        [JsonProperty(PropertyName = "bases")]
        public IList<IList<IList<double>>> Bases { get; set; }
        [JsonProperty(PropertyName = "btipPosition")]
        public IList<double> BtipPosition { get; set; }
        [JsonProperty(PropertyName = "carpPosition")]
        public IList<double> CarpPosition { get; set; }
        [JsonProperty(PropertyName = "dipPosition")]
        public IList<double> DipPosition { get; set; }
        [JsonProperty(PropertyName = "direction")]
        public IList<double> Direction { get; set; }
        [JsonProperty(PropertyName = "extended")]
        public bool Extended { get; set; }
        [JsonProperty(PropertyName = "handId")]
        public int HandId { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "length")]
        public double Length { get; set; }
        [JsonProperty(PropertyName = "mcpPosition")]
        public IList<double> McpPosition { get; set; }
        [JsonProperty(PropertyName = "pipPosition")]
        public IList<double> PipPosition { get; set; }
        [JsonProperty(PropertyName = "stabilizedTipPosition")]
        public IList<double> StabilizedTipPosition { get; set; }
        [JsonProperty(PropertyName = "timeVisible")]
        public double TimeVisible { get; set; }
        [JsonProperty(PropertyName = "tipPosition")]
        public IList<double> TipPosition { get; set; }
        [JsonProperty(PropertyName = "tipVelocity")]
        public IList<double> TipVelocity { get; set; }
        [JsonProperty(PropertyName = "tool")]
        public bool Tool { get; set; }
        [JsonProperty(PropertyName = "touchDistance")]
        public double TouchDistance { get; set; }
        [JsonProperty(PropertyName = "touchZone")]
        public string TouchZone { get; set; }
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }
        [JsonProperty(PropertyName = "width")]
        public double Width { get; set; }
    }
}
