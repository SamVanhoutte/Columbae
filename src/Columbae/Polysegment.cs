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

        //find a intersection of this segment with another segment
        //check equal before whenever use this method
        public Polypoint InterSection(Polysegment seg)
        {
            if (Start.Equals(seg.Start))
                return Start;
            if (End.Equals(seg.End))
                return End;
            // last point of this segment is first point of given segment
            if (End.Equals(seg.Start))
                return End;
            // first point of this segment is last point of given segment
            if (Start.Equals(seg.End))
                return Start;
            // find the intersection of 2 line (p1,p2) and (s.p1, s.p2)
            var vy1 = End.Latitude - Start.Latitude;
            var vx1 = End.Longitude - Start.Longitude;
            var vy2 = seg.End.Latitude - seg.Start.Latitude;
            var vx2 = seg.End.Longitude - seg.Start.Longitude;

            var t = (
                        vy1 * (seg.Start.Longitude - Start.Longitude)
                        - vx1 * (seg.Start.Latitude - Start.Latitude)
                    ) /
                    (vy2 * vx1 - vx2 * vy1);
            var lon = (int) (vx2 * t + seg.Start.Longitude);
            var lat = (int) (vy2 * t + seg.Start.Latitude);
            // check if the intersection inside this segment
            if (IsInArea(lon, lat) && seg.IsInArea(lon, lat))
                return new Polypoint(lat, lon);
            return null;
        }

        //find a inside intersection of this segment with another segment
        //check equal before whenever use this method
        public Polypoint InterSectionExceptThisEnds(Polysegment s)
        {
            if (Start.Equals(s.Start))
                return null;
            if (End.Equals(s.End))
                return null;
            // last point of this segment is first point of given segment
            if (End.Equals(s.Start))
                return null;
            // first point of this segment is last point of given segment
            if (Start.Equals(s.End))
                return null;
            return InterSection(s);
        }

        // check if a point is on the left of this segment or not
        // 0 if point is on line
        // 1 for left
        // -1 for right
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