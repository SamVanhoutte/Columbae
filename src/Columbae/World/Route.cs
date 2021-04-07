using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Columbae.World.Gpx;
using Newtonsoft.Json;

namespace Columbae.World
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Route : Polyline
    {
        [JsonProperty("name")] public string Name { get; set; }

        public Route(List<Polypoint> points) : base(points)
        {
        }

        public async Task<string> ExportGpx()
        {
            await using var memStream = new MemoryStream();
            await ExportGpx(memStream);
            memStream.Position = 0;
            return Encoding.UTF8.GetString(memStream.ToArray());
        }

        public async Task ExportGpx(string fileName)
        {
            await using var fileStream =
                new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await ExportGpx(fileStream);
        }

        public async Task ExportGpx(Stream stream)
        {
            var streamWriter = new StreamWriter(stream);
            await ExportGpx(streamWriter);
        }

        public Task ExportGpx(StreamWriter writer)
        {
            GenerateGpx(writer);
            return Task.CompletedTask;
        }


        private void GenerateGpx(StreamWriter outputWriter)
        {
            var route = new GpxRoute
            {
                Metadata = new Metadata
                {
                    Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                Trk = new List<Trk>{new Trk
                {
                    Name = Name, Type = "1", Trkseg = new Trkseg
                    {
                        Trkpt = Vertices
                            .Select(polypoint => new Trkpt {Ele = 0.0, Lat = polypoint.Y, Lon = polypoint.X}).ToList()
                    }
                }}
            };
            var serializer = new XmlSerializer(typeof(GpxRoute));
            var writer = XmlWriter.Create(outputWriter);
            serializer.Serialize(writer, route);
            // var serializer = new DataContractSerializer(typeof(GpxRoute)); 
            // var writer = XmlWriter.Create(outputWriter);
            // serializer.WriteObject(writer, route);
        }

        public static IEnumerable<Route> ImportGpx(StreamReader gpxReader)
        {
            var serializer = new XmlSerializer(typeof(GpxRoute));
            var writer = XmlReader.Create(gpxReader);
            var deserializedObject = serializer.Deserialize(writer);
            var routes = new List<Route> { };
            if (deserializedObject is GpxRoute route)
            {
                foreach (var trk in route.Trk)
                {
                    var points = trk.Trkseg.Trkpt.Select(wayPoint =>
                        new Polypoint(wayPoint.Lon, wayPoint.Lat)).ToList();
                    routes.Add(new Route(points){Name = trk.Name});
                }

                return routes;
            }

            return null;
        }
    }
}