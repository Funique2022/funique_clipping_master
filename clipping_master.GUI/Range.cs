using System;
using System.Collections.Generic;
using System.Text;

namespace ffmpeg_helper.GUI
{
    public class Range
    {
        public TimeSpan Start { set; get; } = TimeSpan.FromSeconds(0);
        public TimeSpan End { set; get; } = TimeSpan.FromSeconds(0);

        public string StartString
        {
            set => _ = value;
            get => Start.ToString(@"hh\:mm\:ss\:ff");
        }
        public string EndString
        {
            set => _ = value;
            get => End.ToString(@"hh\:mm\:ss\:ff");
        }
    }
}
