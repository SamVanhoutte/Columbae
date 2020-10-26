using System.Xml.Serialization;

namespace Cotacol.Models.Routes
{
    [XmlRoot(ElementName = "TrackPointExtension",
        Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
    public class TrackPointExtension
    {
        [XmlElement(ElementName = "hr", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Hr { get; set; }

        [XmlElement(ElementName = "atemp", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Atemp { get; set; }
    }
}