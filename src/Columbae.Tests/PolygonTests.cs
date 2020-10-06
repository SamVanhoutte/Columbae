using System.Drawing;
using System.Linq;
using Xunit;

namespace Columbae.Tests
{
    public class PolygonTests
    {
        [Theory]
        [InlineData("0,1,2,0,1,3,3,3,2,2", "1,1")]
        [InlineData("0,0,0,2,2,2,2,0", "1,1")]
        public void Polygon_Contains_ShouldMatch(string polygonStr, string pointToMatch)
        {
            // arrange
            var polygon = Polygon.Parse(polygonStr);
            var matchingPoint = Polypoint.Parse(pointToMatch);

            // act
            var isContained = polygon.Contains(matchingPoint);

            // assert
            Assert.True(isContained);
        }

        [Theory]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", "-2,-2,-3,-3")] // Negative fully inside
        [InlineData("0,1,0,4,3,4,3,1", "0,2,4,3")] // Start & stop outside, but crossing
        [InlineData("0,1,0,4,3,4,3,1", "1,2,2,3")] // Fully inside
        [InlineData("0,1,0,4,3,4,3,1", "1,2,6,3")] // Start inside, end outside
        [InlineData("0,1,0,4,3,4,3,1", "0,0,1,3")] // Start outside, end inside
        [InlineData("0,1,0,4,3,4,3,1", "-1,-2,1,3")] // Start outside, end inside
        public void Polygon_Intersects_ShouldMatch(string polygonStr, string segmentToMatch)
        {
            // arrange
            var polygon = Polygon.Parse(polygonStr);
            var matchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = polygon.Intersects(matchingSegment);

            // assert
            Assert.True(intersects);
        }

        [Theory]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", "2,2,3,3")]
        public void Polygon_Intersects_ShouldNotMatch(string polygonStr, string segmentToMatch)
        {
            // arrange
            var polygon = Polygon.Parse(polygonStr);
            var notMatchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = polygon.Intersects(notMatchingSegment);

            // assert
            Assert.False(intersects);
        }

        [Theory]
        [InlineData("mlwlH{dt\\co~BghjB", true)] // Outside to inside
        [InlineData("ealkHs{hYdlG{`sCmqn@pg{Aimy@|llB}mz@|`l@_tSgzcBo~Ti`]", true)] // Outside to inside
        [InlineData("klavHu}nWcwEquaBl{d@khCxnj@|p_Bqem@uqG", true)] // Inside
        [InlineData("cstlHyenS|pg@gl}Aq}h@smT", false)] // Outside
        public void Polygon_Intersects_ShouldBeInBelgium(string polylineStr, bool shouldMatch)
        {
            // arrange
            var polyline = new Polyline(polylineStr);
            var belgium = GeoConstants.Belgium;

            // act
            var intersects = belgium.Intersects(polyline);

            // assert
            Assert.Equal(shouldMatch, intersects);
        }

        [Theory]
        [InlineData("0,0,0,4,1,3,4,4", 4)]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", 4)]
        public void Polygon_Parse_ShouldMatch(string polygonStr, int expectedVertices)
        {
            // arrange
            var polygon = Polygon.Parse(polygonStr);

            // assert
            Assert.NotNull(polygon);
            Assert.Equal(expectedVertices, polygon.Vertices.Count);
        }


        [Theory]
        [InlineData("0,0,0,4,1,3,4,6,7")]
        [InlineData("0,0,0,4")]
        [InlineData("0,0,0,4,1,3,d,6,7")]
        public void Polygon_Parse_IncorrectValues_ShouldBeNull(string polygonStr)
        {
            // arrange
            var polygon = Polygon.Parse(polygonStr);

            // assert
            Assert.Null(polygon);
        }
    }
}