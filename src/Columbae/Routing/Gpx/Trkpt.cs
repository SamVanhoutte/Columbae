using System.Xml.Serialization;

namespace Cotacol.Models.Routes
{
    [XmlRoot(ElementName = "trkpt", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Trkpt
    {
        [XmlElement(ElementName = "ele", Namespace = "http://www.topografix.com/GPX/1/1")]
        public double Ele { get; set; }

        [XmlElement(ElementName = "time", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Time { get; set; }

        [XmlElement(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Extensions Extensions { get; set; }

        [XmlAttribute(AttributeName = "lat")] public double Lat { get; set; }
        [XmlAttribute(AttributeName = "lon")] public double Lon { get; set; }
    }
}