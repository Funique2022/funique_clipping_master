using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// Job array
    /// </summary>
    [Serializable]
    public struct Jobs
    {
        [XmlElement("job")]
        public List<Job> job { set; get; }
    }
}
