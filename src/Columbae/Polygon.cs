using System;
using System.Collections.Generic;
using System.Linq;

namespace Columbae
{
    public class Polygon
    {
        public readonly List<Polypoint> Vertices;

        // null constructor
        public Polygon()
        {
            Vertices = new List<Polypoint>();
        }

        public Polygon(List<Polypoint> points)
        {
            Vertices = points;
        }

        public Polygon(string polyLineString)
        {
            var polyline = new Polyline(polyLineString);
            Vertices = new List<Polypoint>();
            foreach (var polypoint in polyline.Points)
            {
                AddVertex(polypoint);
            }
        }

        // add a vertex
        public void AddVertex(double lon, double lat)
        {
            AddVertex(new Polypoint(lat, lon));
        }

        // add a vertex
        public void AddVertex(Polypoint point)
        {
            Vertices.Add(point);
        }

        // public List<Polysegment> Segments
        // {
        //     get
        //     {
        //         
        //     }
        // }
        //
        // number of vertices
        private int Size => Vertices?.Count ?? 0;


        // check if a point is inside this polygon or not
        public bool Contains(Polypoint point)
        {
            var j = Vertices.Count - 1;
            var oddNodes = false;

            for (var i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].Longitude < point.Longitude && Vertices[j].Longitude >= point.Longitude ||
                    Vertices[j].Longitude < point.Longitude && Vertices[i].Longitude >= point.Longitude)
                {
                    if (Vertices[i].Latitude +
                        (point.Longitude - Vertices[i].Longitude) / (Vertices[j].Longitude - Vertices[i].Longitude) *
                        (Vertices[j].Latitude - Vertices[i].Latitude) < point.Latitude)
                    {
                        oddNodes = !oddNodes;
                    }
                }

                j = i;
            }

            return oddNodes;
        }

        // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        public bool Intersects(Polyline s)
        {
            for (var i = 0; i < s.Points.Count; i++)
            {
                // Takes 1 point and the next point
                // Modulus means the first point will be taken again for last vertex
                var p1 = s.Points[i];
                var p2 = s.Points[(i + 1) % s.Points.Count];

                var edge = new Polysegment(p1, p2);
                if (Intersects(edge))
                    return true;
            }

            return false;
        }

        // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        public bool Intersects(Polysegment s)
        {
            Polypoint split_point = null;
            for (var i = 0; i < Size; i++)
            {
                // Takes 1 point and the next point
                // Modulus means the first point will be taken again for last vertex
                var p1 = Vertices[i];
                var p2 = Vertices[(i + 1) % Size];

                var edge = new Polysegment(p1, p2);
                if (s.Intersects(edge, out split_point))
                    return true;
            }

            //TODO : do we still need this?
            // if we can split
            // if (split_point != null) // then check each part
            // {
            //     var first_part = Intersects(new Polysegment(s.Start, split_point));
            //     if (first_part == true) // a part intersects means whole segment intersects
            //         return first_part;
            //     // if first part doesn't intersect
            //     // it depends on second one
            //     var second_part = Intersects(new Polysegment(split_point, s.End));
            //     return second_part;
            // }
            // // cannot split this segment
            // else
            // {
            var result = Covers(s);
            return result;
            // }
        }

        public bool Covers(Polysegment s)
        {
            // if segment is a edge of this polygon
            var p1_pos = IndexOf(s.Start);
            var p2_pos = IndexOf(s.End);
            if (p1_pos != -1 && p2_pos != -1)
            {
                var pos_distance = Math.Abs(p1_pos - p2_pos);
                if (pos_distance == 1 || pos_distance == Size - 1) // adjcent vertices
                    return false;
            }

            // segment is unseparatable
            // so,if the mid point is inside polygon, whole segment will inside
            var mid = s.MidPoint();
            return Contains(mid);
        }

        // index of vertice
        private int IndexOf(Polypoint p)
        {
            for (var i = 0; i < Size; i++)
                if (Vertices[i].Equals(p))
                    return i;
            return -1;
        }

        // check if a point is one of this polygon vertices
        public bool IsVertex(Polypoint p)
        {
            for (var i = 0; i < Size; i++)
                if (Vertices[i].Equals(p))
                    return true;
            return false;
        }

        public static Polygon Parse(string polygonString)
        {
            // Format should be of X1,Y1,X2,X3,Y3,X4,Y4
            var polygon = new Polygon();
            var coordinates = polygonString.Split(',');
            if (coordinates.Length >= 6 && coordinates.Length % 2 == 0) // minimum of 3 points needed
            {
                if (coordinates.All(s => double.TryParse(s, out var d)))
                {
                    for (var pointIdx = 0; pointIdx < coordinates.Length / 2; pointIdx++)
                    {
                        polygon.AddVertex(
                            double.Parse(coordinates[pointIdx * 2]),
                            double.Parse(coordinates[pointIdx * 2 + 1]));
                    }

                    return polygon;
                }
            }

            return null;
        }
    }
}