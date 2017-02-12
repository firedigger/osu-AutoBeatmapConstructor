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

        public double offset;
        public double bpm;

        public double beginOffset;
        public double endOffset;

        public object Clone()
        {
            MapContextAwareness a = new MapContextAwareness();

            a.X = X;
            a.Y = Y;

            a.offset = offset;
            a.bpm = bpm;

            a.beginOffset = beginOffset;
            a.endOffset = endOffset;

            return a;
        }
    }
}
