using System.Collections.Generic;
using Columbae.GeoJson;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Columbae.Tests
{
    public class GeoPolygonTests
    {
        [Fact]
        public void GeoJsonPolygon_Parse_ShouldSucceed()
        {
            // Test good string
            var input = "{\"type\":\"Polygon\",\"coordinates\":[[22.3,-33.4],[-22.3,33.4]]}";
            var line = Polygon.ParseJson(input);
            Assert.NotNull(line);
            Assert.Equal(2, line.Vertices.Count);
            Assert.Equal(-33.4, line.Vertices[0].Y);
            Assert.Equal(-22.3, line.Vertices[1].X);
        }


        [Fact]
        public void GeoJsonPolygon_ToString_ShouldBeValidJson()
        {
            // Test good string
            var line = new Polygon(new List<Polypoint>
            {
                new Polypoint(22.3, -33.4),
                new Polypoint(-22.3, 33.4)
            });

            var json = line.ToJson();

            var jObject = JObject.Parse(json);
            Assert.NotNull(jObject);
            Assert.NotNull(jObject["type"]);
            Assert.NotNull(jObject["coordinates"]);
            Assert.Equal("Polygon", jObject["type"]);
            Assert.Equal(22.3, jObject["coordinates"][0][0]);
            Assert.Equal(33.4, jObject["coordinates"][1][1]);
        }
    }
}