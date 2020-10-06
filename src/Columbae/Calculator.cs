using System;
using System.Drawing;

namespace Columbae
{
    public class Calculator
    {
        public static double CalculateDistance(Polypoint point1, Polypoint point2)
        {
            var earthRadius = 6371; // Radius of the earth in km
            var latDistance = Degrees2Radius(point2.Latitude - point1.Latitude); // deg2rad below
            var lonDistance = Degrees2Radius(point2.Longitude - point1.Longitude);
            var a =
                    Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                    Math.Cos(Degrees2Radius(point1.Latitude)) * Math.Cos(Degrees2Radius(point2.Latitude)) *
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