using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Columbae.GeoJson;
using Newtonsoft.Json;

namespace Columbae
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Polyline : IShape, IEquatable<Polyline>
    {
        [JsonProperty("vertices")]
        public List<Polypoint> Vertices { get; private set; }
        protected List<Polysegment> CachedSections;

        public Polyline()
        {
        }

        public Polyline(List<Polypoint> vertices)
        {
            Vertices = vertices;
        }

        public Polysegment EndToEndSegment => new Polysegment(Vertices.First(), Vertices.Last());

        protected int Size => Vertices?.Count ?? 0;

        public bool Equals(Polyline other)
        {
            return other != null && string.Equals(ToPolylineString(), other.ToPolylineString());
        }

        public override bool Equals(object obj)
        {
            if (obj is Polyline polyline)
            {
                return string.Equals(ToPolylineString(), polyline.ToPolylineString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToPolylineString().GetHashCode();
        }

        // add a Point
        public void AddVertex(double lon, double lat)
        {
            CachedSections = null;
            AddVertex(new Polypoint(lon, lat));
        }

        // add a vertex
        public void AddVertex(Polypoint point)
        {
            CachedSections = null;
            Vertices ??= new List<Polypoint>();
            Vertices.Add(point);
        }


        public Polygon BoundingBox
        {
            get
            {
                var minX = Vertices.Min(pt => pt.X);
                var maxX = Vertices.Max(pt => pt.X);
                var minY = Vertices.Min(pt => pt.Y);
                var maxY = Vertices.Max(pt => pt.Y);
                return new Polygon
                (new List<Polypoint>
                {
                    new Polypoint(minX, maxY),
                    new Polypoint(maxX, maxY),
                    new Polypoint(maxX, minY),
                    new Polypoint(minX, minY)
                });
            }
        }

        // check if a part of the segment, of which 2 end points are the polygon's vertices, is inside this or not
        public bool Intersects(Polysegment s)
        {
            return Sections.Any(edge => s.Intersects(edge, out _));
        }


        public bool IntersectsWith(Polyline matchingPolyline)
        {
            return false;
        }

        private static void EncodeNextCoordinate(long coordinate, StringBuilder result)
        {
            coordinate = coordinate < 0 ? ~(coordinate << 1) : coordinate << 1;

            while (coordinate >= 0x20)
            {
                result.Append((char) ((int) ((0x20 | (coordinate & 0x1f)) + 63)));
                coordinate >>= 5;
            }

            result.Append((char) ((int) (coordinate + 63)));
        }

        private static int DecodeNextCoordinate(string polyline, ref int polylineIndex)
        {
            var result = 1;
            var shift = 0;
            int bit;

            do
            {
                bit = polyline[polylineIndex++] - 63 - 1;
                result += bit << shift;
                shift += 5;
            } while (bit >= 0x1f); // && polylineIndex < polyline.Length - 1); //TODO  

            return (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
        }

        public bool Contains(Polyline sequence, double safetyMargin = 0.0D, bool verifyDirection = false)
        {
            // First check if the start/endpoint intersect with the boundingbox.  (limiting number of points to check
            // In case the boundingbox of the line does not contain the actual start/end, we return false immediately
            if (BoundingBox.Intersects(sequence.EndToEndSegment))
            {
                var segmentSequence = new List<Polysegment>();
                // Now we loop all points of the sequence and check if they are in the given boundaries of the polygon
                foreach (var sequencePoint in sequence.Vertices)
                {
                    if (!Contains(sequencePoint, out _, out var containingSegment, safetyMargin))
                    {
                        return false;
                    }
                    segmentSequence.Add(containingSegment);
                }
                if (verifyDirection)
                {
                    var previousIndex = -1;
                    foreach (var currentIndex in segmentSequence.Select(polysegment => Vertices.IndexOf(polysegment.Start)))
                    {
                        if (currentIndex < previousIndex)
                        {
                            return false;
                        }
                        previousIndex = currentIndex;
                    }
                    return true;
                }

                return true;
            }
            return false;
        }

        public bool Contains(Polypoint point,  double margin = 0.0D)
        {
            return Contains(point, out _, out _, margin);
        }
        public bool Contains(Polypoint point, out Polypoint closest, double margin = 0.0D)
        {
            return Contains(point, out closest, out _, margin);
        }
        
        public bool Contains(Polypoint point, out Polypoint closest, out Polysegment containingSegment, double margin = 0.0D)
        {
            closest = null;
            containingSegment = null;
            foreach (var section in Sections)
            {
                if (section.Contains(point, out closest, margin))
                {
                    containingSegment = section;
                    return true;
                }
            }

            return false;
        }

        internal List<Polysegment> GetSections(bool closePolygon)
        {
            var sections = new List<Polysegment>();
            // Loop through all points and create segment
            for (var pointIndx = 0; pointIndx < Vertices.Count - 1; pointIndx++)
            {
                sections.Add(new Polysegment(Vertices[pointIndx], Vertices[pointIndx + 1]));
            }

            if (closePolygon)
            {
                sections.Add(new Polysegment(Vertices.Last(), Vertices.First()));
            }

            return sections;
        }

        public virtual List<Polysegment> Sections => CachedSections ??= GetSections(false);

        public string ToPolylineString()
        {
            var result = new StringBuilder();
            long lastLatitude = 0L, lastLongitude = 0L;

            foreach (var polylinePoint in Vertices)
            {
                var longitude = (long) Math.Round(polylinePoint.X * 1e5);
                var latitude = (long) Math.Round(polylinePoint.Y * 1e5);

                EncodeNextCoordinate(latitude - lastLatitude, result);
                EncodeNextCoordinate(longitude - lastLongitude, result);

                lastLatitude = latitude;
                lastLongitude = longitude;
            }

            return result.ToString();
        }

        public string ToJson()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer, new Linestring()
            {
                Type = "LineString",
                Coordinates = Vertices.Select(pt => new[] {pt.X, pt.Y}).ToArray()
            });
            return stringWriter.ToString();
        }

        public static Polyline ParsePolyline(string polyline)
        {
            var vertices = new List<Polypoint>(polyline.Length / 2);

            for (int idx = 0, latitude = 0, longitude = 0; idx < polyline.Length;)
            {
                latitude += DecodeNextCoordinate(polyline, ref idx);
                longitude += DecodeNextCoordinate(polyline, ref idx);

                vertices.Add(new Polypoint(longitude * 1e-5, latitude * 1e-5));
            }

            return new Polyline(vertices);
        }

        public static Polyline ParseJson(string json, string geoType = "LineString")
        {
            var points = new List<Polypoint>();
            var geoJsonLine = JsonConvert.DeserializeObject<Linestring>(json);
            if (geoJsonLine.Type == geoType)
            {
                if (geoJsonLine.Coordinates != null)
                {
                    points = geoJsonLine.Coordinates.Select(c => new Polypoint(c[0], c[1])).ToList();
                    return new Polyline(points);
                }
            }

            return null;
        }

        public static Polyline ParseCsv(string csvString)
        {
            // Format should be of X1,Y1,X2,X3,Y3,X4,Y4
            var line = new Polyline();
            var coordinates = csvString.Split(',');
            if (coordinates.Length % 2 == 0)
            {
                if (coordinates.All(s => double.TryParse(s, out _)))
                {
                    for (var pointIdx = 0; pointIdx < coordinates.Length / 2; pointIdx++)
                    {
                        line.AddVertex(
                            double.Parse(coordinates[pointIdx * 2]),
                            double.Parse(coordinates[pointIdx * 2 + 1]));
                    }

                    return line;
                }
            }

            return null;
        }
    }
}