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
            double radius = circle.raduis;

            double angle1 = calcAngle(from, center);
            double angle2 = calcAngle(to, center);

            return stream(points, center, radius, angle1, angle2);
        }

        public static bool checkCoordinateLimits(Point2 to)
        {
            return to.X >= 0 && to.X <= 512 && to.Y >= 0 && to.Y <= 384;
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

        public static List<CircleObject> streamSquare(Point2 center, int size, int rowSize)
        {
            List<CircleObject> result = new List<CircleObject>();

            var list1 = stream(rowSize, new Point2(center.X - size / 2, center.Y - size / 2), new Point2(center.X + size / 2, center.Y - size / 2));
            list1.RemoveAt(list1.Count - 1);
            list1[0].Type |= BMAPI.v1.HitObjectType.NewCombo;
            var list2 = stream(rowSize, new Point2(center.X + size / 2, center.Y - size / 2), new Point2(center.X + size / 2, center.Y + size / 2));
            list2.RemoveAt(list2.Count - 1);
            list2[0].Type |= BMAPI.v1.HitObjectType.NewCombo;
            var list3 = stream(rowSize, new Point2(center.X + size / 2, center.Y + size / 2), new Point2(center.X - size / 2, center.Y + size / 2));
            list3.RemoveAt(list3.Count - 1);
            list3[0].Type |= BMAPI.v1.HitObjectType.NewCombo;
            var list4 = stream(rowSize, new Point2(center.X - size / 2, center.Y + size / 2), new Point2(center.X - size / 2, center.Y - size / 2));
            list4.RemoveAt(list4.Count - 1);
            list4[0].Type |= BMAPI.v1.HitObjectType.NewCombo;

            result.AddRange(list1);
            result.AddRange(list2);
            result.AddRange(list3);
            result.AddRange(list4);

            return result;
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

        public static Point2 findNextPosition(int x, int y, double nextShift)
        {
            double angle = rng.NextDouble() * Math.PI * 2;
            Point2 to = new Point2(x + (float)(Math.Cos(angle) * nextShift), y + (float)(Math.Sin(angle) * nextShift));
            while(!checkCoordinateLimits(to))
            {
                angle = rng.NextDouble() * Math.PI * 2;
                to = new Point2(x + (float)(Math.Cos(angle) * nextShift), y + (float)(Math.Sin(angle) * nextShift));
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
                if (i == 0)
                    o1.Type |= BMAPI.v1.HitObjectType.NewCombo;
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


        public static List<CircleObject> square(Point2 center, int size, double angle)
        {
            var result = new List<CircleObject>();

            float dya = - size * (float)Math.Cos(angle) / 2;
            float dy = size * (float)Math.Cos(Math.PI / 2 - angle) / 2;

            float dxa = size * (float)Math.Sin(angle) / 2;
            float dx = size * (float)Math.Sin(Math.PI / 2 - angle) / 2;

            CircleObject o1 = new CircleObject();
            //o1.Type |= BMAPI.v1.HitObjectType.NewCombo;
            o1.Location = new Point2(center.X + dxa - dx, center.Y + dya - dy);

            CircleObject o2 = new CircleObject();
            o2.Location = new Point2(center.X + dxa + dx, center.Y + dya + dy);

            CircleObject o3 = new CircleObject();
            o3.Location = new Point2(center.X - dxa + dx, center.Y - dya + dy);

            CircleObject o4 = new CircleObject();
            o4.Location = new Point2(center.X - dxa - dx, center.Y - dya - dy);

            result.Add(o1);
            result.Add(o2);
            result.Add(o3);
            result.Add(o4);

            return result;
        }
    }
}
