using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Columbae
{
    public class Polyline : IShape
    {
        public List<Polypoint> Vertices { get; private set; }

        public Polyline()
        {
        }

        public Polyline(List<Polypoint> vertices)
        {
            Vertices = vertices;
        }

        public Polysegment FullSegment => new Polysegment(Vertices.First(), Vertices.Last());

        // number of vertices
        protected int Size => Vertices?.Count ?? 0;

        public override bool Equals(object obj)
        {
            if (obj is Polyline polyline)
            {
                return string.Equals(ToString(), polyline.ToString());
            }

            return false;
        }

        // add a Point
        public void AddVertex(double lon, double lat)
        {
            AddVertex(new Polypoint(lon, lat));
        }

        // add a vertex
        public void AddVertex(Polypoint point)
        {
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

        public override string ToString()
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

        public bool Contains(Polyline sequence, double safetyMargin = 0.0D)
        {
            // First check if the start/endpoint intersect with the boundingbox.  (limiting number of points to check
            // In case the boundingbox of the line does not contain the actual start/end, we return false immediately
            if (BoundingBox.Intersects(sequence.FullSegment))
            {
                // Now we loop all points of the sequence and check if they are in the given boundaries of the polygon
                return sequence.Vertices.All(sequencePoint => Contains(sequencePoint, safetyMargin));
            }

            return false;
        }

        public bool Contains(Polypoint point, double margin = 0.0D)
        {
            return Sections.Any(section => section.Contains(point, margin));
        }

        internal List<Polysegment> GetSections(bool closePolygon)
        {
            var sections = new List<Polysegment> { };
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

        public List<Polysegment> Sections => GetSections(false);

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

        public static Polyline ParseCsv(string csvString)
        {
            // Format should be of X1,Y1,X2,X3,Y3,X4,Y4
            var line = new Polyline();
            var coordinates = csvString.Split(',');
            if (coordinates.Length % 2 == 0)
            {
                if (coordinates.All(s => double.TryParse(s, out var d)))
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