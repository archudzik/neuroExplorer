using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroExplorer.Connectors.OpenFace
{
    public class FpsTracker
    {
        public TimeSpan HistoryLength { get; set; }
        public DateTime CurrentTime { get { return startTime + sw.Elapsed; } }
        static DateTime startTime;
        static Stopwatch sw = new Stopwatch();

        public FpsTracker()
        {
            startTime = DateTime.Now;
            sw.Start();
            HistoryLength = TimeSpan.FromSeconds(2);
        }

        private Queue<DateTime> frameTimes = new Queue<DateTime>();

        private void DiscardOldFrames()
        {
            while (frameTimes.Count > 0 && (CurrentTime - frameTimes.Peek()) > HistoryLength)
                frameTimes.Dequeue();
        }

        public void AddFrame()
        {
            frameTimes.Enqueue(CurrentTime);
            DiscardOldFrames();
        }

        public double GetFPS()
        {
            DiscardOldFrames();

            if (frameTimes.Count == 0)
                return 0;

            return frameTimes.Count / (CurrentTime - frameTimes.Peek()).TotalSeconds;
        }
    }
}
