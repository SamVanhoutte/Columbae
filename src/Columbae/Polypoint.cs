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
            return $"{Latitude:0.00000} {Longitude:0.00000}";
        }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }
    }
}