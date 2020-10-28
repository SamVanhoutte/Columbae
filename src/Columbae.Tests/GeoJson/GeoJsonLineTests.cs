using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Columbae.Tests.GeoJson
{
    public class GeoJsonLineTests
    {
        [Fact]
        public void GeoJsonLine_Parse_ShouldSucceed()
        {
            // Test good string
            var input = "{\"type\":\"LineString\",\"coordinates\":[[22.3,-33.4],[-22.3,33.4]]}";
            var line = Polyline.ParseJson(input);
            Assert.NotNull(line);
            Assert.Equal(2, line.Vertices.Count);
            Assert.Equal(-33.4, line.Vertices[0].Y);
            Assert.Equal(-22.3, line.Vertices[1].X);
        }


        [Fact]
        public void GeoJsonLine_ToString_ShouldBeValidJson()
        {
            // Test good string
            var line = new Polyline(new List<Polypoint>
            {
                new Polypoint(22.3, -33.4),
                new Polypoint(-22.3, 33.4)
            });

            var json = line.ToJson();

            var jObject = JObject.Parse(json);
            Assert.NotNull(jObject);
            Assert.NotNull(jObject["type"]);
            Assert.NotNull(jObject["coordinates"]);
            Assert.Equal("LineString", jObject["type"]);
            Assert.Equal(22.3, jObject["coordinates"][0][0]);
            Assert.Equal(33.4, jObject["coordinates"][1][1]);
        }
        
        [Theory]
        [InlineData(@"{lfuHgvjU[j@GR[f@c@`A_AbCwBrEOr@a@jDq@|DGj@")]
        public void GeoJson_Generate_ShouldWork(string polylineString)
        {
            // arrange
            var result = Polygon.ParsePolyline(polylineString);
            var geoPolyline = new Polyline(result.Vertices);
            
            // test
            var geoJson = geoPolyline.ToJson();
            
            // assert
            Assert.NotNull(geoJson);
        }
    }
}