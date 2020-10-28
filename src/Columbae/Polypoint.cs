using System;
using System.IO;
using System.Linq;
using Columbae.GeoJson;
using Newtonsoft.Json;

namespace Columbae
{
    public class Polypoint : IShape
    {
        public Polypoint(double longitude, double latitude)
        {
            X = longitude;
            Y = latitude;
        }

        public override string ToString()
        {
            return $"{X:0.00000} {Y:0.00000}";
        }

        public double Y { get; private set; }
        public double X { get; private set; }
        public double Longitude => X;
        public double Latitude => Y;

        public override bool Equals(object obj)
        {
            if (obj is Polypoint point)
            {
                return (Math.Abs(Y - point.Y) <= 0.00001 &&
                        Math.Abs(X - point.X) <= 0.00001);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public double GetDistance(Polypoint pt)
        {
            var dx = pt.X - X;
            var dy = pt.Y - Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        
        public double CalculateDistanceKilometer(Polypoint point)
        {
            return Calculator.CalculateDistanceKilometer(this, point);
        }

        public static Polypoint ParseCsv(string description)
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
        
        public string ToJson()
        {
            var stringWriter = new StringWriter();
            var ser = new JsonSerializer();
            var writer = new JsonTextWriter(stringWriter);
            ser.Serialize(writer,new Pointstring{type = "Point", coordinates = new [] {X, Y}});
            return stringWriter.ToString();
        }

        public static Polypoint ParseJson(string json)
        {
            var geoJsonPoint = JsonConvert.DeserializeObject<Pointstring>(json);
            if (geoJsonPoint.type == "Point")
            {
                if (geoJsonPoint.coordinates != null)
                {
                    return new Polypoint(geoJsonPoint.coordinates[0], geoJsonPoint.coordinates[1]);
                }
            }

            return null;
        }
        
        public double Cross(Polypoint point)
        {
            return X * point.Y - Y * point.X;
        }
        
        public static Polypoint operator -(Polypoint v, Polypoint w)
        {
            return new Polypoint(v.X - w.X, v.Y - w.Y);
        }

        public static Polypoint operator +(Polypoint v, Polypoint w)
        {
            return new Polypoint(v.X + w.X, v.Y + w.Y);
        }

        public static double operator *(Polypoint v, Polypoint w)
        {
            return v.X * w.X + v.Y * w.Y;
        }

        public static Polypoint operator *(Polypoint v, double mult)
        {
            return new Polypoint(v.X * mult, v.Y * mult);
        }

        public static Polypoint operator *(double mult, Polypoint v)
        {
            return new Polypoint(v.X * mult, v.Y * mult);
        }
    }
}