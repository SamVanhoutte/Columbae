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
        public void Polysegment_IsOnTheLine_ShouldContainPoint(double x1, double y1, double x2, double y2, double xMatch,
            double yMatch)
        {
            // arrange
            var point1 = new Polypoint(x1, y1);
            var point2 = new Polypoint(x2, y2);
            var segment = new Polysegment(point1, point2);
            var matchingPoint = new Polypoint(xMatch, yMatch);
            
            // act
            var result = segment.IsOnTheLine(matchingPoint);

            // assert
            Assert.True(result);
        }
        
        [Theory]
        [InlineData(0, 0, 10, 10, 15, 15)]
        [InlineData(0, 0, 10, 10, 4, 5)]
        public void Polysegment_IsOnTheLine_ShouldNotContainPoint(double x1, double y1, double x2, double y2, double xMatch,
            double yMatch)
        {
            // arrange
            var point1 = new Polypoint(x1, y1);
            var point2 = new Polypoint(x2, y2);
            var segment = new Polysegment(point1, point2);
            var matchingPoint = new Polypoint(xMatch, yMatch);
            
            // act
            var result = segment.IsOnTheLine(matchingPoint);

            // assert
            Assert.False(result);
        }
        

        
        [Theory]
        [InlineData(0, 0, 10, 10, 5, 5)]
        [InlineData(1, 4, 3, 6, 2, 5)]
        public void Polysegment_MidPoint_ShouldMatch(double x1, double y1, double x2, double y2, double xMatch,
            double yMatch)
        {
            // arrange
            var point1 = new Polypoint(x1, y1);
            var point2 = new Polypoint(x2, y2);
            var segment = new Polysegment(point1, point2);
            var matchingPoint = new Polypoint(xMatch, yMatch);
            
            // act
            var midPoint = segment.MidPoint();

            // assert
            Assert.Equal(matchingPoint, midPoint);
        }
    }
}