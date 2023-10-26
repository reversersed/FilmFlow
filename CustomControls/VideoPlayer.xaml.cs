using System;
using System.Collections.Generic;
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
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("SourceUrl", typeof(string), typeof(VideoPlayer));
        public string SourceUrl { get { return (string)GetValue(SourceProperty); } 
            set { SetValue(SourceProperty, value); } }

        public VideoPlayer()
        {
            InitializeComponent();

            VideoPlayerElement.LoadedBehavior = MediaState.Manual;
            VideoPlayerElement.UnloadedBehavior = MediaState.Manual;
            PlayButton.Click += PlayCommand;
        }


        private void PlayCommand(object sender, RoutedEventArgs e)
        {
            if(IconButtonPlay.Icon == FontAwesome.Sharp.IconChar.Play)
            {
                IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Pause;
                VideoPlayerElement.Play();
            }
            else
            {

                IconButtonPlay.Icon = FontAwesome.Sharp.IconChar.Play;
                VideoPlayerElement.Pause();
            }
        }
    }
}
