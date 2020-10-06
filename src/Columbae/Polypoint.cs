namespace Columbae
{
    public class Polypoint
    {
        public Polypoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public override string ToString()
        {
            return $"{Longitude:0.00000} {Latitude:0.00000}";
        }

        public double Longitude { get; private set; }

        public double Latitude { get; private set; }
        
    }
}