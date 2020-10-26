using System;
using System.Xml.Serialization;

namespace Cotacol.Models.Routes
{
    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxRoute
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "trk", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Trk Trk { get; set; }

        [XmlAttribute(AttributeName = "creator")]
        public string Creator { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "gpxtpx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxtpx { get; set; }

        [XmlAttribute(AttributeName = "gpxx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxx { get; set; }
    }
}

