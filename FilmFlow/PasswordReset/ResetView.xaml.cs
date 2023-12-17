using System.Windows;
using System.Windows.Input;

namespace FilmFlow.PasswordReset
{
    /// <summary>
    /// Логика взаимодействия для ResetView.xaml
    /// </summary>
    public partial class ResetView : Window
    {
        public ResetView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
