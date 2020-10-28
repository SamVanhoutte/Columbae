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
            var line = Geopolygon.Parse(input);
            Assert.NotNull(line);
            Assert.Equal(2, line.Points.Count);
            Assert.Equal(-33.4, line.Points[0].Y);
            Assert.Equal(-22.3, line.Points[1].X);
        }


        [Fact]
        public void GeoJsonPolygon_ToString_ShouldBeValidJson()
        {
            // Test good string
            var line = new Geopolygon(new List<Polypoint>
            {
                new Geopoint(22.3, -33.4),
                new Geopoint(-22.3, 33.4)
            });

            var json = line.ToString();

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