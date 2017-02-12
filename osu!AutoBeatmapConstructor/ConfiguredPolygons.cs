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
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm / points) - 1;

                number = n;
            }

            var result = new List<CircleObject>();

            double angle = 0;
            int X = mapContext.X;
            int Y = mapContext.Y;

            int shiftx = shift;
            int shifty = shift;

            for (int i = 0; i < number; ++i)
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
                    ps = PatternGenerator.shuffleOrder(ps);
                }

                foreach (var p in ps)
                {
                    p.Type = BMAPI.v1.HitObjectType.Circle;
                }
                ps[0].Type |= BMAPI.v1.HitObjectType.NewCombo;

                foreach (var obj in ps)
                {
                    obj.StartTime = (int)mapContext.offset;
                    mapContext.offset += mapContext.bpm;
                }

                result.AddRange(ps);
                angle += PatternGenerator.ConvertToRadians(rotation);
                if (!PatternGenerator.checkCoordinateLimits(X, Y, points, spacing, shiftx, shifty))
                {
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

        public override List<CircleObject> generatePattern(MapContextAwareness context)
        {
            var notes = new List<CircleObject>();

            if (end)
            {
                double endOffset = context.endOffset;
                double currOffset = context.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / context.bpm);

                number = n / points;

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
    }
}
