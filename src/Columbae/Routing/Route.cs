using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Columbae.Routing.Gpx;

namespace Columbae.Routing
{
    public class Route
    {
        private List<Polypoint> _points;
        public string Name { get; set; }
        
        public Route(List<Polypoint> points)
        {
            _points = points;
        }

        public Task ExportGpx(string fileName)
        {
            using var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var streamWriter = new StreamWriter(fileStream);
            GenerateGpx(streamWriter);
            return Task.CompletedTask;
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
                Trk = new Trk
                {
                    Name = Name, Type = "1", Trkseg = new Trkseg
                    {
                        Trkpt = _points.Select(polypoint => new Trkpt {Ele = polypoint.Elevation, Lat = polypoint.Latitude, Lon = polypoint.Longitude}).ToList()
                    }
                }
            };
            var serializer = new XmlSerializer(typeof(GpxRoute));
            var writer = XmlWriter.Create(outputWriter);
            serializer.Serialize(writer, route);
        }
    }
    
}