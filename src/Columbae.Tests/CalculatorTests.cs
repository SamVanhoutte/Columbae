using System;
using Columbae.World;
using Xunit;

namespace Columbae.Tests
{
    public class CalculatorTests
    {
        [Theory]
        [InlineData(41.85285, -87.63941, 52.22778, 20.98614, 7513)]     // Chicago - Warsaw
        [InlineData(41.89997, 12.50083, 35.68512, 139.66267, 9852)]      // Rome - Tokyo
        [InlineData(50.819477, 3.257726, 51.054340, 3.717424, 41)]    // Kortrijk - Ghent
        [InlineData(50.819477, 3.257726, 50.819477, 3.257726, 0)]    // Kortrijk - Kortrijk
        public void CalculatedDistanceShouldMatch(double latitude1, double longitude1, double latitude2, double longitude2, int expectedDistance)
        {
            // arrange
            var point1 = new Polypoint(longitude1, latitude1);
            var point2 = new Polypoint(longitude2, latitude2);

            // act
            var distanceInKm = Calculator.CalculateDistanceKilometer(point1, point2);

            // assert
            Assert.Equal(expectedDistance, (int)Math.Round(distanceInKm));
        }
    }
}