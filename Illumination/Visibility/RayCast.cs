using System.Collections.Generic;
using System.Linq;
using Illumination.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Illumination.Visibility
{
    class RayCast
    {
        public Ray2 Ray { get; }

        public Angle Angle { get; }

        public Segment2 Segment { get; set; }

        public HashSet<Point2> Intersections = new HashSet<Point2>();

        public bool HasIntersection => Intersections.Any();

        public Point2 NearestIntersection
        {
            get
            {
                if (!Intersections.Any())
                {
                    return Point2.NaN;
                }

                var nearest = Intersections
                    .OrderBy(x => Vector2.Distance(x, Ray.Position))
                    .FirstOrDefault();

                return nearest;
            }
        }

        public RayCast(Ray2 ray, float distance)
        {
            Ray = ray;

            Angle = new Angle(AngleHelper.CalculateAngle(ray.Position, ray.Position + ray.Direction));

            Segment = new Segment2(Ray.Position, Ray.Position + new Vector2(Ray.Direction.X * distance, -Ray.Direction.Y * distance));
        }

        public bool CheckIntersection(Segment2 segment, out Point2 intersection, bool ignoreIntersectionsWithEnds = false)
        {
            return GeometryHelper.DoLinesIntersect(Segment, segment, out intersection, ignoreIntersectionsWithEnds);
        }

        public override string ToString()
        {
            return $"{Ray.Position};{Angle.Degrees}";
        }
    }
}
