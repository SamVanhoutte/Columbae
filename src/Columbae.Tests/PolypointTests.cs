using Xunit;

namespace Columbae.Tests
{
    public class PolypointTests
    {
        [Fact]
        public void Polypoint_Equals_ShouldMatch()
        {
            var p1 = new Polypoint(1, 30);
            var p2 = new Polypoint(1.0, 30D);
            
            Assert.True(p1.Equals(p2));
        }
    }
}