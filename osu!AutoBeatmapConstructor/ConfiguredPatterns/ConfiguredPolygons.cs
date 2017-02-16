using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMAPI.v1.HitObjects;
using BMAPI;

namespace osu_AutoBeatmapConstructor
{
    public class ConfiguredPolygons : ConfiguredPattern
    {
        public int points;
        public int number;
        public int spacing;
        public int rotation;
        public int shift;
        public bool randomize;

        public ConfiguredPolygons(int points, int number, int spacing, int rotation, int shift, bool randomize, bool end) : base(PatternType.Polygons, end)
        {
            this.points = points;
            this.number = number;
            this.spacing = spacing;
            this.rotation = rotation;
            this.shift = shift;
            this.randomize = randomize;
        }

        public ConfiguredPolygons()
        {

        }

        private Point2 checkPolygonialBounds(int X, int Y, int spacing)
        {
            double upperBoundOverhead = Utils.Ymin - (Y - spacing);
            double downBoundOverhead = (Y + spacing) - Utils.Ymax;

            double leftBoundOverhead = Utils.Xmin - (X - spacing);
            double rightBoundOverhead = (X + spacing) - Utils.Xmax;

            if (upperBoundOverhead <= 0 && downBoundOverhead <= 0 && leftBoundOverhead <= 0 && rightBoundOverhead <= 0)
            {
                return null;
            }

            double shiftX = 0;
            double shiftY = 0;

            if (leftBoundOverhead > 0 && rightBoundOverhead > 0)
                throw new Exception("leftBoundOverhead and rightBoundOverhead are both positive => probably the spacing is too large");

            if (upperBoundOverhead > 0 && downBoundOverhead > 0)
                throw new Exception("upperBoundOverhead and downBoundOverhead are both positive => probably the spacing is too large");

            if (leftBoundOverhead > 0)
            {
                shiftX = leftBoundOverhead;
            }
            if (rightBoundOverhead > 0)
            {
                shiftX = -rightBoundOverhead;
            }

            if (upperBoundOverhead > 0)
            {
                shiftY = upperBoundOverhead;
            }
            if (downBoundOverhead > 0)
            {
                shiftY = -downBoundOverhead;
            }

            Point2 point = new Point2((float)shiftX + X, (float)shiftY + Y);

            return point;
        }

        public List<CircleObject> generatePolygons(MapContextAwareness mapContext ,int points, int number, int spacing, int rotation, int shift, bool randomize)
        {
             var result = new List<CircleObject>();

            double angle = 0;
            int X = mapContext.X;
            int Y = mapContext.Y;

            int shiftx = shift;
            int shifty = shift;

            for (int i = 0; i < number && mapContext.Offset < mapContext.endOffset; ++i)
            {
                Point2 pp = checkPolygonialBounds(X, Y, spacing);

                if (!ReferenceEquals(pp, null))
                {
                    X = (int)pp.X;
                    Y = (int)pp.Y;
                }

                var ps = PatternGenerator.polygon(points, new Point2(X, Y), spacing, angle);

                if (randomize)
                {
                    //ps = PatternGenerator.shuffleOrder(ps);
                    ps = reoderIntoStar(ps);
                }

                foreach (var p in ps)
                {
                    p.Type = BMAPI.v1.HitObjectType.Circle;
                }
                ps[0].Type |= BMAPI.v1.HitObjectType.NewCombo;

                foreach (var obj in ps)
                {
                    obj.StartTime = (int)mapContext.Offset;
                    mapContext.Offset += mapContext.bpm;
                }

                result.AddRange(ps);
                angle += PatternGenerator.ConvertToRadians(rotation);
                if (!PatternGenerator.checkCoordinateLimits(X, Y, points, spacing, shiftx, shifty))
                {
                    //TODO: add normalization
                    Point2 next = PatternGenerator.findNextPosition(X, Y, shift);
                    shiftx = (int)(next.X - X);
                    shifty = (int)(next.Y - Y);
                }
                X += shiftx;
                Y += shifty;
            }

            double nextShift = Math.Sqrt(2 * spacing * spacing * (1.0 - Math.Cos(Math.PI * 2 / points)));
            Point2 nextPosition = PatternGenerator.findNextPosition(X, Y, nextShift);

            mapContext.X = (int)nextPosition.X;
            mapContext.Y = (int)nextPosition.Y;

            return result;
        }


        public List<CircleObject> reoderIntoStar(List<CircleObject> polygon)
        {
            int n = polygon.Count;

            bool[] used = null; //массив пометок - была ли использована вершина
            Array.Resize(ref used, n);

            var res = new List<CircleObject>();
            for (int i = 0; i < n; i++)
            {
                if (!used[i])
                { //если еще не были в вершине
                    int ptr = i; //текущая вершина
                    while (!used[ptr])
                    { //пока не прошли цикл
                        used[ptr] = true; //помечаем вершину
                        res.Add(polygon[ptr]); //добавляем бит к звезде
                        ptr += n / 2; //делаем шаг размера k
                        if (ptr >= n) //если вышли за пределы индексов
                            ptr -= n; //вычитаем n
                                      //то же самое:
                                      //ptr = (ptr + k) % n;
                    }
                }
            }
            return res;
        }


        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                number = (int)1e6;
            }
            notes.AddRange(generatePolygons(context, points, number, spacing, rotation, shift, randomize));

            return notes;
        }

        public override string ToString()
        {
            string description;
            if (end)
            {
                description = points + "-Polygons till end";
            }
            else
            {
                description = number + " " + points + "-Polygons";
            }
            return description;
        }

        public static ConfiguredPolygons randomPattern(int level)
        {
            int points;
            if (level <= 2)
                points = Utils.rng.Next(3, 6);
            else
                points = Utils.rng.Next(3, 8);

            int number = Utils.rng.Next(5, 10) * level;
            int spacing = (level - 1) * 60 + Utils.rng.Next(10, 60);
            int rotation = Utils.rng.Next(5, 60);
            int shift = Utils.rng.Next(5, 13) * level;
            bool randomize = Utils.rng.Next(1,5) <= level;

            ConfiguredPolygons p = new ConfiguredPolygons(points, number, spacing, rotation, shift, randomize, false);
            return p;
        }
    }
}
