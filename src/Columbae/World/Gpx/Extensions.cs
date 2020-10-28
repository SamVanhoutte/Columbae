using System.Xml.Serialization;

namespace Columbae.World.Gpx
{
    [XmlRoot(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Extensions
    {
        [XmlElement(ElementName = "TrackPointExtension",
            Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public TrackPointExtension TrackPointExtension { get; set; }
    }
}