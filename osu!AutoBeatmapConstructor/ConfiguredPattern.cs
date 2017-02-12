using BMAPI.v1.HitObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace osu_AutoBeatmapConstructor
{
    [XmlInclude(typeof(ConfiguredBreak))]
    [XmlInclude(typeof(ConfiguredDoubleJumps))]
    [XmlInclude(typeof(ConfiguredPolygons))]
    [XmlInclude(typeof(ConfiguredStreams))]
    [XmlInclude(typeof(ConfiguredRandomJumps))]
    public abstract class ConfiguredPattern
    {
        public PatternType type;
        public bool end;
        
        public ConfiguredPattern(PatternType type, bool end)
        {
            this.type = type;
            this.end = end;
        }

        public ConfiguredPattern()
        {

        }

        public abstract override string ToString();

        public abstract List<CircleObject> generatePattern(MapContextAwareness mapContext);
    }
}
