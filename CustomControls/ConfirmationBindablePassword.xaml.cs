using System.Windows;
using System.Windows.Controls;

namespace FilmFlow.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для ConfirmationBindablePassword.xaml
    /// </summary>
    public partial class ConfirmationBindablePassword : UserControl
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(ConfirmationBindablePassword));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public ConfirmationBindablePassword()
        {
            InitializeComponent();
            tbConfirmPasswordBox.PasswordChanged += OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = tbConfirmPasswordBox.Password;
        }
    }
}
