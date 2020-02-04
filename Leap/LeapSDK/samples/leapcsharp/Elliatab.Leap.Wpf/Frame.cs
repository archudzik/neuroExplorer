using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Elliatab.Leap
{
    /// <summary>
    /// The Frame class represents a set of hand and finger tracking data detected in a single frame.
    /// The Leap detects hands, fingers and tools within the tracking area, reporting their positions, orientations and motions in frames at the Leap frame rate.
    /// Access Frame objects through an instance of a Leap Controller. Implement a Listener subclass to receive a callback event when a new Frame is available.
    /// </summary>
    public sealed class Frame : TrackableEntity
    {
        #region private members
        private List<Hand> hands;
        private List<Pointable> pointables;

        [JsonProperty("hands")]
        private Hand[] HandsArray
        {
            set
            {
                this.hands = new List<Hand>(value.Length);

                foreach (var hand in value)
                {
                    hand.Frame = this;
                    this.hands.Add(hand);
                }    
            }
        }

        [JsonProperty("pointables")]
        private Pointable[] PointablesArray
        {
            set
            {
                this.pointables = new List<Pointable>(value.Length);

                foreach (var pointable in value)
                {
                    pointable.Frame = this;
                    this.pointables.Add(pointable);
                }
            }
        }
        #endregion

        private Frame()
        {
        }

        /// <summary>
        /// The list of Finger objects detected in this frame, given in arbitrary order.    
        /// </summary>
        /// <remarks>The list can be empty if no fingers are detected.</remarks>
        public ReadOnlyCollection<Pointable> Fingers
        {
            get
            {
                return (from f in this.pointables where f.IsFinger select f).ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// The list of Hand objects detected in this frame, given in arbitrary order.
        /// </summary>
        /// <remarks>The list can be empty if no fingers are detected.</remarks>
        public ReadOnlyCollection<Hand> Hands
        {
            get
            {
                return hands.AsReadOnly();
            }
        }

         /// <summary>
        /// The list of <see cref="Pointable"/> objects (fingers and tools) detected in this frame, given in arbitrary order.
        /// </summary>
        /// <remarks>The list can be empty if no fingers are detected.</remarks>
        public ReadOnlyCollection<Pointable> Pointables
        {
            get
            {
                return pointables.AsReadOnly();
            }
        }

        /// <summary>
        /// The frame capture time in microseconds elapsed since the Leap started.
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; private set; }

        /// <summary>
        /// The list of Tool objects detected in this frame, given in arbitrary order.
        /// The list can be empty if no tools are detected.
        /// </summary>
        public ReadOnlyCollection<Pointable> Tools
        {
            get
            {
                return (from f in this.pointables where f.IsTool select f).ToList().AsReadOnly();
            }
        }

        public static Frame DeserializeFromJson(string value)
        {
            var frame = JsonConvert.DeserializeObject<Frame>(value);

            return frame;
        }
    }
}
