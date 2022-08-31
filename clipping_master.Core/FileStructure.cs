using System;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    [Serializable]
    [XmlRoot("file_start")]
    public struct FileStructure
    {
        [XmlElement("header")]
        public Header header { set; get; }
        [XmlElement("jobs")]
        public Jobs jobs { set; get; }
    }
}
