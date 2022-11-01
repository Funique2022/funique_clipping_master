using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// File name array, for merge purposes
    /// </summary>
    [Serializable]
    public struct FileNames
    {
        [XmlElement("file")]
        public List<string> file { set; get; }
    }
}
