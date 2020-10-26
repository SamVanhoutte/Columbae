using System.Xml.Serialization;

namespace Cotacol.Models.Routes
{
    [XmlRoot(ElementName = "metadata", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Metadata
    {
        [XmlElement(ElementName = "time", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Time { get; set; }
    }
}