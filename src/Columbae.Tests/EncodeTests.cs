using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Columbae.Tests
{
    public class EncodeTests
    {
        [Theory]
        [InlineData("41.85285", "-87.63941", "ikm~Fha|uO")]     // Chicago
        [InlineData("52.22778", "20.98614", "svw}Hkza_C")]      // Warsaw
        [InlineData("41.89997", "12.50083", "yqv~FeqhkA")]      // Rome
        [InlineData("35.68512", "139.66267", "_wxxEuzlsY")]     // Tokyo
        [InlineData("-33.45773", "-70.67095", "xuekEl|inL")]    // Sandiego
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

        [Theory]
        [InlineData("41.86231", "-87.63804", "41.87458", "-87.63460", "mfo~Fvx{uOukAoT")]     // Chicago
        public void ToString_WithTwoElements_ShouldReturnExpectedPolyline(string latitude1, string longitude1, string latitude2, string longitude2, string expectedPolyline)
        {
            // arrange
            var points = new List<Polypoint> {
                new Polypoint(double.Parse(longitude1, CultureInfo.InvariantCulture), double.Parse(latitude1, CultureInfo.InvariantCulture)),
                new Polypoint(double.Parse(longitude2, CultureInfo.InvariantCulture), double.Parse(latitude2, CultureInfo.InvariantCulture))
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