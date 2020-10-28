using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Columbae.GeoJson
{
    public class Geoline : Polyline
    {
        public Geoline(List<Polypoint> vertices) : base(vertices)
        {
        }

        public override string ToString()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer, new Linestring()
            {
                type = "LineString",
                coordinates = Vertices.Select(pt => new[] {pt.X, pt.Y}).ToArray()
            });
            return stringWriter.ToString();
        }

        public static Geoline Parse(string json, string geoType = "LineString")
        {
            var points = new List<Polypoint>();
            var geoJsonLine = JsonConvert.DeserializeObject<Linestring>(json);
            if (geoJsonLine.type == geoType)
            {
                if (geoJsonLine.coordinates != null)
                {
                    points = geoJsonLine.coordinates.Select(c => new Polypoint(c[0], c[1])).ToList();
                    return new Geoline(points);
                }
            }

            return null;
        }

        public static Geoline Parse(Stream json, string geoType = "LineString")
        {
            var points = new List<Polypoint>();
            using var reader = new StreamReader(json);
            return Parse(reader.ReadToEnd(), geoType);
        }
        public bool Contains(Polyline section)
        {
            return IntersectsWith(section);
        }
        
        private struct Linestring
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }
    }
}