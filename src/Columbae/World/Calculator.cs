using System;

namespace Columbae.World
{
    public class Calculator
    {
        public static double CalculateDistanceKilometer(Polypoint point1, Polypoint point2)
        {
            var earthRadius = 6371; // Radius of the earth in km
            var latDistance = Degrees2Radius(point2.Y - point1.Y); // deg2rad below
            var lonDistance = Degrees2Radius(point2.X - point1.X);
            var a =
                    Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                    Math.Cos(Degrees2Radius(point1.Y)) * Math.Cos(Degrees2Radius(point2.Y)) *
                    Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2)
                ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = earthRadius * c; // Distance in km
            return d;
        }

        private static double Degrees2Radius(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}