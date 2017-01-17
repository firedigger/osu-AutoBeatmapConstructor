using BMAPI.v1.HitObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public class ConfiguredPattern
    {
        public List<CircleObject> objects;
        public string description;
        public bool end;

        public ConfiguredPattern(List<CircleObject> objects, string description)
        {
            this.objects = objects;
            this.description = description;
        }

        public override string ToString()
        {
            return description;
        }
    }
}
