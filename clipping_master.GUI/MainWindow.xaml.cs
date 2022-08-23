using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using LibVLCSharp.Shared.MediaPlayerElement;
using LibVLCSharp.Shared;
using System.Xml;

namespace ffmpeg_helper.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowDataContext ctx => this.DataContext as WindowDataContext;
        LibVLC libVLC;
        string source;
        string[] options = new string[] {
                "--glconv=any",
                "--vout=gl",
                "--no-video-deco",
                "--input-fast-seek",
                "--deinterlace=1",
                "--deinterlace-mode=discard",
                "--postproc-q=0",
                "--file-caching=10000",
                "--live-caching=10000",
                "--disc-caching=10000",
                "--no-keyboard-events",
                "--no-mouse-events",
                "--video-title-show",
                "--input-repeat=2"
            };
        private MediaPlayer sourceProvider;
        private Dictionary<int, string> SelectEncoderDict = new Dictionary<int, string>()
        {
            { 0, null },
            { 1, "libx264" },
            { 2, "libx265" },
            { 3, "qtrle" },
            { 4, "v410" },
            { 5, "wmv2" },
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctx.Slider = TimeSplider;
            libVLC = new LibVLC(true, options);
            sourceProvider = new MediaPlayer(libVLC);
            BackgroundVideo.MediaPlayer = sourceProvider;
            ctx.VlcSource = BackgroundVideo;
        }

        #region Media Control
        private void MarkStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ctx.SelectRange == null) return;
            TimeSpan t = TimeSpan.FromMilliseconds(sourceProvider.Time);
            if (ctx.SelectRange.End < t) return;
            ctx.SelectRange.Start = t;
            ctx.OnPropertyChanged("SelectRange");
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(!sourceProvider.IsPlaying)
                sourceProvider.Pause();
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sourceProvider.IsPlaying)
                sourceProvider.Pause();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            sourceProvider.Position = 0;
        }
        private void MarkEndButton_Click(object sender, RoutedEventArgs e)
        {
            if (ctx.SelectRange == null) return;
            TimeSpan t = TimeSpan.FromMilliseconds(sourceProvider.Time);
            if (ctx.SelectRange.Start > t) return;
            ctx.SelectRange.End = t;
            ctx.OnPropertyChanged("SelectRange");
        }
        #endregion

        #region TimeCode Control
        private void DecreaseMin_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Max(0, t - 60000);
        }
        private void DecreaseSec_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Max(0, t - 1000);
        }
        private void DecreaseTick_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Max(0, t - 100);
        }
        private void IncreaseTick_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Min(sourceProvider.Length, t + 100);
        }
        private void IncreaseSec_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Min(sourceProvider.Length, t + 1000);
        }
        private void IncreaseMin_Click(object sender, RoutedEventArgs e)
        {
            long t = sourceProvider.Time;
            sourceProvider.Time = Math.Min(sourceProvider.Length, t + 60000);
        }
        #endregion

        #region Slider Control
        private void TimeSplider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            ctx.isPlayBefore = sourceProvider.IsPlaying;
            ctx.onSliderPressed = true;
            if(ctx.isPlayBefore)
                sourceProvider.Pause();
        }
        private void TimeSplider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ctx.onSliderPressed = false;
            if (ctx.isPlayBefore)
                sourceProvider.Pause();
        }
        #endregion

        #region Utility
        ListBoxItem Generate_ListBoxItem(Action action)
        {
            ListBoxItem tb = new ListBoxItem();
            tb.Content = "Action";
            tb.Tag = action;
            return tb;
        }
        #endregion

        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectFile = new OpenFileDialog();
            bool? result = selectFile.ShowDialog(this);
            if (result.HasValue && result.Value && selectFile.CheckPathExists)
            {
                string v = System.IO.Path.GetFullPath(selectFile.FileName);
                using (Media media = new Media(libVLC, new Uri($"file:\\\\\\{v}", UriKind.Absolute), options))
                    sourceProvider.Play(media);
                ctx.Source = v;
                source = v;
                Title = $"Clipping Master, Path = {v}";
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ctx.Source)) return;
            SaveFileDialog selectFile = new SaveFileDialog();
            selectFile.FileName = "Output";
            /*
             * 0: Copy
             * 1: H264
             * 2: H265
             * 3: Quicktime
             * 4: FLV
             * 5: AVI
             * 6: WMV
            */
            selectFile.Filter = $"Copy Encoder| *{System.IO.Path.GetExtension(ctx.Source)}" +
                $"|H264 (.mp4)|*.mp4" +
                $"|H265 (.mp4)|*.mp4" +
                $"|Quicktime (.mov)|*.mov" +
                $"|AVI (.avi)|*.avi" +
                $"|Windows Media Video (.wmv)|*.wmv";
            bool? result = selectFile.ShowDialog(this);
            if (result.HasValue && result.Value && selectFile.CheckPathExists)
            {
                string v = System.IO.Path.GetFullPath(selectFile.FileName);
                Range[] range = new Range[ClipList.Items.Count];
                for(int i = 0; i < range.Length; i++)
                {
                    range[i] = (ClipList.Items[i] as ListBoxItem).Tag as Range;
                }
                ctx.Dist = v;
                int selectIndex = selectFile.FilterIndex - 1;
                string encoder = SelectEncoderDict[selectIndex];
                Header header = new Header()
                {
                    filename = ctx.Source,
                    output = v,
                    cleantemp = true,
                    temp = "temp"
                };
                Job[] twojob = new Job[2];
                Action[] cuts = new Action[range.Length];
                for(int i = 0; i < cuts.Length; i++)
                {
                    cuts[i] = new Action()
                    {
                        type = "Cut",
                        start = range[i].Start.ToString(@"hh\:mm\:ss"),
                        end = range[i].End.ToString(@"hh\:mm\:ss")
                    };
                }
                Debug.WriteLine($"codecs: {encoder}");
                Action mergeA = new Action()
                {
                    type = "Merge",
                    all = true,
                    encoder = encoder
                };
                twojob[0] = new Job()
                {
                    type = 1,
                    actions = new Actions() { action = new List<Action>(cuts) }
                };
                twojob[1] = new Job()
                {
                    type = 0,
                    action = mergeA
                };
                FileStructure root = new FileStructure()
                {
                    header = header,
                    jobs = new Jobs() { job = new List<Job>(twojob) }
                };
                XmlSerializer serializer = new XmlSerializer(typeof(FileStructure));
                XmlWriter xml_writer = XmlWriter.Create("config.xml");
                serializer.Serialize(xml_writer, root);
                xml_writer.Close();
                try
                {
                    Process proc = Process.Start("clipping_master.exe");
                    proc.WaitForExit();
                    Process.Start($"start {System.IO.Path.GetDirectoryName(ctx.Dist)}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GoToStart_Click(object sender, RoutedEventArgs e)
        {
            if (sourceProvider.IsPlaying)
            {
                sourceProvider.Pause();
                sourceProvider.Time = (long)ctx.SelectRange.Start.TotalMilliseconds;
                sourceProvider.Play();
            }
            else
            {
                sourceProvider.Time = (long)ctx.SelectRange.Start.TotalMilliseconds;
            }
        }

        private void GoToEnd_Click(object sender, RoutedEventArgs e)
        {
            if (sourceProvider.IsPlaying)
            {
                sourceProvider.Pause();
                sourceProvider.Time = (long)ctx.SelectRange.End.TotalMilliseconds;
                sourceProvider.Play();
            }
            else
            {
                sourceProvider.Time = (long)ctx.SelectRange.End.TotalMilliseconds;
            }
        }

        private void AddNewClip_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            ElementContextMenu(item);
            Range range = new Range();
            range.End = TimeSpan.FromMilliseconds(sourceProvider.Length);
            item.Tag = range;
            ClipList.Items.Add(item);

            UpdateList();
        }

        private void CleanAllClip_Click(object sender, RoutedEventArgs e)
        {
            ClipList.Items.Clear();
        }

        private void ElementContextMenu(ListBoxItem target)
        {
            Thickness padding = new Thickness(15, 3, 15, 3);
            MenuItem clone = new MenuItem();
            MenuItem up = new MenuItem();
            MenuItem down = new MenuItem();
            MenuItem remove = new MenuItem();

            clone.Padding = padding;
            up.Padding = padding;
            down.Padding = padding;
            remove.Padding = padding;

            clone.Header = "Clone";
            up.Header = "Up";
            down.Header = "Down";
            remove.Header = "Remove";

            clone.Click += Clone_Click;
            up.Click += Up_Click;
            down.Click += Down_Click;
            remove.Click += Remove_Click;
            clone.Tag = target;
            up.Tag = target;
            down.Tag = target;
            remove.Tag = target;

            target.ContextMenu = new ContextMenu();
            target.ContextMenu.Padding = padding;
            target.ContextMenu.Items.Add(clone);
            target.ContextMenu.Items.Add(up);
            target.ContextMenu.Items.Add(down);
            target.ContextMenu.Items.Add(remove);
        }

        private void Clone_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            ListBoxItem target = item.Tag as ListBoxItem;
            Range targetR = target.Tag as Range;

            ListBoxItem nitem = new ListBoxItem();
            ElementContextMenu(nitem);
            Range range = new Range();
            range.Start = targetR.Start;
            range.End = targetR.End;
            nitem.Tag = range;
            ClipList.Items.Add(nitem);
            UpdateList();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            ListBoxItem target = item.Tag as ListBoxItem;
            int index = ClipList.Items.IndexOf(target);
            if (index == 0) return;
            var a1 = ClipList.Items[index - 1];
            var a2 = ClipList.Items[index];
            ClipList.Items.Remove(a1);
            ClipList.Items.Remove(a2);
            ClipList.Items.Insert(index - 1, a1);
            ClipList.Items.Insert(index - 1, a2);
            UpdateList();
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            ListBoxItem target = item.Tag as ListBoxItem;
            int index = ClipList.Items.IndexOf(target);
            if (index == ClipList.Items.Count - 1) return;
            var a1 = ClipList.Items[index];
            var a2 = ClipList.Items[index + 1];
            ClipList.Items.Remove(a1);
            ClipList.Items.Remove(a2);
            ClipList.Items.Insert(index, a1);
            ClipList.Items.Insert(index, a2);
            UpdateList();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            ListBoxItem target = item.Tag as ListBoxItem;
            ClipList.Items.Remove(target);
            UpdateList();
        }

        private void ClipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ctx.ContentVisibility = (ClipList.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible);
            ctx.RangeVisibility = (ClipList.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible);
            if (ClipList.SelectedIndex != -1)
            {
                ctx.SelectRange = (ClipList.SelectedItem as ListBoxItem).Tag as Range;
            }
            else
            {
                ctx.SelectRange = null;
            }

            ctx.OnPropertyChanged("ContentVisibility");
            ctx.OnPropertyChanged("RangeVisibility");
            ctx.OnPropertyChanged("SelectRange");
        }

        private void UpdateList()
        {
            for (int i = 0; i < ClipList.Items.Count; i++)
            {
                ListBoxItem tar = ClipList.Items[i] as ListBoxItem;
                tar.Content = $"Clip {i}";
            }
        }
    }
}
