using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Columbae.GeoJson
{
    public class Geoline : Polyline
    {
        public Geoline(List<Polypoint> points) : base(points)
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
                coordinates = Points.Select(pt => new[] {pt.Longitude, pt.Latitude}).ToArray()
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
        
        private struct Linestring
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }
    }
}