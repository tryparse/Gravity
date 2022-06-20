using System;
using System.Collections.Generic;
using System.Linq;
using Illumination.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Illumination.Visibility
{
    class VisibilityCalculator
    {
        private const int Digits = 5;
        private List<VisibilitySegment> _segments = new List<VisibilitySegment>();
        private List<VisibilityEndpoint> _endpoints = new List<VisibilityEndpoint>();
        private List<RayCast> _rayCasts = new List<RayCast>();
        private float _radius;

        public IReadOnlyList<VisibilitySegment> Segments => _segments;

        public IReadOnlyList<VisibilityEndpoint> Endpoints => _endpoints;

        public IReadOnlyCollection<RayCast> RayCasts => _rayCasts;

        public Vector2 PointOfView { get; private set; }

        public float MaxAngle { get; private set; }

        public HashSet<Polygon> Calculate(Vector2 pov, float radius, List<VisibilitySegment> obstacles, float maxAngle = MathHelper.TwoPi)
        {
            MaxAngle = maxAngle;
            PointOfView = pov;
            _radius = radius;

            _segments.Clear();
            _endpoints.Clear();
            _rayCasts.Clear();

            _segments.AddRange(obstacles);

            var visibleArea = new CircleF(pov, radius);
            var visibleAreaEndpoints = new List<VisibilityEndpoint>();
            var visibleAreaBoundaries = new List<VisibilitySegment>();

            for (int i = 0; i <= 360; i += 20)
            {
                Point2 point = visibleArea.BoundaryPointAt(MathHelper.ToRadians(i));

                visibleAreaEndpoints.Add(new VisibilityEndpoint(point)
                {
                    Angle = AngleHelper.CalculateAngle(pov, point)
                });
            }

            for (int i = 0; i < visibleAreaEndpoints.Count - 1; i++)
            {
                visibleAreaBoundaries.Add(new VisibilitySegment(visibleAreaEndpoints[i].Point, visibleAreaEndpoints[i + 1].Point));
            }

            _endpoints = CollectEndpoints(PointOfView);

            var output = BuildVisibilityTriangles();

            return output;
        }

        private HashSet<Polygon> BuildVisibilityTriangles()
        {
            var output = new HashSet<Polygon>();

            var endpointsInRadius = _endpoints
                .Where(x => Vector2.Distance(PointOfView, x.Point) <= _radius);

            foreach (var e in endpointsInRadius)
            {
                var direction = e.Angle.ToUnitVector();

                var ray = new Ray2(PointOfView, direction);
                var additionalRay1 = new Ray2(PointOfView, new Angle(e.Angle.Degrees - 0.1f, AngleType.Degree).ToUnitVector());
                var additionalRay2 = new Ray2(PointOfView, new Angle(e.Angle.Degrees + 0.1f, AngleType.Degree).ToUnitVector());

                _rayCasts.Add(new RayCast(additionalRay1, _radius));
                _rayCasts.Add(new RayCast(ray, _radius));
                _rayCasts.Add(new RayCast(additionalRay2, _radius));
            }

            for (int i = 0; i < 360; i+=90)
            {
                var angle = new Angle(MathHelper.ToRadians(i));
                var direction = angle.ToUnitVector();

                var ray = new Ray2(PointOfView, direction);

                _rayCasts.Add(new RayCast(ray, _radius));
            }

            //_rayCasts = _rayCasts
            //    .OrderBy(x => x.Angle)
            //    .ToList();

            foreach (var r in _rayCasts)
            {
                foreach (var s in _segments)
                {
                    if (r.CheckIntersection(s.Points, out var intersection))
                    {
                        if (!r.Intersections.Contains(intersection))
                        {
                            r.Intersections.Add(intersection);
                        }
                    }
                }
            }

            for (int i = 0; i < _rayCasts.Count - 1; i += 1)
            {
                var p1 = _rayCasts[i].HasIntersection
                    ? _rayCasts[i].NearestIntersection
                    : _rayCasts[i].Segment.End;

                var p2 = _rayCasts[i + 1].HasIntersection
                    ? _rayCasts[i + 1].NearestIntersection
                    : _rayCasts[i + 1].Segment.End;

                var verticies = new Vector2[3]
                {
                    PointOfView,
                    p1,
                    p2
                };

                output.Add(new Polygon(verticies));
            }

            _rayCasts = _rayCasts.OrderBy(x => x.Angle.Degrees).ToList();

            var last = _rayCasts.LastOrDefault();
            var first = _rayCasts.FirstOrDefault();

            if (last != first)
            {
                var p1 = last.HasIntersection
                    ? last.NearestIntersection
                    : last.Segment.End;

                var p2 = first.HasIntersection
                    ? first.NearestIntersection
                    : first.Segment.End;

                var verticies = new Vector2[3]
                {
                    PointOfView,
                    p1,
                    p2
                };

                output.Add(new Polygon(verticies));
            }

            return output;
        }

        private List<VisibilityEndpoint> CollectEndpoints(Vector2 pov)
        {
            var result = new List<VisibilityEndpoint>();

            foreach (var segment in _segments)
            {
                if (!result.Exists(x =>
                    Math.Round(x.Point.X, Digits) == Math.Round(segment.Points.Start.X, Digits)
                    && Math.Round(x.Point.Y, Digits) == Math.Round(segment.Points.Start.Y, Digits)))
                {
                    result.Add(new VisibilityEndpoint(segment.Points.Start)
                    {
                        Angle = AngleHelper.CalculateAngle(pov, segment.Points.Start),
                    });
                }

                if (!result.Exists(x =>
                    Math.Round(x.Point.X, Digits) == Math.Round(segment.Points.End.X, Digits)
                    && Math.Round(x.Point.Y, Digits) == Math.Round(segment.Points.End.Y, Digits)))
                {
                    result.Add(new VisibilityEndpoint(segment.Points.End)
                    {
                        Angle = AngleHelper.CalculateAngle(pov, segment.Points.End),
                    });
                }
            }

            result.Sort((a, b) =>
            {
                return a.Angle.Degrees.CompareTo(b.Angle.Degrees);
            });

            return result;
        }

        public static void DeconstructRectangleToSegments(RectangleF rectangle, out List<VisibilitySegment> segments)
        {
            segments = new List<VisibilitySegment>();

            segments.Add(new VisibilitySegment(rectangle.TopRight, rectangle.BottomRight));
            segments.Add(new VisibilitySegment(rectangle.BottomRight, rectangle.BottomLeft));
            segments.Add(new VisibilitySegment(rectangle.BottomLeft, rectangle.TopLeft));
            segments.Add(new VisibilitySegment(rectangle.TopLeft, rectangle.TopRight));
        }
    }
}
