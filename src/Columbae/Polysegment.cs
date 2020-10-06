using System;
using System.Linq;

namespace Columbae
{
    public class Polysegment
    {
        public Polypoint Start { get; private set; }
        public Polypoint End { get; private set; }

        public Polysegment(Polypoint point1, Polypoint point2)
        {
            // Set right order of start / end
            if (point1.Longitude < point2.Longitude)
            {
                Start = point1;
                End = point2;
            }
            else
            {
                if (point1.Longitude > point2.Longitude)
                {
                    Start = point2;
                    End = point1;
                }
                else
                {
                    // same long
                    if (point1.Latitude < point2.Latitude)
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
            return new Polypoint((Start.Longitude + End.Longitude) / 2, (Start.Latitude + End.Latitude) / 2);
        }

        //given a point that is in line (p1,p2)
        //check if a point is in this segment
        public bool IsOnTheLine(Polypoint point)
        {
            //TODO : investigate this
            if ((point.Latitude == Start.Latitude && point.Longitude == Start.Longitude)
                || (point.Latitude == End.Latitude && point.Longitude == End.Longitude))
            {
                return true;
            }

            return IsInArea(point) && ((point.Longitude - Start.Longitude) / (End.Longitude - Start.Longitude) ==
                                       (point.Latitude - Start.Latitude) / (End.Latitude - Start.Latitude));
        }

        //given a point that is in line (p1,p2)
        //check if a point is in this segment
        public bool IsInArea(Polypoint point)
        {
            //TODO : investigate this
            if ((point.Latitude == Start.Latitude && point.Longitude == Start.Longitude)
                || (point.Latitude == End.Latitude && point.Longitude == End.Longitude))
            {
                return true;
            }

            var minLat = Math.Min(Start.Latitude, End.Latitude);
            var maxLat = Math.Max(Start.Latitude, End.Latitude);
            var minLon = Math.Min(Start.Longitude, End.Longitude);
            var maxLon = Math.Max(Start.Longitude, End.Longitude);

            return (point.Longitude >= minLon &&
                    point.Longitude <= maxLon &&
                    point.Latitude >= minLat &&
                    point.Latitude <= maxLat);
        }

        //given a point that is in line (p1,p2)
        //check if that point is inside this segment
        //segment does not contain its end points
        public bool IsInArea(double lon, double lat)
        {
            return IsInArea(new Polypoint(lat, lon));
        }

        /// <summary>
        /// Test whether two line segments intersect. If so, calculate the intersection point.
        /// </summary>
        /// <param name="p">Polypoint to the start point of p.</param>
        /// <param name="p2">Polypoint to the end point of p.</param>
        /// <param name="q">Polypoint to the start point of q.</param>
        /// <param name="q2">Polypoint to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?</param>
        /// <returns>True if an intersection point was found.</returns>
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
                    if ((0 <= (seg.Start - Start) * r && (seg.Start - Start) * r <= r * r) || (0 <= (Start - seg.Start) * s && (Start - seg.Start) * s <= s * s))
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
            var i_isLeft = ((End.Latitude - Start.Latitude) * (p.Longitude - Start.Longitude) -
                            (End.Longitude - Start.Longitude) * (p.Latitude - Start.Latitude));
            if (i_isLeft > 0) // p is on the left
                return PointPosition.Left;
            else if (i_isLeft < 0)
                return PointPosition.Right;
            return PointPosition.OnLine;
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
                if (coordinates.All(s => double.TryParse(s, out var d)))
                {
                    //TODO :localization
                    return new Polysegment(
                        new Polypoint(double.Parse(coordinates[0]), double.Parse(coordinates[1])),
                        new Polypoint(double.Parse(coordinates[2]), double.Parse(coordinates[3])));
                }
            }

            return null;
        }
    }

    public enum PointPosition
    {
        OnLine = 0,
        Left = 1,
        Right = -1
    }
}