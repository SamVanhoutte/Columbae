using Newtonsoft.Json.Linq;
using Xunit;

namespace Columbae.Tests.GeoJson
{
    public class GeoJsonPointTests
    {

        [Fact]
        public void GeoJsonPoint_Parse_ShouldSucceed()
        {
            // Test good string
            var input = "{\n\"type\":\"Point\",\n\"coordinates\":[ 31.9, -4.8 ]\n}";
            var point = Polypoint.ParseJson(input);
            Assert.NotNull(point);
            Assert.Equal(31.9, point.X);
            Assert.Equal(-4.8, point.Y);
        }
        
        
        [Fact]
        public void GeoJsonPoint_ToString_ShouldBeValidJson()
        {
            // Test good string
            var point = new Polypoint(22.3, -33.4);

            var json = point.ToJson();

            var jObject = JObject.Parse(json);
            Assert.NotNull(jObject);
            Assert.NotNull(jObject["type"]);
            Assert.NotNull(jObject["coordinates"]);
            Assert.Equal("Point", jObject["type"]);
            Assert.Equal(22.3, jObject["coordinates"][0]);
            Assert.Equal(-33.4, jObject["coordinates"][1]);
        }

        
        
        [Fact]
        public void GeoJsonPoint_ParseIncorrect_ShouldReturnNull()
        {
            // Test linestring
            var input = "{\n\"type\":\"LinString\",\n\"coordinates\":[ 31.9, -4.8 ]\n}";
            var point = Polypoint.ParseJson(input);
            Assert.Null(point);

            // Test non numeric string
            input = "{\n\"type2\":\"LinString\",\n\"coordinates\":[ 31.9, -4.8 ]\n}";
            point = Polypoint.ParseJson(input);
            Assert.Null(point);

            // Test missing values
            input = "{\n\"type\":\"LinString\",\n\"coordinate\":[ 31.9, -4.8 ]\n}";
            point = Polypoint.ParseJson(input);
            Assert.Null(point);
        }
    }
}