using BMAPI.v1.HitObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public abstract class ConfiguredPattern
    {
        public PatternType type;
        public bool end;
        
        public ConfiguredPattern(PatternType type, bool end)
        {
            this.type = type;
            this.end = end;
        }

        public abstract override string ToString();

        public abstract List<CircleObject> generatePattern(MapContextAwareness mapContext);
    }
}
