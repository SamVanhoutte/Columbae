using System;

namespace Columbae
{
    public struct Polypoint
    {
        public Polypoint(double latitude, double longitude)
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
                    Math.Abs(Longitude - point.Longitude) <= 0.00001) ;
            }

            return false;
        }
        
        public double GetDistance(Polypoint point)
        {
            return Calculator.CalculateDistance(this, point);
        }
    }
}