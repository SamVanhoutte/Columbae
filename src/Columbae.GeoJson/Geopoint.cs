using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Columbae.GeoJson
{
    public class Geopoint : Polypoint
    {
        public Geopoint(double longitude, double latitude) : base(longitude, latitude)
        {
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(new Pointstring{type = "Point", coordinates = new [] {Longitude, Latitude}});
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