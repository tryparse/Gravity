using MonoGame.Extended;

namespace Illumination.Visibility
{
    public class VisibilitySegment
    {
        public Segment2 Points { get; }

        public bool IsIntersects { get; set; }

        public VisibilitySegment(Point2 start, Point2 end)
        {
            Points = new Segment2(start, end);
        }
    }
}
