using System;
using System.Collections.Generic;
using System.Linq;

namespace Columbae
{
    public class Polygon : Polyline, IShape, IEquatable<Polygon>
    {
        public Polygon() : base(new List<Polypoint>{})
        {
        }
        
        public Polygon(Polyline line) : base(line.Vertices)
        {
        }

        public Polygon(List<Polypoint> vertices) : base(vertices)
        {
        }

        public bool Equals(Polygon other)
        {
            return other != null && string.Equals(ToString(), other.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj is Polygon polygon)
            {
                return string.Equals(ToString(), polygon.ToString());
            }
            return false;
        }

        public override List<Polysegment> Sections => _sections ??= GetSections(true);
 

        
        // check if a point is inside this polygon or not
        public bool IsInside(Polypoint pt)
        {
            if (Vertices.Contains(pt)) return true;
            bool result = false;
            foreach (var polysegment in Sections)
            {
                var start = polysegment.Start;
                var end = polysegment.End;
                if ((start.Y < pt.Y && end.Y >= pt.Y) ||  // If point is between start/end of section
                    (end.Y < pt.Y && start.Y >= pt.Y))    // This includes Y values that are on the edge
                {
                    var position = polysegment.GetPointPositioning(pt);
                    if (position == PointPosition.OnLine)
                    {
                        return true;
                    }
                    if(position== PointPosition.Left)
                    {
                    // if (start.X + (pt.Y - start.Y) / (end.Y - start.Y) * (end.X - start.X) < pt.X)
                    // {
                        result = !result;
                    // }
                    }
                }
            }
            return result;
            int j = Size - 1;

            for (int i = 0; i < Vertices.Count(); i++)
            {
                if (Vertices[i].Y < pt.Y && Vertices[j].Y >= pt.Y || Vertices[j].Y < pt.Y && Vertices[i].Y >= pt.Y)
                {
                    if (Vertices[i].X + (pt.Y - Vertices[i].Y) / (Vertices[j].Y - Vertices[i].Y) * (Vertices[j].X - Vertices[i].X) < pt.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            
            
            // var coef = Vertices.Skip(1).Select((p, i) => 
            //         (point.Y - Vertices[i].Y)*(p.X - Vertices[i].X) 
            //         - (point.X - Vertices[i].X) * (p.Y - Vertices[i].Y))
            //     .ToList();
            //
            // if (coef.Any(p => p == 0))
            //     return true;
            //
            // for (var i = 1; i < coef.Count(); i++)
            // {
            //     if (coef[i] * coef[i - 1] < 0)
            //         return false;
            // }
            // return true;
            //
            
            
            // var j = Vertices.Count - 1;
            // var oddNodes = false;
            //
            // foreach (var polysegment in Sections)
            // {
            //     var ipt = polysegment.Start;
            //     var jpt = polysegment.End;
            //     
            //     if (ipt.X <= point.X && jpt.X >= point.X ||
            //         jpt.X <= point.X && ipt.X >= point.X)  //
            //     {
            //         
            //         if (ipt.Y +
            //             (point.X - ipt.X) / (jpt.X - ipt.X) *
            //             (jpt.Y - ipt.Y) <= point.Y)
            //         {
            //             oddNodes = !oddNodes;
            //         }
            //     }
            // }
            
            
            // for (var i = 0; i < Vertices.Count; i++)
            // {
            //     var ipt = Vertices[i];
            //     var jpt = Vertices[j];
            //     if (ipt.X <= point.X && jpt.X >= point.X ||
            //         jpt.X <= point.X && ipt.X >= point.X)  //
            //     {
            //         if (ipt.Y +
            //             (point.X - ipt.X) / (jpt.X - ipt.X) *
            //             (jpt.Y - ipt.Y) <= point.Y)
            //         {
            //             oddNodes = !oddNodes;
            //         }
            //     }
            //
            //     j = i;
            // }

            //return oddNodes;
        }

        // // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        // public bool Contains(Polyline s, double margin = 0, bool requiresDirection = false)
        // {
        //     for (var i = 0; i < s.Points.Count; i++)
        //     {
        //         // Takes 1 point and the next point
        //         // Modulus means the first point will be taken again for last vertex
        //         var p1 = s.Points[i];
        //         var p2 = s.Points[(i + 1) % s.Points.Count];
        //
        //         var edge = new Polysegment(p1, p2);
        //         if (Intersects(edge))
        //             return true;
        //     }
        //
        //     return false;
        // }
        
        // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        public bool Intersects(Polyline s)
        {
            return s.Sections.Any(Intersects);
        }

        public new bool Intersects(Polysegment s)
        {
            // Check if the line intersects the segment, or else if the polygon covers the segment
            return base.Intersects(s) || Covers(s);
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
            return IsInside(mid);
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

        public static Polygon ParseCsv(string polygonString)
        {
            var line = Polyline.ParseCsv(polygonString);
            if (line != null && line.Vertices.Count > 2) return new Polygon(line);
            return null;
        }
        
        public static Polygon ParsePolyline(string polyline)
        {
            var line = Polyline.ParsePolyline(polyline);
            if (line != null && line.Vertices.Count > 2) return new Polygon(line);
            return null;
        }
    }
}