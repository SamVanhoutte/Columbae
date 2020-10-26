using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Columbae.GeoJson
{
    public class Line : Polyline
    {
        public Line(List<Point> points) :
            base(points.Select(p => new Polypoint(p.Longitude, p.Latitude)).ToList())
        {
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(new GeoLine()
            {
                type = "LineString",
                coordinates = Points.Select(pt => new[] {pt.Longitude, pt.Latitude}).ToArray()
            });
        }

        public static Line Parse(string json)
        {
            var points = new List<Point>();
            var geoJsonLine = JsonConvert.DeserializeObject<GeoLine>(json);
            if (geoJsonLine.type == "LineString")
            {
                if (geoJsonLine.coordinates != null)
                {
                    points = geoJsonLine.coordinates.Select(c => new Point(c[0], c[1])).ToList();
                    return new Line(points);
                }
            }

            return null;
        }
        
        private struct GeoLine
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }
    }
}