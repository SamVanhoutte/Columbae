using System;
using System.Globalization;
using Xunit;

namespace Columbae.Tests
{
    public class DecodeTests
    {
        [Theory]
        [InlineData("ikm~Fha|uO", "41.85285", "-87.63941")]     // Chicago
        [InlineData("svw}Hkza_C", "52.22778", "20.98614")]      // Warsaw
        [InlineData("yqv~FeqhkA", "41.89997", "12.50083")]      // Rome
        [InlineData("_wxxEuzlsY", "35.68512", "139.66267")]     // Tokyo
        [InlineData("xuekEl|inL", "-33.45773", "-70.67095")]    // Sandiego
        public void Constructor_ShouldReturnArray_WithOnePositionElement(string polylineString, string latitude, string longitude)
        {
            // arrange
            var polyline = new Polyline(polylineString);

            // act
            var conversionString = polyline.ToString();
            Assert.Equal(polylineString, conversionString);

            // assert
            Assert.Single(polyline.Points);
            Assert.Equal($"{longitude} {latitude}", polyline.Points[0].ToString());

        }

        [Theory]
        [InlineData("mfo~Fvx{uOukAoT", "41.86231", "-87.63804", "41.87458", "-87.63460")]     // Chicago
        public void Constructor_ShouldReturnArray_WithTwoPositionsElement(string polylineString, string latitude1, string longitude1, string latitude2, string longitude2)
        {
            // arrange
            var result = new Polyline(polylineString);

            // act
            var conversionString = result.ToString();
            Assert.Equal(polylineString, conversionString);

            // assert
            Assert.Equal(2, result.Points.Count);
            
            Assert.Equal($"{longitude1} {latitude1}", result.Points[0].ToString());
            Assert.Equal($"{longitude2} {latitude2}", result.Points[1].ToString());
        }

        [Theory]
        [InlineData(@"wvduHmyjU_CbAi@R_@FiAJkCz@cBd@{@T]Bu@Pq@TeAd@q@b@u@\\mAt@i@^aCtB[^Wf@w@bCI^Kv@A`AEh@]zBU`CKf@GPQTcA`A")]
        public void Constructor_ShouldParse(string polylineString)
        {
            // arrange
            var result = new Polyline(polylineString);

            // assert
            Assert.NotNull(result);
        }
    }
}