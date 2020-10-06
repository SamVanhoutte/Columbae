using System;
using Xunit;

namespace Columbae.Tests
{
    public class PolysegmentTests
    {
        [Theory]
        [InlineData(0, 0, 10, 10, 5, 5)]
        [InlineData(20, 20, 10, 10, 15, 15)]
        [InlineData(0, 0, 10, 10, 0, 0)]
        [InlineData(0, 0, 10, 10, 10, 10)]
        [InlineData(0, 0, 10, 5, 8, 4)]
        [InlineData(1, 3.3, 4, 12.3, 2.5, 7.8)]
        public void Polysegment_IsOnTheLine_ShouldContainPoint(double x1, double y1, double x2, double y2,
            double xMatch,
            double yMatch)
        {
            // arrange
            var point1 = new Polypoint(y1, x1);
            var point2 = new Polypoint(y2, x2);
            var segment = new Polysegment(point1, point2);
            var matchingPoint = new Polypoint(yMatch, xMatch);

            // act
            var result = segment.IsOnTheLine(matchingPoint);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("0, 0, 10, 10", "5, 5")]
        [InlineData("20, 20, 10, 10", "15, 15")]
        [InlineData("0, 0, 10, 10", "0, 0")]
        [InlineData("0, 0, 10, 10", "10, 10")]
        [InlineData("0, 0, 10, 5", "8, 4")]
        [InlineData("1, 3.3, 4, 12.3", "2.5, 7.8")]
        [InlineData("0, 0, 10, 10", "0.1, 9.9")]
        [InlineData("0, 3, 10, 5", "8, 3.2")]
        [InlineData("0,4,3,1", "2,2")]
        [InlineData("1,1,3,3", "2,2")]
        public void Polysegment_IsInArea_ShouldContainPoint(string segmentString, string pointString)
        {
            // arrange
            var segment =  Polysegment.Parse(segmentString);
            var matchingPoint =  Polypoint.Parse(pointString);

            // act
            var result = segment.IsInArea(matchingPoint);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("0, 0, 10, 10", "15, 15")]
        [InlineData("0, 0, 10, 10", "4, 5")]
        public void Polysegment_IsOnTheLine_ShouldNotContainPoint(string segmentString, string pointString)
        {
            // arrange
            var segment =  Polysegment.Parse(segmentString);
            var matchingPoint =  Polypoint.Parse(pointString);
            
            // act
            var result = segment.IsOnTheLine(matchingPoint);

            // assert
            Assert.False(result);
        }


        [Theory]
        [InlineData("0, 0, 10, 10", "5, 5")]
        [InlineData("1, 4, 3, 6", "2, 5")]
        public void Polysegment_MidPoint_ShouldMatch(string segmentString, string pointString)
        {
            // arrange
            var segment =  Polysegment.Parse(segmentString);
            var matchingPoint =  Polypoint.Parse(pointString);

            // act
            var midPoint = segment.MidPoint();

            // assert
            Assert.Equal(matchingPoint, midPoint);
        }

        [Theory]
        [InlineData("0,4,3,1", "1,1,3,3", "2,2")]
        [InlineData("0,0,3,1", "0,0,3,3", "0,0")]
        [InlineData("10,4,3,1", "10,0,3,1", "3,1")]
        public void TestIntersection(string segment1str, string segment2str, string expectedPointstr)
        {
            // arrange
            var segment1 = Polysegment.Parse(segment1str);
            var segment2 = Polysegment.Parse(segment2str);
            var matchingPoint = Polypoint.Parse(expectedPointstr);
            
            // act
            var intersects = segment1.Intersects(segment2, out var interSection);
        
            // assert
            Assert.True(intersects);
            Assert.Equal(matchingPoint, interSection);
        }
        
        [Theory]
        [InlineData("0,4,3,1",  "1,2", PointPosition.Left)]
        [InlineData("0,4,3,1",  "2,2", PointPosition.OnLine)]
        [InlineData("0,4,3,1",  "3,2", PointPosition.Right)]
        public void Segment_GetPointPosition_ShouldMatch(string segmentStr, string pointToMatch, PointPosition expectedPosition)
        {
            // arrange
            var segment = Polysegment.Parse(segmentStr);
            var matchingPoint = Polypoint.Parse(pointToMatch);
            
            // act
            var calculatedPosition = segment.GetPointPositioning(matchingPoint);
        
            // assert
            Assert.Equal(expectedPosition, calculatedPosition);
        }

        [Fact]
        public void ParseSegment_ShouldSucceed()
        {
            // Test good string
            var input = "0.2,0.3,4.5,6.5";
            var segment = Polysegment.Parse(input);
            Assert.Equal(0.2, segment.Start.Longitude);
            Assert.Equal(0.3, segment.Start.Latitude);
            Assert.Equal(4.5, segment.End.Longitude);
            Assert.Equal(6.5, segment.End.Latitude);
        }

        [Fact]
        public void ParseIncorrectSegment_ShouldReturnNull()
        {
            // Test non numeric string
            var input = "0.2,0.d,4.5,6.5";
            var segment = Polysegment.Parse(input);
            Assert.Null(segment);

            // Test non numeric string
            input = "0.2,0.d,4.5,";
            segment = Polysegment.Parse(input);
            Assert.Null(segment);

            // Test missing values
            input = "0.2,0,4.5";
            segment = Polysegment.Parse(input);
            Assert.Null(segment);
        }
    }
}