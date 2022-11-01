using System;
using System.Collections.Generic;
using System.Text;

namespace ffmpeg_helper
{
    public static class CodecMap
    {
        public static Dictionary<CodecType, string> Dict = new Dictionary<CodecType, string>()
        {
            { CodecType.Copy, string.Empty },
            { CodecType.H264, "libx264" },
            { CodecType.H265, "libx265" },
            { CodecType.QuickTime, "qtrle" },
            { CodecType.FLV, "v410" },
            { CodecType.WMV, "wmv2" },
        };
    }
}
