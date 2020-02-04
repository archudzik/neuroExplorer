using Newtonsoft.Json;
using System;
using System.Windows.Media.Media3D;

namespace Elliatab.Leap
{
    public abstract class TrackableEntity
    {
        /// <summary>
        /// A unique ID assigned to this object, whose value remains the same across consecutive frames while the tracked object remains visible. 
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("r")]
        protected float[][] RotationMatrixArray
        {
            set
            {
                this.Rotation = new Matrix3D(
                    value[0][0], value[0][1], value[0][2], 0,
                    value[1][0], value[1][1], value[1][2], 0,
                    value[2][0], value[2][1], value[2][2], 0,
                        0, 0, 0, 1);
            }
        }

        [JsonProperty("s")]
        protected float ScaleFactor { get; set; }

        [JsonProperty("t")]
        protected float[] Translation { get; set; }

        public Matrix3D Rotation { get; private set; }
    }
}
