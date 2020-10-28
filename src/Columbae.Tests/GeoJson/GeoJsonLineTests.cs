using System.Collections.Generic;
using Columbae.GeoJson;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Columbae.Tests
{
    public class GeoJsonLineTests
    {
        [Fact]
        public void GeoJsonLine_Parse_ShouldSucceed()
        {
            // Test good string
            var input = "{\"type\":\"LineString\",\"coordinates\":[[22.3,-33.4],[-22.3,33.4]]}";
            var line = Geoline.Parse(input);
            Assert.NotNull(line);
            Assert.Equal(2, line.Points.Count);
            Assert.Equal(-33.4, line.Points[0].Y);
            Assert.Equal(-22.3, line.Points[1].X);
        }


        [Fact]
        public void GeoJsonLine_ToString_ShouldBeValidJson()
        {
            // Test good string
            var line = new Geoline(new List<Polypoint>
            {
                new Geopoint(22.3, -33.4),
                new Geopoint(-22.3, 33.4)
            });

            var json = line.ToString();

            var jObject = JObject.Parse(json);
            Assert.NotNull(jObject);
            Assert.NotNull(jObject["type"]);
            Assert.NotNull(jObject["coordinates"]);
            Assert.Equal("LineString", jObject["type"]);
            Assert.Equal(22.3, jObject["coordinates"][0][0]);
            Assert.Equal(33.4, jObject["coordinates"][1][1]);
        }

        [Fact]
        public void GeoJsonLine_ParsePolyline_ShouldWork()
        {
            var polylineString = "adeuHqjlUo@i@u@e@uAu@u@]sB]g@EoCe@MA_@BsA^_@Pe@DgBw@aAYkAk@}A[QBaAn@m@R";
            var polyline = new Polyline(polylineString);
            var geoPolyline = new Geoline(polyline.Points);
            var outputString = geoPolyline.ToString();
            Assert.NotNull(outputString);
        }

        [Fact]
        public void GeoJsonLine_ParseIncorrect_ShouldReturnNull()
        {
            // Test linestring
            var input = "{\n\"type\":\"LineString\",\n\"coordinates\":[ 31.9, -4.8 ]\n}";
            var point = Geopoint.Parse(input);
            Assert.Null(point);

            // Test non numeric string
            input = "{\n\"type2\":\"LinString\",\n\"coordinates\":[ 31.9, -4.8 ]\n}";
            point = Geopoint.Parse(input);
            Assert.Null(point);

            // Test missing values
            input = "{\n\"type\":\"LinString\",\n\"coordinate\":[ 31.9, -4.8 ]\n}";
            point = Geopoint.Parse(input);
            Assert.Null(point);
        }
        
        
        [Theory]
        [InlineData(@"{lfuHgvjU[j@GR[f@c@`A_AbCwBrEOr@a@jDq@|DGj@")]
        public void GeoJson_Generate_ShouldWork(string polylineString)
        {
            // arrange
            var result = new Polyline(polylineString);
            var geoPolyline = new Geoline(result.Points);
            
            // test
            var geoJson = geoPolyline.ToString();
            
            // assert
            Assert.NotNull(geoJson);
        }
    }
}