using System;
using MonoGame.Extended;

namespace Illumination.Utility
{
    static class GeometryHelper
    {
        /// <summary>
        /// This is based off an explanation and expanded math presented by Paul Bourke:
        /// 
        /// It takes two lines as inputs and returns true if they intersect, false if they 
        /// don't.
        /// If they do, ptIntersection returns the point where the two lines intersect.  
        /// </summary>
        /// <param name="a">The first line</param>
        /// <param name="b">The second line</param>
        /// <param name="intersection">The point where both lines intersect (if they do).</param>
        /// <returns></returns>
        /// <remarks>See http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/</remarks>
        public static bool DoLinesIntersect(Segment2 a, Segment2 b, out Point2 intersection, bool ignoreIntersectionWithEnds = false)
        {
            intersection = Point2.NaN;

            // Denominator for ua and ub are the same, so store this calculation
            float d =
               (b.End.Y - b.Start.Y) * (a.End.X - a.Start.X)
               -
               (b.End.X - b.Start.X) * (a.End.Y - a.Start.Y);

            //n_a and n_b are calculated as seperate values for readability
            float n_a =
               (b.End.X - b.Start.X) * (a.Start.Y - b.Start.Y)
               -
               (b.End.Y - b.Start.Y) * (a.Start.X - b.Start.X);

            float n_b =
               (a.End.X - a.Start.X) * (a.Start.Y - b.Start.Y)
               -
               (a.End.Y - a.Start.Y) * (a.Start.X - b.Start.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            float ua = n_a / d;
            float ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                intersection.X = a.Start.X + (ua * (a.End.X - a.Start.X));
                intersection.Y = a.Start.Y + (ua * (a.End.Y - a.Start.Y));

                if (ignoreIntersectionWithEnds
                    && Math.Round(intersection.X, 1) == Math.Round(b.Start.X, 1)
                    && Math.Round(intersection.Y, 1) == Math.Round(b.Start.Y, 1))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool TryCheckIntersection(Segment2 a, Segment2 b, out Point2 intersection)
        {
            var x1 = a.Start.X;
            var y1 = a.Start.Y;
            var x2 = a.End.X;
            var y2 = a.End.Y;

            var x3 = b.Start.X;
            var y3 = b.Start.Y;
            var x4 = b.End.X;
            var y4 = b.End.Y;

            return TryCheckIntersection(x1, y1, x2, y2, x3, y3, x4, y4, out intersection);
        }

        private static bool TryCheckIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, out Point2 intersection)
        {
            intersection = Point2.NaN;

            var d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (d == 0)
            {
                return false;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / d;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / d;

            if (t > 0 && t < 1 && u > 0)
            {
                intersection.X = x1 + t * (x2 - x1);
                intersection.Y = y1 + t * (y2 - y1);

                return true;
            }

            return false;
        }

        public static bool TryCheckIntersection(Ray2 ray, Segment2 segment, out Point2 intersection)
        {
            var raySegment = new Segment2(
                ray.Position,
                new Point2(ray.Position.X + ray.Direction.X, ray.Position.Y + ray.Direction.Y));

            return TryCheckIntersection(segment, raySegment, out intersection);
        }
    }
}
