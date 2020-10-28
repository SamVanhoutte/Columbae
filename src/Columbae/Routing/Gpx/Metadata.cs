using System.Xml.Serialization;

namespace Columbae.Routing.Gpx
{
    [XmlRoot(ElementName = "metadata", Namespace = "http://www.topografix.com/GPX/1/1")]
    internal class Metadata
    {
        [XmlElement(ElementName = "time", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Time { get; set; }
    }
}