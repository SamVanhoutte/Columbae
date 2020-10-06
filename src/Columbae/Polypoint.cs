using System;
using System.Linq;

namespace Columbae
{
    public class Polypoint
    {
        public Polypoint(double longitude, double latitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString()
        {
            return $"{Longitude:0.00000} {Latitude:0.00000}";
        }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is Polypoint point)
            {
                return (Math.Abs(Latitude - point.Latitude) <= 0.00001 &&
                        Math.Abs(Longitude - point.Longitude) <= 0.00001);
            }

            return false;
        }

        public double GetDistance(Polypoint point)
        {
            return Calculator.CalculateDistance(this, point);
        }

        public static Polypoint Parse(string description)
        {
            // Format should be of X1,Y1,X2,Y2
            var coordinates = description.Split(',');
            if (coordinates.Length == 2)
            {
                if (coordinates.All(s => double.TryParse(s, out var d)))
                {
                    //TODO :localization
                    return new Polypoint(double.Parse(coordinates[0]), double.Parse(coordinates[1]));
                }
            }

            return null;
        }
    }
}