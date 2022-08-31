using LibVLCSharp.WPF;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace ffmpeg_helper.GUI
{
    public class WindowDataContext : INotifyPropertyChanged
    {
        public Visibility ContentVisibility { set; get; } = Visibility.Collapsed;
        public Visibility RangeVisibility { set; get; } = Visibility.Collapsed;
        public Thickness RangeMargin { set; get; } = new Thickness(30, 7, 30, 7);
        public float WindowWidth { set; get; }
        public VideoView VlcSource { set; get; }
        public Slider Slider { set; get; }
        public Range SelectRange { set; get; }
        public string Source { set; get; }
        public string Dist { set; get; }
        public bool onSliderPressed = false;
        public bool isPlayBefore = false;
        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer Timer;

        public LibVLCSharp.Shared.MediaPlayer Player => VlcSource?.MediaPlayer;
        string CurrentVideoTimecode
        {
            get
            {
                if (Player != null && Player.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(Player.Time);
                    return time.ToString(@"hh\:mm\:ss\:ff");
                }
                else return "00:00:00:00";
            }
        }
        string LengthVideoTimecode
        {
            get
            {
                if (Player != null && Player.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(VlcSource.MediaPlayer.Length);
                    return time.ToString(@"hh\:mm\:ss\:ff");
                }
                else return "00:00:00:00";
            }
        }
        public string TimeCode
        {
            get
            {
                return $"{CurrentVideoTimecode}/{LengthVideoTimecode}";
            }
        }
        public long Duration
        {
            get
            {
                if (Player == null || !Player.IsSeekable) return 0;
                return Player.Length;
            }
        }
        public long TimePosition
        {
            set
            {
                if(Player != null)
                    Player.Time = value;
            }
            get
            {
                if (Player == null) return 0;
                return Player.Time;
            }
        }


        public WindowDataContext()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(20);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateRangeHighLight();
            OnPropertyChanged("TimeCode");
            OnPropertyChanged("Duration");

            if(!onSliderPressed)
                OnPropertyChanged("TimePosition");
        }

        private void UpdateRangeHighLight()
        {
            if(RangeVisibility == Visibility.Visible && SelectRange != null)
            {
                double width = Slider.ActualWidth;
                double hWidth = width / 2f;
                double perPixel = Duration / width;
                long left_mil = (long)SelectRange.Start.TotalMilliseconds;
                long right_mil = (long)SelectRange.End.TotalMilliseconds;

                double left_pixel_offset = left_mil / perPixel;
                double right_pixel_offset = right_mil / perPixel;

                RangeMargin = new Thickness(30 + left_pixel_offset, 7, 30 + (width - right_pixel_offset), 7);
                OnPropertyChanged("RangeMargin");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
