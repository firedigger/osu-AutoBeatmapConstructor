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
        public int curviness;
        public int shift;

        public ConfiguredStreams(int points, int number, int spacing, int curviness, int shift, bool end) : base(PatternType.Streams, end)
        {
            this.points = points;
            this.number = number;
            this.spacing = spacing;
            this.shift = shift;
            this.curviness = curviness;
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

        private double generateMidDelta()
        {
            return (0.2 + 0.2 * Utils.rng.NextDouble()) * (1.0 * curviness / 100);
        }

        public List<CircleObject> generateStreams(MapContextAwareness mapContext, int points, int number, int spacing, int shift)
        {
            //spacing is the distance between two notes in the stream

            if (end)
            {
                //double endOffset = mapContext.endOffset;
                //double currOffset = mapContext.Offset;

                //int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm * 2 / points) - 1;

                number = (int)1e6;
            }

            List<CircleObject> result = new List<CircleObject>();

            int shiftx = shift;
            int shifty = shift;

            double angle = 0;
            bool flag = false;

            int distToNextPoint = spacing * (points - 1);

            for (int i = 0; i < number && mapContext.Offset < mapContext.endOffset; ++i)
            {
                Point2 from = new Point2(mapContext.X, mapContext.Y);
                double tmp_angle = angle;
                Point2 to = new Point2(from.X + distToNextPoint * (float)Math.Cos(angle), from.Y + distToNextPoint * (float)Math.Sin(angle));
                if (!PatternGenerator.checkCoordinateLimits(to))
                {
                    flag = true;
                }

                int tmp_spacing = distToNextPoint;
                for (int k = 0; flag; ++k)
                {
                    tmp_angle = Utils.rng.NextDouble() * Math.PI * 2;
                    while (Math.Abs(Math.Tan(angle) - Math.Tan(tmp_angle)) < 0.05)
                        tmp_angle = Utils.rng.NextDouble() * Math.PI * 2;

                    to = new Point2(from.X + tmp_spacing * (float)Math.Cos(tmp_angle), from.Y + tmp_spacing * (float)Math.Sin(tmp_angle));

                    if (PatternGenerator.checkCoordinateLimits(to))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (k > 1000)
                        {
                            to = pointToCenter(from, tmp_spacing);
                            tmp_spacing = (int)(tmp_spacing * 0.9);
                            k = 0;
                        }
                    }
                }

                double mid_delta = generateMidDelta();

                List<CircleObject> pattern = null;

                if (mid_delta > 0.1)
                {
                    double shift_delta = mid_delta * distToNextPoint;
                    to = binarySeachPoint(from, tmp_angle, distToNextPoint, shift_delta);

                    var mid = new Point2((to.X + from.X) / 2, (to.Y + from.Y) / 2);
                    mid.X += (float)(shift_delta * Math.Sin(tmp_angle));
                    mid.Y += (float)(shift_delta * Math.Cos(tmp_angle));
                    if (!PatternGenerator.checkCoordinateLimits(mid))
                    {
                        mid.X -= 2 * (float)(shift_delta * Math.Sin(tmp_angle));
                        mid.Y -= 2 * (float)(shift_delta * Math.Cos(tmp_angle));
                    }
                    pattern = PatternGenerator.stream(points, from, to, mid);
                }
                else
                {
                    pattern = PatternGenerator.stream(points, from, to);
                }
                pattern[0].Type |= BMAPI.v1.HitObjectType.NewCombo;
                foreach (var obj in pattern)
                {
                    obj.StartTime = (int)mapContext.Offset;
                    mapContext.Offset += mapContext.bpm / 2;
                }

                result.AddRange(pattern);

                double spacing_shift = spacing;
                double recommended_shift = Math.Max(spacing_shift,shift);

                if (!PatternGenerator.checkCoordinateLimits(to.X + shiftx, to.Y + shifty))
                {
                    Point2 next = PatternGenerator.findNextPosition(mapContext.X, mapContext.Y, recommended_shift);
                    shiftx = (int)(next.X - mapContext.X);
                    shifty = (int)(next.Y - mapContext.Y);

                    double norm = Math.Sqrt(Utils.sqr(shiftx) + Utils.sqr(shifty));

                    if (norm < 0.1)
                        norm = 1;

                    shiftx = (int)(shiftx / norm * recommended_shift);
                    shifty = (int)(shifty / norm * recommended_shift);

                }

                mapContext.X = (int)to.X + shiftx;
                mapContext.Y = (int)to.Y + shifty;

                angle = tmp_angle;
            }

            return result;
        }

        private Point2 pointToCenter(Point2 from, int tmp_spacing)
        {
            float dx = from.X - (Utils.Xmax + Utils.Xmin) / 2;
            float dy = from.Y - (Utils.Ymax + Utils.Ymin) / 2;

            float norm = (float)Math.Sqrt(Utils.sqr(dx) + Utils.sqr(dy));

            dx = dx / norm * tmp_spacing;
            dy = dy / norm * tmp_spacing;

            return new Point2(from.X + dx, from.Y + dy);
        }

        private double calcCircleSegmentLength(Point2 from, Point2 to, Point2 mid)
        {
            Circle circle = new Circle(from, to, mid);

            Point2 center = circle.center;
            double radius = circle.radius;

            double angle1 = PatternGenerator.calcAngle(from, center);
            double angle2 = PatternGenerator.calcAngle(to, center);

            return Math.Abs(angle2 - angle1) * radius;
        }

        private Point2 binarySeachPoint(Point2 from, double angle, int distToNextPoint, double shift_delta)
        {
            double high = distToNextPoint;
            double low = 0;

            double eps = 1e-3;

            Point2 to = null;

            while (high - low > eps)
            {
                double current = (low + high) / 2;

                to = new Point2(from.X + (float)(current * Math.Cos(angle)), from.Y + (float)(current * Math.Sin(angle)));
                var mid = new Point2((to.X + from.X) / 2, (to.Y + from.Y) / 2);
                mid.X += (float)(shift_delta * Math.Sin(angle));
                mid.Y += (float)(shift_delta * Math.Cos(angle));
                if (!PatternGenerator.checkCoordinateLimits(mid))
                {
                    mid.X -= 2 * (float)(shift_delta * Math.Sin(angle));
                    mid.Y -= 2 * (float)(shift_delta * Math.Cos(angle));
                }

                double spacing = calcCircleSegmentLength(from, to, mid);

                if (spacing < distToNextPoint)
                    low = current;
                else
                    high = current;
            }

            return to;

        }

        private double calcSpacing(List<CircleObject> pattern)
        {
            if (pattern.Count < 2)
                throw new Exception("Stream of less than two notes");
            return pattern.Last().Location.DistanceTo(pattern[pattern.Count - 2].Location);
        }

        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                double endOffset = context.endOffset;
                double currOffset = context.Offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm * 2);

                number = n / points;

            }
            notes.AddRange(generateStreams(context,points, number, spacing, shift));

            return notes;
        }

        public static ConfiguredStreams randomPattern(int level)
        {
            int points = Utils.rng.Next(4, 8);
            int number = Utils.rng.Next(5, 10) * level;
            int spacing = (level - 1) * 30 + Utils.rng.Next(10,20);
            int curviness = (level - 1) * 30 + Utils.rng.Next(20);
            int shift = (level - 1) * 40 + Utils.rng.Next(10, 20);

            ConfiguredStreams p = new ConfiguredStreams(points, number, spacing, curviness, shift, false);
            return p;
        }
    }
}
