using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilmFlow.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("SourceUrl", typeof(string), typeof(VideoPlayer),
            new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        static Action loadVideo;
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => loadVideo?.Invoke();
        public string SourceUrl { get { return (string)GetValue(SourceProperty); } 
            set { SetValue(SourceProperty, value);  } }

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
            SetEnabledStateToControls(false);
            VideoPlayerElement.MediaOpened += VideoPlayerElement_MediaOpened;
            VideoPlayerElement.MediaFailed += VideoPlayerElement_MediaFailed;
        }

        private void VideoPlayerElement_MediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Play;
            SetEnabledStateToControls(false);
        }

        private void VideoPlayerElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            SetEnabledStateToControls(true);
        }
        private void SetEnabledStateToControls(bool enabled)
        {
            PlayButton.IsEnabled = enabled;
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
    }
}
