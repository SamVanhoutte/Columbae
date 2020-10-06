using System;
using System.Collections.Generic;

namespace Columbae
{
    public class Polygon
    {
        private List<Polypoint> _vertices;

        // null constructor
        public Polygon()
        {
            _vertices = new List<Polypoint>();
        }

        // add a vertex
        public void AddVertex(double lon, double lat)
        {
            var point = new Polypoint(lat, lon);
            _vertices.Add(point);
        }

        // number of vertices
        private int Size => _vertices?.Count ?? 0;

        // check if a point is inside this polygon or not
        public bool Contains(Polypoint point)
        {
            var j = _vertices.Count - 1;
            var oddNodes = false;

            for (var i = 0; i < _vertices.Count; i++)
            {
                if (_vertices[i].Longitude < point.Longitude && _vertices[j].Longitude >= point.Longitude ||
                    _vertices[j].Longitude < point.Longitude && _vertices[i].Longitude >= point.Longitude)
                {
                    if (_vertices[i].Latitude +
                        (point.Longitude - _vertices[i].Longitude) / (_vertices[j].Longitude - _vertices[i].Longitude) *
                        (_vertices[j].Latitude - _vertices[i].Latitude) < point.Latitude)
                    {
                        oddNodes = !oddNodes;
                    }
                }

                j = i;
            }

            return oddNodes;
        }


        // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        public bool Intersects(Polysegment s)
        {
            // a triangle does not Intersect any segment of which end points are the triangle's vertices
            //if (size() == 3)
            //	return false;
            // polygon has more than 3 vertices		
            // split the big segment into parts
            Polypoint split_point = null;
            for (var i = 0; i < Size; i++)
            {
                var p1 = _vertices[i];
                var p2 = _vertices[(i + 1) % Size];
                var edge = new Polysegment(p1, p2);
                split_point = s.InterSectionExceptThisEnds(edge);
                if (split_point != null)
                    break;
            }

            // if we can split
            if (split_point != null) // then check each part
            {
                var first_part = Intersects(new Polysegment(s.Start, split_point));
                if (first_part == true) // a part intersects means whole segment intersects
                    return first_part;
                // if first part doesn't intersect
                // it depends on second one
                var second_part = Intersects(new Polysegment(split_point, s.End));
                return second_part;
            }
            // cannot split this segment
            else
            {
                var result = Covers(s);
                return result;
            }
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
                if (_vertices[i].Equals(p))
                    return i;
            return -1;
        }

        // check if a point is one of this polygon vertices
        public bool IsVertex(Polypoint p)
        {
            for (var i = 0; i < Size; i++)
                if (_vertices[i].Equals(p))
                    return true;
            return false;
        }
    }
}