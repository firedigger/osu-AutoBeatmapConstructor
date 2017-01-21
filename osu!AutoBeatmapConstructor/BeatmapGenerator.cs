using BMAPI.v1;
using BMAPI.v1.Events;
using BMAPI.v1.HitObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public class BeatmapGenerator
    {
        private Beatmap baseMap;
        public Beatmap generatedMap;
        public MapContextAwareness mapContext;
        public PatternsGenerator patternGenerator;

        public BeatmapGenerator(Beatmap baseMap)
        {
            this.baseMap = baseMap;
            generatedMap = new Beatmap(baseMap);
            mapContext = new MapContextAwareness();
            patternGenerator = new PatternsGenerator(mapContext);
        }

        public Beatmap generateBeatmap()
        {
            return generatedMap;
        }

        public void addPattern(List<CircleObject> pattern)
        {
            generatedMap.HitObjects.AddRange(pattern);
        }

        public void addBreak(BreakEvent b)
        {
            generatedMap.Events.Add(b);
        }
    }
}
