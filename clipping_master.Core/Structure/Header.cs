using System;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// The header information defined how app is execute
    /// </summary>
    [Serializable]
    public struct Header
    {
        [XmlElement("filename")]
        public string filename { set; get; }
        [XmlElement("output")]
        public string output { set; get; }
        [XmlElement("cleantemp")]
        public bool cleantemp { set; get; }
        [XmlElement("temp")]
        public string temp { set; get; }
    }
}
