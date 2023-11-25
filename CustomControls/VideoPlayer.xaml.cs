using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        public static readonly DependencyProperty subscribedProperty = DependencyProperty.Register("Subscribed", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(true));

        static Action loadVideo;
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => loadVideo?.Invoke();
        public string SourceUrl { get { return (string)GetValue(SourceProperty); } 
            set { SetValue(SourceProperty, value);  } }
        public string ElapsedTime { get { return (string)GetValue(elapsedProperty); } set { SetValue(elapsedProperty, value); } }
        public bool Subscribed { get { return (bool)GetValue(subscribedProperty); } set { SetValue(subscribedProperty, value); } }

        private double savedVolume;
        private DispatcherTimer videoTimer;

        private Window fullscreen;
        private Grid fullscreen_parent;
        private Uri fullscreen_source = new Uri("https://www.google.com");
        private Grid fullscreen_grid;
        private Size originalPlayerSize;

        private System.Windows.Forms.Timer mouseIdling = new System.Windows.Forms.Timer();
        public VideoPlayer()
        {
            InitializeComponent();
            loadVideo = new Action(() => { 
                if(SourceUrl != null && !SourceUrl.Equals(fullscreen_source.OriginalString))
                {
                    Uri newSource;
                    if(Uri.TryCreate(SourceUrl, UriKind.Absolute, out newSource))
                    {
                        fullscreen_source = newSource;
                        this.VideoPlayerElement.Source = fullscreen_source;
                    }
                }

                VideoPlayerElement.Play();
                VideoPlayerElement.Pause();
            });

            VideoPlayerElement.LoadedBehavior = MediaState.Manual;
            VideoPlayerElement.UnloadedBehavior = MediaState.Manual;
            PlayButton.Click += PlayCommand;
            PauseButton.Click += PlayCommand;
            FullscreenButton.Click += FullscreenCommand;
            ElapsedTime = "00:00:00/00:00:00";
            iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeMute;
            SetEnabledStateToControls(false);
            VideoPlayerElement.MediaOpened += VideoPlayerElement_MediaOpened;
            VideoPlayerElement.MediaFailed += VideoPlayerElement_MediaFailed;
            VideoPlayerElement.MediaEnded += VideoPlayerElement_MediaEnded;
            mouseIdling.Tick += new EventHandler(
                (object s, EventArgs e) => 
                {
                    this.PlayerControl.Cursor = Cursors.None;
                });
            mouseIdling.Interval = 3000;
        }

        private void VideoPlayerElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (fullscreen != null)
                EnableFullscreen();
            TimeSlider.Value = 0;
            VideoPlayerElement.Position = TimeSpan.FromSeconds(TimeSlider.Value);
            PlayCommand(sender, e);
        }

        private void FullscreenCommand(object sender, RoutedEventArgs e) => EnableFullscreen();

        private void VideoPlayerElement_MediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            videoTimer?.Stop();
            ElapsedTime = "00:00:00/00:00:00";
            iconVolume.Icon = FontAwesome.Sharp.IconChar.VolumeMute;
            VolumeSlider.Value = 0;
            TimeSlider.Value = 0;
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
            SetEnabledStateToControls(false);
        }

        private void VideoPlayerElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSlider.Maximum = VideoPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
            VideoPlayerElement.Volume = VolumeSlider.Value = VolumeSlider.Maximum = 1;
            VolumeChange(VideoPlayerElement.Volume);
            videoTimer = new DispatcherTimer();
            videoTimer.Interval = TimeSpan.FromSeconds(1);
            videoTimer.Tick += VideoTimer_Tick;
            videoTimer.Start();
            ElapsedTime = string.Format("{0}/{1}", VideoPlayerElement.Position.ToString(@"hh\:mm\:ss"), VideoPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
            SetEnabledStateToControls(Subscribed);
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
            VideoPlayerElement.Volume = VolumeSlider.Value;
            ElapsedTime = string.Format("{0}/{1}", VideoPlayerElement.Position.ToString(@"hh\:mm\:ss"), VideoPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
        }

        private void SetEnabledStateToControls(bool enabled)
        {
            NotSubscribed.Visibility = Subscribed ? Visibility.Collapsed : Visibility.Visible;
            PlayButton.IsEnabled = enabled;
            PauseButton.IsEnabled = enabled;
            TimeSlider.IsEnabled = enabled;
            VolumeSlider.IsEnabled = enabled;
            volumeButton.IsEnabled = enabled;
        }
        private void PlayCommand(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayerElement.HasVideo)
                return;
            if(PlayButton.Visibility == Visibility.Visible)
            {
                PlayButton.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Visible;
                VideoPlayerElement.Play();
            }
            else
            {
                PauseButton.Visibility = Visibility.Collapsed;
                PlayButton.Visibility = Visibility.Visible;
                VideoPlayerElement.Pause();
            }
        }
        private void EnableFullscreen()
        {
            if (fullscreen == null)
            {
                fullscreen_parent = (this.Parent as Grid);
                fullscreen_parent.Children.Remove(this);

                fullscreen = new Window();
                fullscreen_grid = new Grid();
                fullscreen.Content = fullscreen_grid;
                fullscreen_grid.Children.Add(this);
                this.VideoPlayerElement.Source = fullscreen_source;
                this.BorderThickness = new Thickness(0);

                originalPlayerSize.Width = this.Width;
                originalPlayerSize.Height = this.Height;

                this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                fullscreen.WindowStyle = WindowStyle.None;
                fullscreen.WindowState = WindowState.Maximized;
                fullscreen.ResizeMode = ResizeMode.NoResize;
                fullscreen.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
                fullscreen.Topmost = true;

                fullscreen.Show();
                fullscreen.Deactivated += DeactivateFullscreen;
                fullscreen.LostFocus += DeactivateFullscreen;
                fullscreen.KeyDown += FullScreen_KeyDown;

                FullscreenIcon.Icon = FontAwesome.Sharp.IconChar.Minimize;

                fullscreen.Focus();
            }
            else
            {
                fullscreen_grid.Children.Remove(this);

                fullscreen.Deactivated -= DeactivateFullscreen;
                fullscreen.LostFocus -= DeactivateFullscreen;
                fullscreen.KeyDown -= FullScreen_KeyDown;

                fullscreen.Close();
                fullscreen = null;

                fullscreen_parent.Children.Add(this);

                this.Width = originalPlayerSize.Width;
                this.Height = originalPlayerSize.Height;
                this.BorderThickness = new Thickness(1);

                FullscreenIcon.Icon = FontAwesome.Sharp.IconChar.Expand;
            }
            if(PlayButton.Visibility != Visibility.Visible)
                this.VideoPlayerElement.Play();
        }
        private void DeactivateFullscreen(object? sender, EventArgs e)
        {
            if (fullscreen != null)
                EnableFullscreen();
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

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            this.PlayerControl.Cursor = Cursors.Arrow;
            if (mouseIdling.Enabled)
                mouseIdling.Stop();
            mouseIdling.Start();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIdling.Enabled)
                mouseIdling.Start();
            this.PlayerControl.Cursor = Cursors.Arrow;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if(mouseIdling.Enabled)
                mouseIdling.Stop();
            this.PlayerControl.Cursor = Cursors.Arrow;
        }

        private void FullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                DeactivateFullscreen(sender, new EventArgs());
            if(e.Key == Key.F11)
                EnableFullscreen();
            if(e.Key == Key.Space)
                PlayCommand(sender, null);
            if(e.Key == Key.M)
                volumeButton_Click(sender, e);
        }

        private void VolumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => VolumeChange(((Slider)sender).Value);
    }
}
