using Xunit;

namespace Columbae.Tests
{
    public class PolypointTests
    {
        [Fact]
        public void Polypoint_Equals_ShouldMatch()
        {
            var p1 = new Polypoint(30, 1);
            var p2 = new Polypoint(30D, 1.0);
            
            Assert.True(p1.Equals(p2));
        }
        
        [Fact]
        public void ParsePoint_ShouldSucceed()
        {
            // Test good string
            var input = "0.2,0.3";
            var point = Polypoint.Parse(input);
            Assert.Equal(0.2, point.Longitude);
            Assert.Equal(0.3, point.Latitude);
        }

        [Fact]
        public void ParseIncorrectPoint_ShouldReturnNull()
        {
            // Test non numeric string
            var input = "0.2,0.d";
            var point = Polypoint.Parse(input);
            Assert.Null(point);

            // Test non numeric string
            input = "0.2,0.d,4.5,";
            point = Polypoint.Parse(input);
            Assert.Null(point);

            // Test missing values
            input = "0.2";
            point = Polypoint.Parse(input);
            Assert.Null(point);
        }
    }
}