using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMAPI.v1.HitObjects;
using BMAPI;

namespace osu_AutoBeatmapConstructor
{
    [Serializable]
    public class ConfiguredStreams : ConfiguredPattern
    {
        public int points;
        public int number;
        public int spacing;
        public int shift;

        public ConfiguredStreams(int points, int number, int spacing, int shift, bool end) : base(PatternType.Streams, end)
        {
            this.points = points;
            this.number = number;
            this.spacing = spacing;
            this.shift = shift;
        }

        public ConfiguredStreams()
        {

        }

        public override string ToString()
        {
            string description;

            if (end)
            {
                description = points + "-Streams till end";
            }
            else
            {
                description = number + " " + points + "-Streams";
            }

            return description;
        }

        public List<CircleObject> generateStreams(MapContextAwareness mapContext, int points, int number, int spacing, int shift)
        {
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm / points) - 1;

                number = n;
            }

            List<CircleObject> result = new List<CircleObject>();

            int standard_shift = (int)(spacing * 1.0 / points / Math.Sqrt(2));
            if (shift < standard_shift)
            {
                shift = standard_shift;
            }

            int shiftx = shift;
            int shifty = shift;

            double angle = 0;
            bool flag = false;
            for (int i = 0; i < number; ++i)
            {
                Point2 from = new Point2(mapContext.X, mapContext.Y);

                Point2 to = new Point2(from.X + spacing * (float)Math.Cos(angle), from.Y + spacing * (float)Math.Sin(angle));
                if (!PatternGenerator.checkCoordinateLimits(to))
                {
                    flag = true;
                }
                while (flag)
                {
                    angle = Utils.rng.NextDouble() * Math.PI * 2;

                    to = new Point2(from.X + spacing * (float)Math.Cos(angle), from.Y + spacing * (float)Math.Sin(angle));

                    if (PatternGenerator.checkCoordinateLimits(to))
                    {
                        flag = false;
                    }
                }

                Point2 mid = new Point2((to.X + from.X) / 2, (to.Y + from.Y) / 2);

                double shift_delta = 0.2 + 0.4 * Utils.rng.NextDouble();
                if (Utils.rng.NextDouble() > 0.5)
                    shift_delta = -shift_delta;

                double delta = shift_delta * spacing;

                mid.X += (float)(delta * Math.Sin(angle));
                mid.Y += (float)(delta * Math.Cos(angle));

                var pattern = PatternGenerator.stream(points, from, to, mid);
                pattern[0].Type |= BMAPI.v1.HitObjectType.NewCombo;
                foreach (var obj in pattern)
                {
                    obj.StartTime = (int)mapContext.offset;
                    mapContext.offset += mapContext.bpm / 2;
                }

                result.AddRange(pattern);

                if (!PatternGenerator.checkCoordinateLimits(to.X + shiftx, to.Y + shifty))
                {
                    Point2 next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, shift);
                    shiftx = (int)(next.X - mapContext.X);
                    shifty = (int)(next.Y - mapContext.Y);

                    double proportion = Math.Sqrt(Utils.sqr(shiftx) + Utils.sqr(shifty) / 2 / Utils.sqr(shift));

                    shiftx = (int)(shiftx / proportion);
                    shifty = (int)(shifty / proportion);

                }

                mapContext.X = (int)to.X + shiftx;
                mapContext.Y = (int)to.Y + shifty;
            }

            return result;
        }

        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                double endOffset = context.endOffset;
                double currOffset = context.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm * 2);

                number = n / points;

            }
            notes.AddRange(generateStreams(context,points, number, spacing, shift));

            return notes;
        }
    }
}
