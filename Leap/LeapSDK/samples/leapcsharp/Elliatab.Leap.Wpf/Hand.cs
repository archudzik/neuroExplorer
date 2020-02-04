using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Elliatab.Leap
{
    /// <summary>
    /// The Hand class reports the physical characteristics of a detected hand.
    /// </summary>
    public sealed class Hand : TrackableEntity
    {
        #region private properties used for deserialization
        [JsonProperty("direction")]
        private float[] DirectionArray
        {
            set { this.Direction = new Vector3D(value[0], value[1], value[2]);}
        }

        [JsonProperty("palmNormal")]
        private float[] PalmNormalArray
        {
            set { this.PalmNormal = new Vector3D(value[0], value[1], value[2]); }
        }

        [JsonProperty("palmPosition")]
        private float[] PalmPositionArray
        {
            set { this.PalmPosition = new Point3D(value[0], value[1], value[2]); }
        }
        
        [JsonProperty("palmVelocity")]
        private float[] PalmVelocityArray
        {
            set { this.PalmVelocity = new Vector3D(value[0], value[1], value[2]); }
        }

        [JsonProperty("sphereCenter")]
        private float[] SphereCenterArray
        {
            set { this.SphereCenter = new Point3D(value[0], value[1], value[2]); }
        }
        #endregion
        
        /// <summary>
        /// The direction from the palm position toward the fingers. 
        /// </summary>
        public Vector3D Direction { get; private set; }

        /// <summary>
        /// The list of Finger objects detected in this frame that are attached to this hand, given in arbitrary order. 
        /// </summary>
        public ReadOnlyCollection<Pointable> Fingers
        {
            get
            {
                return (from f in this.Frame.Fingers 
                             where f.Hand != null && f.Hand.Id == this.Id 
                             select f).ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// The Frame associated with this Hand. 
        /// </summary>
        public Frame Frame { get; internal set; }

        /// <summary>
        /// The normal vector to the palm. 
        /// </summary>
        public Vector3D PalmNormal { get; private set; }

        /// <summary>
        /// The center position of the palm in millimeters from the Leap origin. 
        /// </summary>
        public Point3D PalmPosition { get; private set; }

        /// <summary>
        /// The rate of change of the palm position in millimeters/second. 
        /// </summary>
        public Vector3D PalmVelocity { get; private set; }
        
        /// <summary>
        /// The list of Pointable objects (fingers and tools) detected in this frame that are associated with this hand, given in arbitrary order. 
        /// </summary>
        public ReadOnlyCollection<Pointable> Pointables
        {
            get { return (from p in this.Frame.Pointables where p.Hand.Id == this.Id select p).ToList().AsReadOnly(); }
        }
        
        /// <summary>
        /// The center of a sphere fit to the curvature of this hand. 
        /// </summary>
        public Point3D SphereCenter { get; private set; }

        /// <summary>
        /// The radius of a sphere fit to the curvature of this hand. 
        /// </summary>
        [JsonProperty("sphereRadius")]
        public float SphereRadius { get; private set; }

        /// <summary>
        /// The list of Tool objects detected in this frame that are held by this hand, given in arbitrary order. 
        /// </summary>
        public ReadOnlyCollection<Pointable> Tools
        {
            get { return (from t in this.Frame.Tools
                          where t.Hand != null && t.Hand.Id == this.Id 
                          select t).ToList().AsReadOnly(); }
        }
    }
}
