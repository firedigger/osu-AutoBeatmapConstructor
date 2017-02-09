using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMAPI.v1.HitObjects;

namespace osu_AutoBeatmapConstructor
{
    public class ConfiguredBreak : ConfiguredPattern
    {
        private int seconds;

        public ConfiguredBreak(int seconds) : base(PatternType.Break, false)
        {
            this.seconds = seconds;
        }

        public override List<CircleObject> generatePattern(MapContextAwareness mapContext)
        {
            int periods = (int)(seconds / mapContext.bpm);
            mapContext.offset += mapContext.bpm * periods;

            return new List<CircleObject>();
        }

        public override string ToString()
        {
            return seconds + " seconds break";
        }
    }
}
