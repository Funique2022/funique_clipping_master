using System;
using System.Xml;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    /// <summary>
    /// Define single action and multuple action
    /// </summary>
    [Serializable]
    public struct Job
    {
        /// <summary>
        /// Must be 0 - 1 <br />
        /// 0: Single <br />
        /// 1: Multiple
        /// </summary>
        [XmlElement("type")]
        public ThreadType type { set; get; }
        [XmlElement("action", IsNullable = true)]
        public Nullable<Action> action { set; get; }
        [XmlElement("actions", IsNullable = true)]
        public Nullable<Actions> actions { set; get; }
    }
}
