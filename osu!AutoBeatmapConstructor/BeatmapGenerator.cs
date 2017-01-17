using BMAPI.v1;
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

        private void spamSquares()
        {
            TimingPoint current = generatedMap.TimingPoints[0];

            double bpm = current.BpmDelay / 2;
            double currentOffset = current.Time;

            double X = 256;
            double Y = 192;
            double angle = 0;
            int points = 5;
            for (int i = 0; i < 4; ++i)
            {
                var square = PatternGenerator.polygon(points,new BMAPI.Point2((float)X, (float)Y),100,angle);

                X += 3;
                Y += 3;
                angle += Math.PI / 12;

                square[0].Type |= HitObjectType.NewCombo;

                foreach(var note in square)
                {
                    note.StartTime = (int)Math.Round(currentOffset);
                    currentOffset += bpm;
                }

                generatedMap.HitObjects.AddRange(square);
            }

            var stream = PatternGenerator.streamSquare(new BMAPI.Point2((float)X, (float)Y),100,16);
            foreach (var note in stream)
            {
                note.StartTime = (int)Math.Round(currentOffset);
                currentOffset += bpm / 2;
            }

            generatedMap.HitObjects.AddRange(stream);

        }

        public Beatmap generateBeatmap()
        {
            return generatedMap;
        }

        public void addPattern(ConfiguredPattern pattern)
        {
            generatedMap.HitObjects.AddRange(pattern.objects);
        }
    }
}
