using System.Collections.Generic;
using System.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Columbae.GeoJson
{
    public class Geopolygon : Polygon
    {
        private Geoline _geoline;

        public Geopolygon(List<Polypoint> points) :
            base(points)
        {
            _geoline = new Geoline(points);
        }

        public List<Polypoint> Points => Vertices;


        public override string ToString()
        {
            return JsonSerializer.Serialize(new Linestring()
            {
                type = "Polygon",
                coordinates = Vertices.Select(pt => new[] {pt.Longitude, pt.Latitude}).ToArray()
            });
        }

        public static Geopolygon Parse(string json)
        {
            var line = Geoline.Parse(json, "Polygon");
            return line != null ? new Geopolygon(line.Points) : null;
        }

        private struct Linestring
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }
    }
}