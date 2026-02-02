using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Columbae.GeoJson;
using Newtonsoft.Json;

namespace Columbae
{
    public class Polygon : Polyline, IEquatable<Polygon>
    {
        public Polygon() : base(new List<Polypoint>())
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
            return other != null && string.Equals(ToPolylineString(), other.ToPolylineString());
        }

        public override bool Equals(object obj)
        {
            if (obj is Polygon polygon)
            {
                return string.Equals(ToPolylineString(), polygon.ToPolylineString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToPolylineString().GetHashCode();
        }

        public override List<Polysegment> Sections => CachedSections ??= GetSections(true);

        // check if a point is inside this polygon or not
        public bool IsInside(Polypoint pt)
        {
            if (Vertices.Contains(pt)) return true;
            var result = false;
            foreach (var polysegment in Sections)
            {
                var start = polysegment.Start;
                var end = polysegment.End;
                var position = polysegment.GetPointPositioning(pt);
                if (position == PointPosition.OnLine)
                {
                    return true;
                }

                if ((start.Y < pt.Y && end.Y >= pt.Y) || // If point is between start/end of section
                    (end.Y < pt.Y && start.Y >= pt.Y)) // This includes Y values that are on the edge
                {
                    if (position == PointPosition.Left)
                    {
                        result = !result;
                    }
                }
            }

            return result;

        }

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
            var p1Pos = IndexOf(s.Start);
            var p2Pos = IndexOf(s.End);
            if (p1Pos != -1 && p2Pos != -1)
            {
                // If both points are vertex of polygon, we consider it covered
                return true;
            }

            // segment is unseparatable
            // so,if the mid point is inside polygon, whole segment will inside
            var mid = s.MidPoint();
            bool inside = IsInside(mid);
            return inside;
        }

        public bool Covers(Polyline line)
        {
            return line.Sections.All(Covers);
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
            return Vertices.Contains(p);
        }

        public new static Polygon ParseCsv(string polygonString)
        {
            var line = Polyline.ParseCsv(polygonString);
            if (line != null && line.Vertices.Count > 2) return new Polygon(line);
            return null;
        }
        
        public new string ToJson()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer,new Linestring()
            {
                Type = "Polygon",
                Coordinates = Vertices.Select(pt => new[] {pt.X, pt.Y}).ToArray()
            });
            return stringWriter.ToString();
        }

        public static Polygon ParseJson(string json)
        {
            var line = Polyline.ParseJson(json, "Polygon");
            return line != null ? new Polygon(line) : null;
        }



        public new static Polygon ParsePolyline(string polyline)
        {
            var line = Polyline.ParsePolyline(polyline);
            if (line != null && line.Vertices.Count > 2) return new Polygon(line);
            return null;
        }

        public double[][] ToArray()
        {
            return Vertices.Select(v => v.ToArray()).ToArray();
        }
        

    }
}