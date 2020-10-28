using System.Drawing;
using System.Linq;
using Xunit;
namespace Columbae.Tests
{
    public class PolylineTests
    {
        [Theory]
        [InlineData("0,1,0,4,3,4,3,1", "0,2,4,3")] // Start & stop outside, but crossing
        [InlineData("0,0,0,4,3,4,3,1", "-1,-2,1,3")] // Start outside, end inside
        [InlineData("0,1,0,4,3,4,3,1", "1,2,6,3")] // Start inside, end outside
        public void Polyline_Intersects_ShouldMatch(string polylineStr, string segmentToMatch)
        {
            // arrange
            var line = Polyline.ParseCsv(polylineStr);
            var matchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = line.Intersects(matchingSegment);

            // assert
            Assert.True(intersects);
        }

        [Theory]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", "2,2,3,3")]
        [InlineData("0,0,2,0,2,2,0,2", "-1,1,1,1")]
        [InlineData("0,1,0,4,3,4,3,1", "0,0,1,3")] // Start outside, end inside
        [InlineData("-1,-1, -4,-1, -4,-1, -4,-4", "-2,-2,-3,-3")] // Crossing
        public void Polyline_Intersects_ShouldNotMatch(string polylineStr, string segmentToMatch)
        {
            // arrange
            var line = Polyline.ParseCsv(polylineStr);
            var notMatchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = line.Intersects(notMatchingSegment);

            // assert
            Assert.False(intersects);
        }

        [Theory]
        [InlineData("0,0,2,0,2,2,0,2", "2,2,0,2", 3)]
        [InlineData("0,0,2,2,2,0,0,2", "2,2,2,0", 3)]
        public void Polyline_Sections_Shouldmatch(string polylineStr, string expectedSection, int expectedSections)
        {
            // arrange
            var line = Polyline.ParseCsv(polylineStr);
            var expectedSegment = Polysegment.Parse(expectedSection);
            
            // act
            var sections = line.Sections;
            
            // assert
            Assert.Equal(expectedSections, sections.Count);
            Assert.Contains(expectedSegment, sections);
        }

        [Theory]
        [InlineData("0,1, -3,1, -1,-1.5, 3,2.5, -2,3, 2,-1, -1,2", "-3,3,3,3,3,-1.5,-3,-1.5")]
        public void Polyline_Boundingbox_Shouldmatch(string polylineStr, string expectedBox)
        {
            // arrange
            var line = Polyline.ParseCsv(polylineStr);
            var expectedMatch = Polygon.ParseCsv(expectedBox);

            // act
            var box = line.BoundingBox;

            // assert
            Assert.Equal(expectedMatch, box);
        }

        [Theory]
        [InlineData("0,0,0,4,1,3,4,4", 4, 4, 0)]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", 4, -4, -1)]
        [InlineData("0,1, -3,1, -1,-1.5, 3,2.5, -2,3, 2,-1, -1,2",7, -1, 1)]
        public void Polygon_Parse_ShouldMatch(string polylinestr, int expectedVertices, double lastLongitude, double firstLatitude)
        {
            // arrange
            var line = Polyline.ParseCsv(polylinestr);

            // assert
            Assert.NotNull(line);
            Assert.Equal(expectedVertices, line.Vertices.Count);
            Assert.Equal(lastLongitude, line.Vertices.Last().X);
            Assert.Equal(firstLatitude, line.Vertices.First().Y);
        }


        [Theory]
        [InlineData("0,0,0,4,1,3,4,6,7")]
        [InlineData("0,0,0,4,1,3,d,6,7")]
        public void Polyline_Parse_IncorrectValues_ShouldBeNull(string polylineStr)
        {
            // arrange
            var line = Polyline.ParseCsv(polylineStr);

            // assert
            Assert.Null(line);
        }
    }
}
