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

        private void removeBreaks()
        {
            List<EventBase> events = new List<EventBase>();
            foreach (var ev in generatedMap.Events)
            {
                if (!(ev is BreakEvent))
                    events.Add(ev);
            }
        }

        public BeatmapGenerator(Beatmap baseMap)
        {
            this.baseMap = baseMap;
            generatedMap = new Beatmap(baseMap);
            removeBreaks();
            mapContext = new MapContextAwareness();
        }

        public Beatmap generateBeatmap()
        {
            return generatedMap;
        }

        public void addPatterns(IEnumerable<ConfiguredPattern> patterns)
        {
            foreach(var pattern in patterns)
            {
                if (pattern.type == PatternType.Break)
                {
                    BreakEvent b = new BreakEvent();
                    b.StartTime = (int)mapContext.offset;
                    generatedMap.HitObjects.AddRange(pattern.generatePattern(mapContext));
                    b.EndTime = (int)mapContext.offset;
                    addBreak(b);
                }
                else
                {
                    generatedMap.HitObjects.AddRange(pattern.generatePattern(mapContext));
                }
            }
        }

        public void addBreak(BreakEvent b)
        {
            generatedMap.Events.Add(b);
        }
    }
}
