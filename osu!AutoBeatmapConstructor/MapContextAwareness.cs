using BMAPI.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public class MapContextAwareness : ICloneable
    {
        public int X = 200;
        public int Y = 200;

        private double offset;
        public double bpm;

        private List<TimingPoint> timingPoints;
        private TimingPoint currentTimingPoint;
        private TimingPoint nextTimingPoint;

        public double beginOffset;
        public double endOffset;
        private double bpm_multiplier;

        public double Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
                checkForNewTiming();
            }
        }

        private void checkForNewTiming()
        {
            if (nextTimingPoint != null && nextTimingPoint.Time <= offset)
            {
                currentTimingPoint = nextTimingPoint;
                updateOffsetBpm();
                findNextTimingPoint();
            }
        }

        private void updateOffsetBpm()
        {
            bpm = currentTimingPoint.BpmDelay / 2.0 * bpm_multiplier;
            offset = Math.Ceiling((offset - currentTimingPoint.Time) / bpm) * bpm + currentTimingPoint.Time;
        }

        private void findNextTimingPoint()
        {
            var i = timingPoints.FindIndex((x) => !x.InheritsBPM && x.Time > currentTimingPoint.Time);

            if (i == -1)
                nextTimingPoint = null;
            else
                nextTimingPoint = timingPoints[i];
        }

        private void initTiming()
        {
            int i;
            for (i = timingPoints.Count - 1; i >= 0; --i)
            {
                if (!timingPoints[i].InheritsBPM && timingPoints[i].Time <= beginOffset)
                    break;
            }

            if (i == -1)
                throw new Exception("The map has no timing data");

            currentTimingPoint = timingPoints[i];
            updateOffsetBpm();

            findNextTimingPoint();    
        }

        /*public MapContextAwareness()
        {

        }*/

        public MapContextAwareness(double bpm_multiplier, double beginOffset, double endOffset, int X, int Y, List<TimingPoint> timingPoints)
        {
            this.bpm_multiplier = bpm_multiplier;
            this.beginOffset = beginOffset;
            this.endOffset = endOffset;
            offset = beginOffset;
            this.X = X;
            this.Y = Y;

            this.timingPoints = timingPoints;
            initTiming();
        }

        public object Clone()
        {
            MapContextAwareness a = new MapContextAwareness(bpm_multiplier, beginOffset, endOffset, X, Y, timingPoints);
            return a;
        }
    }
}
