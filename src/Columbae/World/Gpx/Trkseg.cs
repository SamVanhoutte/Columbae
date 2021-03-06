using System.Collections.Generic;
using System.Xml.Serialization;

namespace Columbae.World.Gpx
{
    [XmlRoot(ElementName = "trkseg", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Trkseg
    {
        [XmlElement(ElementName = "trkpt", Namespace = "http://www.topografix.com/GPX/1/1")]
        public List<Trkpt> Trkpt { get; set; }
    }
}