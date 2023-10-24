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

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer viewer = (ScrollViewer)sender;
            if (e.Delta > 0)
                viewer.LineLeft();
            else
                viewer.LineRight();
            e.Handled = true;
        }
    }
}
