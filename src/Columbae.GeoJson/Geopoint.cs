using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace Columbae.GeoJson
{
    public class Geopoint : Polypoint
    {
        public Geopoint(double longitude, double latitude) : base(longitude, latitude)
        {
        }

        public override string ToString()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer,new Pointstring{type = "Point", coordinates = new [] {Longitude, Latitude}});
            return stringWriter.ToString();
        }

        public static Geopoint Parse(string json)
        {
            var geoJsonPoint = JsonConvert.DeserializeObject<Pointstring>(json);
            if (geoJsonPoint.type == "Point")
            {
                if (geoJsonPoint.coordinates != null)
                {
                    return new Geopoint(geoJsonPoint.coordinates[0], geoJsonPoint.coordinates[1]);
                }
            }

            return null;
        }

        private struct Pointstring
        {
            public string type { get; set; }
            public double[] coordinates { get; set; }
        }
    }
}