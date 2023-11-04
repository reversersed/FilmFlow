using FilmFlow.Models;
using System.Windows;

namespace FilmFlow.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IUserRepository userRepository)
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(userRepository);
        }
    }
}
