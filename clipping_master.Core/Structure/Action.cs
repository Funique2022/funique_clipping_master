using System;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// Define single action <br />
    /// The command that will tell ffmpeg run
    /// </summary>
    [Serializable]
    public struct Action
    {
        /// <summary>
        /// cut: cutting video clip <br />
        /// merge: merge temp folder multiple clip
        /// </summary>
        [XmlElement("type", IsNullable = true)]
        public ActionType type { set; get; }
        [XmlElement("start", IsNullable = true)]
        public string start { set; get; }
        [XmlElement("end", IsNullable = true)]
        public string end { set; get; }
        [XmlElement("length", IsNullable = true)]
        public string length { set; get; }
        [XmlElement("name", IsNullable = true)]
        public string name { set; get; }
        [XmlElement("encoder", IsNullable = true)]
        public string encoder { set; get; }
        [XmlElement("all", IsNullable = true)]
        public Nullable<bool> all { set; get; }
        [XmlElement("files", IsNullable = true)]
        public Nullable<FileNames> files { set; get; }
    }
}
