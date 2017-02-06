using BMAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_AutoBeatmapConstructor
{
    public class Circle
    {
        public float radius;
        public Point2 center;

        private void FindCircle(Point2 a, Point2 b, Point2 c,
    out Point2 center, out float radius)
        {
            // Get the perpendicular bisector of (x1, y1) and (x2, y2).
            float x1 = (b.X + a.X) / 2;
            float y1 = (b.Y + a.Y) / 2;
            float dy1 = b.X - a.X;
            float dx1 = -(b.Y - a.Y);

            // Get the perpendicular bisector of (x2, y2) and (x3, y3).
            float x2 = (c.X + b.X) / 2;
            float y2 = (c.Y + b.Y) / 2;
            float dy2 = c.X - b.X;
            float dx2 = -(c.Y - b.Y);

            // See where the lines intersect.
            bool lines_intersect, segments_intersect;
            Point2 intersection, close1, close2;
            FindIntersection(
                new Point2(x1, y1), new Point2(x1 + dx1, y1 + dy1),
                new Point2(x2, y2), new Point2(x2 + dx2, y2 + dy2),
                out lines_intersect, out segments_intersect,
                out intersection, out close1, out close2);
            if (!lines_intersect)
            {
                //throw new Exception("The points are colinear");
                center = new Point2(0, 0);
                radius = 0;
            }
            else
            {
                center = intersection;
                float dx = center.X - a.X;
                float dy = center.Y - a.Y;
                radius = (float)Math.Sqrt(dx * dx + dy * dy);
            }
        }

        private void FindIntersection(
    Point2 p1, Point2 p2, Point2 p3, Point2 p4,
    out bool lines_intersect, out bool segments_intersect,
    out Point2 intersection,
    out Point2 close_p1, out Point2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point2(float.NaN, float.NaN);
                close_p1 = new Point2(float.NaN, float.NaN);
                close_p2 = new Point2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Point2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Point2(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point2(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        public Circle(Point2 A, Point2 B, Point2 C)
        {
            FindCircle(A,B,C,out center,out radius);
        }
    }
}
