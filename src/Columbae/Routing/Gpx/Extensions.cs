using System.Xml.Serialization;

namespace Cotacol.Models.Routes
{
    [XmlRoot(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Extensions
    {
        [XmlElement(ElementName = "TrackPointExtension",
            Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public TrackPointExtension TrackPointExtension { get; set; }
    }
}