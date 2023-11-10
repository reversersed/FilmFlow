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

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    /// <summary>
    /// Логика взаимодействия для MovieView.xaml
    /// </summary>
    public partial class MovieView : UserControl
    {
        public MovieView()
        {
            InitializeComponent();
        }

        private void BarMoved(object sender, MouseEventArgs e)
        {
            (this.DataContext as MovieViewModel)?.MouseRatingMoved.Invoke(null, (float)((e.GetPosition(sender as ProgressBar).X/(sender as ProgressBar).Width)*5));
        }

        private void BarLeaved(object sender, MouseEventArgs e)
        {
            (this.DataContext as MovieViewModel)?.RatingChanged.Invoke(null, false);
        }

        private void BarClicked(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                (this.DataContext as MovieViewModel)?.RatingChanged.Invoke(null, true);
        }
    }
}
