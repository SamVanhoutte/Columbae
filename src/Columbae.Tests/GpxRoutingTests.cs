using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Columbae.Tests
{
    public class GpxRoutingTests
    {
        [Theory]
        [InlineData("41.85285", "-87.63941", "ikm~Fha|uO")]     // Chicago
        public void ToString_WithOneElement_ShouldReturnExpectedPolyline(string latitude, string longitude, string expectedPolyline)
        {
            // arrange
            var points = new List<Polypoint> {
                new Polypoint(double.Parse(longitude, CultureInfo.InvariantCulture), double.Parse(latitude, CultureInfo.InvariantCulture))
            };
            var polyline = new Polyline(points);

            // act
            var result = polyline.ToPolylineString();

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(expectedPolyline, result);
        }
    }
}