using FilmFlow.Models;
using System;
using System.Windows.Input;
using System.Security.Principal;
using System.Windows;
using FilmFlow.ViewModels;
using System.Text.RegularExpressions;
using FilmFlow.Models.BaseTables;

namespace FilmFlow.Login
{
    class LoginViewModel : ViewModelBase
    {
        //Private properties
        private Regex validateEnglish = new Regex("^[A-Za-z\\d*.$@^&_-]+$");
        private string _username { get; set; }
        private string _password { get; set; }
        private string _errorMessage { get; set; }
        private bool _isViewVisible = true;
        private bool _isPasswordRemembered = false;

        //Public properties
        public string Username { get { return _username; } 
            set {
                if (value != null && value.Length > 0 && !validateEnglish.IsMatch(value))
                    return;
                _username = value; 
                OnPropertyChanged(nameof(Username)); 
            } 
        }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        public bool IsViewVisible { get { return _isViewVisible; } set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); } }
        public bool IsPasswordRemembered { get { return _isPasswordRemembered; } set { _isPasswordRemembered = value; OnPropertyChanged(nameof(IsPasswordRemembered)); } }

        //Commands
        public ICommand LoginUser { get; }
        public ICommand RecoverPassword { get; }
        public ICommand ShowRegistration { get; }
        public ICommand CloseApplication { get; }
        public ICommand LoginViewLoaded { get; }

        //Actions
        public Action<object?> showRegistrationWindow;
        public Action<object?> showAuthorizedWindow;
        public Action<object?> showPasswordRecover;

        //Models
        IUserRepository userRepository;

        //Methods
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginUser = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLogin);
            RecoverPassword = new ViewModelCommand(RecoverPasswordCommand);
            ShowRegistration = new ViewModelCommand(ShowRegistrationCommand);
            CloseApplication = new ViewModelCommand(CloseApplicationCommand);
            LoginViewLoaded = new ViewModelCommand(LoginViewLoadedCommand);
        }

        private void LoginViewLoadedCommand(object obj)
        {
            if((Username = userRepository.AuthenticateUser()) != null)
            {
                System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;

                showAuthorizedWindow?.Invoke(Username);
            }
        }

        private void CloseApplicationCommand(object obj) => Application.Current.Shutdown();
        private void ShowRegistrationCommand(object obj) => showRegistrationWindow?.Invoke(obj);
        private void RecoverPasswordCommand(object obj) => showPasswordRecover?.Invoke(obj);

        private bool CanExecuteLogin(object obj)
        {
            return !(string.IsNullOrEmpty(Username) || Username.Length < 4 || Password == null || Password.Length < 6);
        }

        private void ExecuteLoginCommand(object obj)
        {
            User User = userRepository.AuthenticateUser(Username, Password, IsPasswordRemembered);
            if (User != default(User))
            {
                System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(User.Username), null);
                IsViewVisible = false;

                showAuthorizedWindow?.Invoke(User.Username);
            }
            else
            {
                ErrorMessage = Application.Current.FindResource("ErrorLoginMessage") as string;
            }
        }
    }
}
