using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// Action array
    /// </summary>
    [Serializable]
    public struct Actions
    {
        [XmlElement("action")]
        public List<Action> action { set; get; }
    }
}
