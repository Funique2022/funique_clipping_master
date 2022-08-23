using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace ffmpeg_helper.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowDataContext ctx;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mPlayer.SourceUpdated += MPlayer_SourceUpdated;
        }

        private void MPlayer_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            ctx = new WindowDataContext(mPlayer, TimeSplider, strcture);
            this.DataContext = ctx;
        }

        #region Media Control
        private void MarkStartButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mPlayer.SourceProvider.MediaPlayer.Play();
            ctx.isPlaying = true;
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mPlayer.SourceProvider.MediaPlayer.Pause();
            ctx.isPlaying = false;
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mPlayer.SourceProvider.MediaPlayer.Stop();
            ctx.isPlaying = false;
        }
        private void MarkEndButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region TimeCode Control
        private void DecreaseMin_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DecreaseSec_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DecreaseTick_Click(object sender, RoutedEventArgs e)
        {

        }
        private void IncreaseTick_Click(object sender, RoutedEventArgs e)
        {

        }
        private void IncreaseSec_Click(object sender, RoutedEventArgs e)
        {

        }
        private void IncreaseMin_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Slider Control
        private void TimeSplider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            ctx.isPlayBefore = ctx.isPlaying;
            ctx.isPlaying = false;
            ctx.onSliderPressed = true;
            mPlayer.SourceProvider.MediaPlayer.Pause();
        }
        private void TimeSplider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ctx.onSliderPressed = false;
            if (ctx.isPlayBefore)
            {
                mPlayer.SourceProvider.MediaPlayer.Play();
                ctx.isPlaying = true;
            }
        }
        #endregion

        #region Menu
        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SelectVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectFile = new OpenFileDialog();
            bool? result = selectFile.ShowDialog(this);
            if (result.HasValue && result.Value && selectFile.CheckPathExists)
            {
                string v = System.IO.Path.GetFullPath(selectFile.FileName);
                mPlayer.SourceProvider.MediaPlayer.Play(new Uri($"file:\\\\\\{v}", UriKind.Absolute));
                SetTitle(v);
            }
        }
        #endregion

        #region Utility
        private void SetTitle(string title)
        {
            this.Title = $"Video Editor Helper, {title}";
        }
        #endregion
    }

    class WindowDataContext : INotifyPropertyChanged
    {
        readonly VlcMediaPlayer mPlayer;
        readonly Slider mSlider;
        readonly DispatcherTimer timer;

        public string source = string.Empty;
        public string dist = string.Empty;

        public bool onSliderPressed = false;
        public bool isPlayBefore = false;
        public bool isPlaying = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public WindowDataContext(VlcMediaPlayer mPlayer, Slider mSlider, FileStructure loadData)
        {
            this.mPlayer = mPlayer;
            mPlayer.Opening += MediaPlayer_Opening;
            this.mSlider = mSlider;

            source = loadData.header.filename;
            dist = loadData.header.output;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MediaPlayer_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            OnPropertyChanged("timeCode");
            OnPropertyChanged("duration");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OnPropertyChanged("timeCode");
            OnPropertyChanged("duration");

            if(!onSliderPressed)
                OnPropertyChanged("time_position");
        }

        string currentVideoTimecode
        {
            get
            {
                if (mPlayer.SourceProvider.MediaPlayer.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(mPlayer.SourceProvider.MediaPlayer.Time);
                    return time.ToString(@"hh\:mm\:ss\:ff");
                }
                else return "00:00:00:00";
            }
        }
        string lengthVideoTimecode
        {
            get
            {
                if (mPlayer.SourceProvider.MediaPlayer.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(mPlayer.SourceProvider.MediaPlayer.Length);
                    return time.ToString(@"hh\:mm\:ss\:ff");
                }
                else return "00:00:00:00";
            }
        }
        public string timeCode
        {
            get
            {
                return $"{currentVideoTimecode}/{lengthVideoTimecode}";
            }
        }
        public double duration
        {
            get
            {
                if (mPlayer.SourceProvider.MediaPlayer.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(mPlayer.SourceProvider.MediaPlayer.Length);
                    return time.TotalMilliseconds;
                }
                else return 0;
            }
        }
        public double time_position
        {
            set
            {
                mPlayer.SourceProvider.MediaPlayer.Position = (float)value;
            }
            get
            {
                if (mPlayer.SourceProvider.MediaPlayer.IsSeekable)
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(mPlayer.SourceProvider.MediaPlayer.Time);
                    return time.TotalMilliseconds;
                }
                else return 0;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
