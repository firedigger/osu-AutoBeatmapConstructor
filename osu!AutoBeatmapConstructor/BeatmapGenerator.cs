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
            mapContext = null;
        }

        public Beatmap generateBeatmap(PatternConfiguration config)
        {
            addPatterns(config.patterns);
            applyBeatmapStats(config.beatmapStats);
            return generatedMap;
        }

        private void applyBeatmapStats(BeatmapStats beatmapStats)
        {
            generatedMap.CircleSize = beatmapStats.CS;
            generatedMap.ApproachRate = beatmapStats.AR;
            generatedMap.OverallDifficulty = beatmapStats.OD;
            generatedMap.HPDrainRate = beatmapStats.HP;
        }

        private void addPatterns(IEnumerable<ConfiguredPattern> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.type == PatternType.Break)
                {
                    BreakEvent b = new BreakEvent();
                    b.StartTime = (int)mapContext.Offset;
                    generatedMap.HitObjects.AddRange(pattern.generatePattern(mapContext));
                    b.EndTime = (int)mapContext.Offset;
                    addBreak(b);
                }
                else
                {
                    generatedMap.HitObjects.AddRange(pattern.generatePattern(mapContext));
                }
            }

            foreach(var c in generatedMap.HitObjects)
            {
                c.Location.X = (int)c.Location.X;
                c.Location.Y = (int)c.Location.Y;
            }
        }

        public void addBreak(BreakEvent b)
        {
            generatedMap.Events.Add(b);
        }
    }
}
