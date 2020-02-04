using Newtonsoft.Json;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Elliatab.Leap
{
    /// <summary>
    /// The Pointable class reports the physical characteristics of a detected finger or tool.
    /// </summary>
    public class Pointable
    {
        #region private properties used for deserialization
        [JsonProperty("direction")]
        private float[] DirectionArray
        {
            set { this.Direction = new Vector3D(value[0], value[1], value[2]); }
        }

        [JsonProperty("handId")]
        private int HandId { get; set; }

        [JsonProperty("tipPosition")]
        private float[] TipPositionArray
        {
            set { this.TipPosition = new Vector3D(value[0], value[1], value[2]); }
        }

        [JsonProperty("tipVelocity")]
        private float[] TipVelocityArray
        {
            set { this.TipVelocity = new Vector3D(value[0], value[1], value[2]); }
        }

        [JsonProperty("tool")]
        private bool Tool
        {
            set 
            { 
                this.IsTool = value;
                this.IsFinger = !value;
            }
        }
        #endregion

        /// <summary>
        /// The direction in which this finger or tool is pointing. 
        /// </summary>
        public Vector3D Direction { get; private set; }

        /// <summary>
        /// The Frame associated with this Pointable object. 
        /// </summary>
        public Frame Frame { get; internal set; }

        /// <summary>
        /// The Hand associated with this finger or tool. 
        /// </summary>
        public Hand Hand
        {
            get
            {
                return (from h in this.Frame.Hands where h.Id == this.HandId select h).FirstOrDefault();
            }
        }

        /// <summary>
        /// A unique ID assigned to this Pointable object, whose value remains the same across consecutive frames while the tracked finger or tool remains visible. 
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; private set; }

        /// <summary>
        /// Whether or not the Pointable is believed to be a finger. 
        /// </summary>
        public bool IsFinger { get; private set; }

        /// <summary>
        /// Whether or not the Pointable is believed to be a tool. 
        /// </summary>
        public bool IsTool { get; private set; }
        
        /// <summary>
        /// The estimated length of the finger or tool in millimeters. 
        /// </summary>
        [JsonProperty("length")]
        public float Length { get; private set; }

        /// <summary>
        /// The tip position in millimeters from the Leap origin. 
        /// </summary>
        public Vector3D TipPosition { get; private set; }

        /// <summary>
        /// The rate of change of the tip position in millimeters/second. 
        /// </summary>
        public Vector3D TipVelocity { get; private set; }
    }
}
