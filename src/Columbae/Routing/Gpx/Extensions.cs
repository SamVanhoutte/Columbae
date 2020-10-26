using System.Xml.Serialization;

namespace Columbae.Routing.Gpx
{
    [XmlRoot(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
    internal class Extensions
    {
        [XmlElement(ElementName = "TrackPointExtension",
            Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public TrackPointExtension TrackPointExtension { get; set; }
    }
}