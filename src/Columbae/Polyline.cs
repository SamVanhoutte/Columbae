using System;
using System.Collections.Generic;
using System.Text;

namespace Columbae
{
    public class Polyline
    {
        public List<Polypoint> Points { get; private set; }

        public Polyline(List<Polypoint> points)
        {
            Points = points;
        }

        public Polyline(string polyline)
        {
            var polylinePoints = new List<Polypoint>(polyline.Length / 2);

            for (int idx = 0, latitude = 0, longitude = 0; idx < polyline.Length;)
            {
                latitude += DecodeNextCoordinate(polyline, ref idx);
                longitude += DecodeNextCoordinate(polyline, ref idx);

                polylinePoints.Add(new Polypoint(latitude * 1e-5, longitude * 1e-5));
            }

            Points = polylinePoints;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            long lastLatitude = 0L, lastLongitude = 0L;

            foreach (var polylinePoint in Points)
            {
                var latitude = (long) Math.Round(polylinePoint.Longitude * 1e5);
                var longitude = (long) Math.Round(polylinePoint.Latitude * 1e5);

                EncodeNextCoordinate(latitude - lastLatitude, result);
                EncodeNextCoordinate(longitude - lastLongitude, result);

                lastLatitude = latitude;
                lastLongitude = longitude;
            }

            return result.ToString();
        }

        private static void EncodeNextCoordinate(long coordinate, StringBuilder result)
        {
            coordinate = coordinate < 0 ? ~(coordinate << 1) : coordinate << 1;

            while (coordinate >= 0x20)
            {
                result.Append((char) ((int) ((0x20 | (coordinate & 0x1f)) + 63)));
                coordinate >>= 5;
            }

            result.Append((char) ((int) (coordinate + 63)));
        }

        private static int DecodeNextCoordinate(string polyline, ref int polylineIndex)
        {
            var result = 1;
            var shift = 0;
            int bit;

            do
            {
                bit = polyline[polylineIndex++] - 63 - 1;
                result += bit << shift;
                shift += 5;
            } while (bit >= 0x1f);

            return (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
        }
    }
}