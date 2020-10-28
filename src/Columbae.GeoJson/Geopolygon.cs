using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


namespace Columbae.GeoJson
{
    public class Geopolygon : Polygon
    {
        private Geoline _geoline;

        public Geopolygon(List<Polypoint> vertices) :
            base(vertices)
        {
            _geoline = new Geoline(vertices);
        }

        public List<Polypoint> Points => Vertices;


        public override string ToString()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer,new Linestring()
            {
                type = "Polygon",
                coordinates = Vertices.Select(pt => new[] {pt.X, pt.Y}).ToArray()
            });
            return stringWriter.ToString();
        }

        public new static Geopolygon Parse(string json)
        {
            var line = Geoline.Parse(json, "Polygon");
            return line != null ? new Geopolygon(line.Vertices) : null;
        }

        private struct Linestring
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }
    }
}