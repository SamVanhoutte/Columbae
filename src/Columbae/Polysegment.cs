using System;
using System.Linq;
using Newtonsoft.Json;

namespace Columbae
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Polysegment : IShape, IEquatable<Polysegment>
    {
        const double Tolerance = 0.001;

        public Polypoint Start { get; }
        public Polypoint End { get; }

        public Polysegment(Polypoint point1, Polypoint point2)
        {
            // Set right order of start / end
            if (point1.X < point2.X)
            {
                Start = point1;
                End = point2;
            }
            else
            {
                if (point1.X > point2.X)
                {
                    Start = point2;
                    End = point1;
                }
                else
                {
                    // same long
                    if (point1.Y < point2.Y)
                    {
                        Start = point1;
                        End = point2;
                    }
                    else
                    {
                        Start = point2;
                        End = point1;
                    }
                }
            }
        }

        // give the mid point
        public Polypoint MidPoint()
        {
            return new Polypoint((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        }

        //given a point that is in line (p1,p2)
        //check if a point is in this segment
        public bool IsOnTheLine(Polypoint point)
        {
            //TODO : investigate this
            if ((Math.Abs(point.Y - Start.Y) < Tolerance && Math.Abs(point.X - Start.X) < Tolerance)
                || (Math.Abs(point.Y - End.Y) < Tolerance && Math.Abs(point.X - End.X) < Tolerance))
            {
                return true;
            }

            return IsInArea(point) && (Math.Abs((point.X - Start.X) / (End.X - Start.X) - (point.Y - Start.Y) / (End.Y - Start.Y)) < Tolerance);
        }

        //given a point that is in line (p1,p2)
        //check if a point is in this segment
        public bool IsInArea(Polypoint point)
        {
            //TODO : investigate this
            if ((Math.Abs(point.Y - Start.Y) < Tolerance && Math.Abs(point.X - Start.X) < Tolerance)
                || (Math.Abs(point.Y - End.Y) < Tolerance && Math.Abs(point.X - End.X) < Tolerance))
            {
                return true;
            }

            var minLat = Math.Min(Start.Y, End.Y);
            var maxLat = Math.Max(Start.Y, End.Y);
            var minLon = Math.Min(Start.X, End.X);
            var maxLon = Math.Max(Start.X, End.X);

            return (point.X >= minLon &&
                    point.X <= maxLon &&
                    point.Y >= minLat &&
                    point.Y <= maxLat);
        }

        //given a point that is in line (p1,p2)
        //check if that point is inside this segment
        //segment does not contain its end points
        public bool IsInArea(double lon, double lat)
        {
            return IsInArea(new Polypoint(lat, lon));
        }

        public bool Intersects(Polysegment seg, out Polypoint intersection, bool considerOverlapAsIntersect = true)
        {
            intersection = null;

            var r = End - Start;
            var s = seg.End - seg.Start;
            var rxs = r.Cross(s);
            var qpxr = (seg.Start - Start).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerOverlapAsIntersect)
                    if ((0 <= (seg.Start - Start) * r && (seg.Start - Start) * r <= r * r) ||
                        (0 <= (Start - seg.Start) * s && (Start - seg.Start) * s <= s * s))
                        return true;

                // 2. If neither 0 <= (q - p) * r ≤ r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (seg.Start - Start).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (seg.Start - Start).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = Start + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }


        public PointPosition GetPointPositioning(Polypoint p)
        {
            var t = ((End.Y - Start.Y) * (p.X - Start.X) -
                            (End.X - Start.X) * (p.Y - Start.Y));
            if (t > 0) // p is on the left
                return PointPosition.Left;
            if (t < 0)
                return PointPosition.Right;
            return IsInArea(p) ? PointPosition.OnLine : PointPosition.OnLineOffSegment;
        }

        public bool Contains(Polypoint point, out Polypoint closestPoint, double margin = 0.0)
        {
            return CalculateDistance(point, out closestPoint) <= margin;
        }
        
        public double CalculateDistance(Polypoint pt, out Polypoint closest)
        {
            //http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
            var dx = End.X - Start.X;
            var dy = End.Y - Start.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = Start;
                return Start.GetDistance(pt);
            }

            // Calculate the t that minimizes the distance.
            var t = ((pt.X - Start.X) * dx + (pt.Y - Start.Y) * dy) /
                      (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point between start & end.
            // We're getting the closest point (shortest t) and will then calculate the distance
            if (t <= 0) closest = Start;
            else if (t >= 1) closest = End;
            else
            {
                closest = new Polypoint(Start.X + t * dx, Start.Y + t * dy);
            }

            return closest.GetDistance(pt);
        }

        // check if a point is end point of this segment
        public bool IsEndPoint(Polypoint p)
        {
            return Start.Equals(p) || End.Equals(p);
        }

        // check if a point is end point of this segment
        public bool IsEndPoint(double x, double y)
        {
            var tmp = new Polypoint(y, x);
            return IsEndPoint(tmp);
        }

        public static Polysegment Parse(string segmentString)
        {
            // Format should be of X1,Y1,X2,Y2
            var coordinates = segmentString.Split(',');
            if (coordinates.Length == 4)
            {
                if (coordinates.All(s => double.TryParse(s, out _)))
                {
                    //TODO :localization
                    return new Polysegment(
                        new Polypoint(double.Parse(coordinates[0]), double.Parse(coordinates[1])),
                        new Polypoint(double.Parse(coordinates[2]), double.Parse(coordinates[3])));
                }
            }

            return null;
        }

        public bool Equals(Polysegment other)
        {
            if (other == null) return false;
            return Equals(other.Start, Start) && Equals(other.End, End);
        }
    }

    public enum PointPosition
    {
        OnLine = 0,
        Left = 1,
        Right = -1,
        OnLineOffSegment = 2
    }
}