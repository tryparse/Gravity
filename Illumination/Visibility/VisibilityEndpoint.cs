using System;
using MonoGame.Extended;

namespace Illumination.Visibility
{
    class VisibilityEndpoint
    {
        public Point2 Point { get; }

        public Angle Angle { get; set; }

        public VisibilityEndpoint(Point2 point)
        {
            Point = point;
        }

        public override string ToString()
        {
            return $"{Math.Round(Point.X, 1)},{Math.Round(Point.Y, 1)} {Math.Round(Angle.Degrees, 1)}";
        }
    }
}
