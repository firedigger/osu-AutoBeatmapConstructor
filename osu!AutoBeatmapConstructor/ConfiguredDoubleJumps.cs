using System;
using System.Collections.Generic;
using BMAPI.v1.HitObjects;
using BMAPI;

namespace osu_AutoBeatmapConstructor
{
    public class ConfiguredDoubleJumps : ConfiguredPattern
    {
        private int number;
        private int spacing;
        private int shift;
        private int rotation;
        private DoubleJumpType jumpType;
        private bool randomize;

        public ConfiguredDoubleJumps(DoubleJumpType jumpType, int number, int spacing, int rotation, int shift, bool randomize, bool end) : base(PatternType.DoubleJumps, end)
        {
            this.jumpType = jumpType;
            this.number = number;
            this.spacing = spacing;
            this.rotation = rotation;
            this.shift = shift;
            this.randomize = randomize;
            this.end = end;
        }

        public List<CircleObject> generateHorizontalPattern(MapContextAwareness mapContext)
        {
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm / 2) - 1;

                number = n;
            }

            var result = new List<CircleObject>();

            double angle = rotation;
            int X = mapContext.X;
            int Y = mapContext.Y;

            int shiftx = 0;
            int shifty = shift;

            for (int i = 0; i < number; ++i)
            {
                Point2 pp = checkHorizontalJumpBounds(X, Y, spacing, angle);

                if (!ReferenceEquals(pp, null))
                {
                    X = (int)pp.X;
                    Y = (int)pp.Y;
                }

                var ps = PatternGenerator.generateHorizontalJump(new Point2(X,Y),spacing, angle, randomize);
                
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
                //angle += PatternGenerator.ConvertToRadians(rotation);
                if (!PatternGenerator.checkCoordinateLimits(X + shiftx, Y + shifty))
                {
                    shifty = -shifty;
                }
                X += shiftx;
                Y += shifty;
            }

            double nextShift = shift;
            Point2 nextPosition = PatternGenerator.findNextPosition(X, Y, nextShift);

            mapContext.X = (int)nextPosition.X;
            mapContext.Y = (int)nextPosition.Y;

            return result;
        }

        private Point2 checkHorizontalJumpBounds(int X, int Y, int spacing, double angle)
        {
            double dy1 = (Y + spacing * Math.Sin(PatternGenerator.ConvertToRadians(angle)));
            double dy2 = (Y - spacing * Math.Sin(PatternGenerator.ConvertToRadians(angle)));

            double upperBoundOverhead = Utils.Ymin - Math.Min(dy1,dy2);
            double downBoundOverhead = Math.Max(dy1, dy2) - Utils.Ymax;

            double dx1 = (X + spacing * Math.Cos(PatternGenerator.ConvertToRadians(angle)));
            double dx2 = (X - spacing * Math.Cos(PatternGenerator.ConvertToRadians(angle)));

            double leftBoundOverhead = Utils.Xmin - Math.Min(dx1, dx2);
            double rightBoundOverhead = Math.Max(dx1, dx2) - Utils.Xmax;

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

        public override string ToString()
        {
            string description = jumpType.ToString() + " 1-2 jumps";

            if(end)
            {
                description += " till end";
            }
            else
            {
                description = number + " " + description;
            }

            return description;
        }

        public override List<CircleObject> generatePattern(MapContextAwareness mapContext)
        {
            switch (jumpType)
            {
                case DoubleJumpType.Horizontal: return generateHorizontalPattern(mapContext);
                case DoubleJumpType.Vertical: return generateVerticalPattern(mapContext);
                case DoubleJumpType.Rotating: return generateRotatingPattern(mapContext);
                default: throw new Exception("Unknown double jump type value");
            }
        }

        private List<CircleObject> generateRotatingPattern(MapContextAwareness mapContext)
        {
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm / 2) - 1;

                number = n;
            }

            var result = new List<CircleObject>();

            double angle = rotation;
            int X = mapContext.X;
            int Y = mapContext.Y;

            int shiftx = shift;
            int shifty = shift;

            for (int i = 0; i < number; ++i)
            {
                Point2 pp = checkHorizontalJumpBounds(X, Y, spacing, angle);

                if (!ReferenceEquals(pp, null))
                {
                    X = (int)pp.X;
                    Y = (int)pp.Y;
                }

                var ps = PatternGenerator.generateRotationalJump(new Point2(X, Y), spacing, angle, randomize);

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
                if (!PatternGenerator.checkCoordinateLimits(X + shiftx, Y + shifty))
                {
                    Point2 next = PatternGenerator.findNextPosition(X, Y, shift);
                    shiftx = (int)(next.X - X);
                    shifty = (int)(next.Y - Y);
                }
                X += shiftx;
                Y += shifty;
            }

            double nextShift = shift;
            Point2 nextPosition = PatternGenerator.findNextPosition(X, Y, nextShift);

            mapContext.X = (int)nextPosition.X;
            mapContext.Y = (int)nextPosition.Y;

            return result;
        }

        private List<CircleObject> generateVerticalPattern(MapContextAwareness mapContext)
        {
            if (end)
            {
                double endOffset = mapContext.endOffset;
                double currOffset = mapContext.offset;

                int n = (int)Math.Floor((endOffset - currOffset) / mapContext.bpm / 2) - 1;

                number = n;
            }

            var result = new List<CircleObject>();

            double angle = rotation;
            int X = mapContext.X;
            int Y = mapContext.Y;

            int shiftx = shift;
            int shifty = 0;

            for (int i = 0; i < number; ++i)
            {
                Point2 pp = checkVerticalJumpBounds(X, Y, spacing, angle);

                if (!ReferenceEquals(pp, null))
                {
                    X = (int)pp.X;
                    Y = (int)pp.Y;
                }

                var ps = PatternGenerator.generateVerticalJump(new Point2(X, Y), spacing, angle, randomize);

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
                //angle += PatternGenerator.ConvertToRadians(rotation);
                if (!PatternGenerator.checkCoordinateLimits(X + shiftx, Y + shifty))
                {
                    shiftx = -shiftx;
                }
                X += shiftx;
                Y += shifty;
            }

            double nextShift = shift;
            Point2 nextPosition = PatternGenerator.findNextPosition(X, Y, nextShift);

            mapContext.X = (int)nextPosition.X;
            mapContext.Y = (int)nextPosition.Y;

            return result;
        }

        private Point2 checkVerticalJumpBounds(int X, int Y, int spacing, double angle)
        {
            double dy1 = (Y + spacing * Math.Cos(PatternGenerator.ConvertToRadians(angle)));
            double dy2 = (Y - spacing * Math.Cos(PatternGenerator.ConvertToRadians(angle)));

            double upperBoundOverhead = Utils.Ymin - Math.Min(dy1, dy2);
            double downBoundOverhead = Math.Max(dy1, dy2) - Utils.Ymax;

            double dx1 = (X + spacing * Math.Sin(PatternGenerator.ConvertToRadians(angle)));
            double dx2 = (X - spacing * Math.Sin(PatternGenerator.ConvertToRadians(angle)));

            double leftBoundOverhead = Utils.Xmin - Math.Min(dx1, dx2);
            double rightBoundOverhead = Math.Max(dx1, dx2) - Utils.Xmax;

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
    }
}