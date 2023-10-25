using System.Windows.Controls;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    /// <summary>
    /// Логика взаимодействия для HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ((ScrollViewer)sender).ScrollToVerticalOffset(((ScrollViewer)sender).VerticalOffset - e.Delta*0.5);
            e.Handled = true;
        }
    }
}
