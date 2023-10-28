using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FilmFlow.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("SourceUrl", typeof(string), typeof(VideoPlayer),
            new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));
        public static readonly DependencyProperty elapsedProperty = DependencyProperty.Register("ElapsedTime", typeof(string), typeof(VideoPlayer));

        static Action loadVideo;
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => loadVideo?.Invoke();
        public string SourceUrl { get { return (string)GetValue(SourceProperty); } 
            set { SetValue(SourceProperty, value);  } }
        public string ElapsedTime { get { return (string)GetValue(elapsedProperty); } set { SetValue(elapsedProperty, value); } }

        private double savedVolume;
        private DispatcherTimer videoTimer;
        //private Window fullscreen;
        public VideoPlayer()
        {
            InitializeComponent();
            loadVideo = new Action(() => { 
                VideoPlayerElement.Play();
                VideoPlayerElement.Pause();
            });

            VideoPlayerElement.LoadedBehavior = MediaState.Manual;
            VideoPlayerElement.UnloadedBehavior = MediaState.Manual;
            PlayButton.Click += PlayCommand;
            ElapsedTime = "00:00:00/00:00:00";
            iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeMute;
            SetEnabledStateToControls(false);
            VideoPlayerElement.MediaOpened += VideoPlayerElement_MediaOpened;
            VideoPlayerElement.MediaFailed += VideoPlayerElement_MediaFailed;
        }

        private void VideoPlayerElement_MediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            videoTimer?.Stop();
            ElapsedTime = "00:00:00/00:00:00";
            iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeMute;
            VolumeSlider.Value = 0;
            IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Play;
            SetEnabledStateToControls(false);
        }

        private void VideoPlayerElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSlider.Maximum = VideoPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
            VolumeSlider.Maximum = 1;
            VolumeSlider.Value = VideoPlayerElement.Volume;
            VolumeChange(VideoPlayerElement.Volume);
            videoTimer = new DispatcherTimer();
            videoTimer.Interval = TimeSpan.FromSeconds(1);
            videoTimer.Tick += VideoTimer_Tick;
            videoTimer.Start();
            ElapsedTime = string.Format("{0}/{1}", VideoPlayerElement.Position.ToString(@"hh\:mm\:ss"), VideoPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
            SetEnabledStateToControls(true);
        }

        private void VideoTimer_Tick(object? sender, EventArgs e)
        {
            if (!VideoPlayerElement.HasVideo || !VideoPlayerElement.NaturalDuration.HasTimeSpan)
            {
                videoTimer.Stop();
                return;
            }
            if (VideoPlayerElement.NaturalDuration.TimeSpan.TotalSeconds > 0 && VideoPlayerElement.Position.TotalSeconds > 0)
                TimeSlider.Value = VideoPlayerElement.Position.TotalSeconds;
            VolumeSlider.Value = VideoPlayerElement.Volume;
            ElapsedTime = string.Format("{0}/{1}", VideoPlayerElement.Position.ToString(@"hh\:mm\:ss"), VideoPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
        }

        private void SetEnabledStateToControls(bool enabled)
        {
            PlayButton.IsEnabled = enabled;
            TimeSlider.IsEnabled = enabled;
            VolumeSlider.IsEnabled = enabled;
            volumeButton.IsEnabled = enabled;
        }
        private void PlayCommand(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayerElement.HasVideo)
                return;
            if(IconButtonPlay.Icon == FontAwesome.Sharp.IconChar.Play)
            {
                IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Pause;
                VideoPlayerElement.Play();

                //(this.Parent as StackPanel).Children.Remove(this);
                //fullscreen = new Window();
                //fullscreen.Content = this;
                //fullscreen.WindowStyle = WindowStyle.None;
                //fullscreen.WindowState = WindowState.Maximized;
                //fullscreen.ResizeMode = ResizeMode.NoResize;
                //fullscreen.Show();
            }
            else
            {
                IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Play;
                VideoPlayerElement.Pause();
            }
        }

        private void TimeSlider_ValueChanged(object sender, MouseButtonEventArgs e)
        {
            if (VideoPlayerElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
                VideoPlayerElement.Position = TimeSpan.FromSeconds(TimeSlider.Value);
        }

        private void TimeSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (VideoPlayerElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                TimeSlider.Value += Math.Sign(e.Delta) * 5;
                VideoPlayerElement.Position = TimeSpan.FromSeconds(TimeSlider.Value);
            }
        }
        private void VolumeSlider_ValueChanged(object sender, MouseButtonEventArgs e) => VolumeChange(VolumeSlider.Value);

        private void VolumeSlider_MouseWheel(object sender, MouseWheelEventArgs e) => VolumeChange(VolumeSlider.Value+ Math.Sign(e.Delta)*0.1);
        private void VolumeChange(double value)
        {
            if (value > 0.4)
                iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeUp;
            else if (value > 0.01)
                iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeDown;
            else
            {
                savedVolume = VideoPlayerElement.Volume;
                iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeMute;
            }
            VideoPlayerElement.Volume = value;
            VolumeSlider.Value = value;
        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            if(VideoPlayerElement.Volume > 0)
                VolumeChange(0);
            else
                VolumeChange(savedVolume);
        }

        private void VideoPlayerElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => PlayCommand(sender, null);
    }
}
