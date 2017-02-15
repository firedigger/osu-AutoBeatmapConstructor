using BMAPI;
using BMAPI.v1.HitObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMAPI.v1;

namespace osu_AutoBeatmapConstructor
{
    public static class PatternGenerator
    {
        private static Random rng = new Random();

        public static List<CircleObject> stream(int points, Point2 center, double radius, double angle1, double angle2)
        {
            var result = new List<CircleObject>();

            double dangle = (angle2 - angle1)/(points - 1);

            for (int i = 0; i < points; ++i)
            {
                CircleObject o1 = new CircleObject();
                o1.Location = new Point2(center.X + (float)(radius * Math.Cos(angle1 + i * dangle)), center.Y - (float)(radius * Math.Sin(angle1 + i * dangle)));
                result.Add(o1);
            }

            return result;
        }

        public static double vecAbs(Point2 v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static double calcAngle(Point2 p, Point2 center)
        {
            double dx = p.X - center.X;
            double dy = p.Y - center.Y;

            double res = Math.Acos(dx / vecAbs(dx, dy));
            if (dy > 0)
            { 
                res = -res;
            }
            return res;
        }

        private static double vecAbs(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static List<CircleObject> stream(int points, Point2 from, Point2 to, Point2 mid)
        {
            Circle circle = new Circle(from, to, mid);

            Point2 center = circle.center;
            double radius = circle.radius;

            double angle1 = calcAngle(from, center);
            double angle2 = calcAngle(to, center);

            return stream(points, center, radius, angle1, angle2);
        }

        public static bool checkCoordinateLimits(Point2 to)
        {
            return to.X >= Utils.Xmin && to.X <= Utils.Xmax && to.Y >= Utils.Ymin && to.Y <= Utils.Ymax;
        }

        public static List<CircleObject> stream(int points, Point2 from, Point2 to)
        {
            var result = new List<CircleObject>();

            float dx = (to.X - from.X) / points;
            float dy = (to.Y - from.Y) / points;

            for(int i = 0; i < points; ++i)
            {
                CircleObject o1 = new CircleObject();
                o1.Location = new Point2(from.X + i * dx, from.Y + i * dy);
                result.Add(o1);
            }

            return result;
        }

        public static bool checkCoordinateLimits(double x, double y)
        {
            return x >= 0 && x <= 512 && y >= 0 && y <= 384;
        }

        public static void copyMap(Beatmap baseBeatmap, Beatmap generatedMap, double offset, double endOffset)
        {
            foreach(var obj in baseBeatmap.HitObjects)
            {
                if (obj.StartTime < offset || obj.StartTime > endOffset)
                {
                    generatedMap.HitObjects.Add(obj);
                }
            }
        }

        public static Point2 findNextPosition(int X, int Y, double nextShift)
        {
            double angle = rng.NextDouble() * Math.PI * 2;
            Point2 to = new Point2(X + (float)(Math.Cos(angle) * nextShift), Y + (float)(Math.Sin(angle) * nextShift));
            for (int k = 0; !checkCoordinateLimits(to); ++k)
            {
                angle = rng.NextDouble() * Math.PI * 2;

                to = new Point2(X + (float)(Math.Cos(angle) * nextShift), Y + (float)(Math.Sin(angle) * nextShift));

                if (k > 100)
                {
                    //throw new Exception("Couldn't find next position: " + x + " " + y + " " + nextShift);
                    break;
                }

            }

            to = new Point2(X + (float)(Math.Cos(angle) * nextShift), Y + (float)(Math.Sin(angle) * nextShift));

            if (!checkCoordinateLimits(to))
            {
                if (to.X < Utils.Xmin)
                    to.X = Utils.Xmin;
                if (to.X > Utils.Xmax)
                    to.X = Utils.Xmax;
                if (to.Y < Utils.Ymin)
                    to.Y = Utils.Ymin;
                if (to.Y > Utils.Ymax)
                    to.Y = Utils.Ymax;
            }

            return to;

        }

        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static bool checkCoordinateLimits(int x, int y, int points, int spacing, int shiftx, int shifty)
        {
            return x - spacing + shiftx >= 0 && x +spacing + shiftx <= 512 && y - spacing + shifty >= 0 && y + spacing + shifty <= 384;
        }

        public static List<CircleObject> polygon(int points, Point2 center, int size, double angle)
        {
            var result = new List<CircleObject>();

            double dangle = Math.PI / points * 2;

            double offset = Math.PI - dangle / 2;
            for (int i = 0; i < points; ++i)
            {
                CircleObject o1 = new CircleObject();
                o1.Location = new Point2(center.X + size * (float)Math.Cos(offset), center.Y - size * (float)Math.Sin(offset));
                result.Add(o1);
                offset -= dangle;
            }

            rotate(result, center, angle);

            return result;
        }


        public static void rotate(List<CircleObject> points, Point2 center, double angle)
        {
            foreach(var point in points)
            {
                point.Location.X -= center.X;
                point.Location.Y -= center.Y;

                float tx = point.Location.X;
                float ty = point.Location.Y;

                point.Location.X = tx * (float)Math.Cos(angle) - ty * (float)Math.Sin(angle);
                point.Location.Y = tx * (float)Math.Sin(angle) + ty * (float)Math.Cos(angle);

                point.Location.X += center.X;
                point.Location.Y += center.Y;
            }
        }

        public static List<CircleObject> shuffleOrder(List<CircleObject> pattern)
        {
            var result = pattern.OrderBy(x => rng.Next()).ToList();
            return result;
        }

        //TODO replace for general polygon to_star algorithm
        public static List<CircleObject> polygonToStar(List<CircleObject> polygon)
        {
            if (polygon.Count != 5)
                throw new InvalidOperationException("Star transform can be only applied to 5-note patterns");

            var result = new List<CircleObject>();
            result.Add(polygon[0]);
            result.Add(polygon[2]);
            result.Add(polygon[4]);
            result.Add(polygon[1]);
            result.Add(polygon[3]);
            return result;
        }

        public static List<CircleObject> generateHorizontalJump(Point2 start, int spacing, double angle, bool randomize)
        {
            var result = new List<CircleObject>();

            CircleObject o1 = new CircleObject();
            o1.Location = start;
            result.Add(o1);

            CircleObject o2 = new CircleObject();
            o2.Location = new Point2(start);
            o2.Location.X += spacing;
            result.Add(o2);

            Point2 center = new Point2(o1.Location);
            center.X = (o1.Location.X + o2.Location.X) / 2;

            rotate(result, center, angle);

            return result;
        }

        public static List<CircleObject> generateVerticalJump(Point2 start, int spacing, double angle, bool randomize)
        {
            var result = new List<CircleObject>();

            CircleObject o1 = new CircleObject();
            o1.Location = start;
            result.Add(o1);

            CircleObject o2 = new CircleObject();
            o2.Location = new Point2(start);
            o2.Location.Y -= spacing;
            result.Add(o2);

            Point2 center = new Point2(o1.Location);
            center.Y = (o1.Location.Y + o2.Location.Y) / 2;

            rotate(result, center, angle);

            return result;
        }

        public static List<CircleObject> generateRotationalJump(Point2 start, int spacing, double angle, bool randomize)
        {
            return generateVerticalJump(start, spacing, angle, randomize);
        }

    }
}
