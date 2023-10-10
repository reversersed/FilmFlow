using FilmFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Security.Principal;
using System.Windows;
using FilmFlow.ViewModels;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;

namespace FilmFlow.Login
{
    class LoginViewModel : ViewModelBase
    {
        private Regex validateEnglish = new Regex("^[A-Za-z\\d*$@^&_-]+$");
        private string _username { get; set; }
        private string _password { get; set; }
        private string _errorMessage { get; set; }
        private bool _isViewVisible = true;
        private bool _isPasswordRemembered = false;

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

        public ICommand LoginUser { get; }
        public ICommand RecoverPassword { get; }
        public ICommand ShowRegistration { get; }
        public ICommand CloseApplication { get; }
        public ICommand LoginViewLoaded { get; }

        IUserRepository userRepository;

        public Action<object?> showRegistrationWindow;
        private Action<object?> _showAuthorizedWindow;
        public Action<object?> showAuthorizedWindow { get { return _showAuthorizedWindow; } set { _showAuthorizedWindow = value; } }
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

        private void CloseApplicationCommand(object obj)
        {
            Application.Current.Shutdown();
        }

        private void ShowRegistrationCommand(object obj)
        {
            showRegistrationWindow?.Invoke(obj);
        }

        private void RecoverPasswordCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanExecuteLogin(object obj)
        {
            if(string.IsNullOrEmpty(Username) || Username.Length < 4 || Password == null || Password.Length < 6) 
                return false;
            return true;
        }

        private void ExecuteLoginCommand(object obj)
        {
            bool isValidUser = userRepository.AuthenticateUser(Username, Password, IsPasswordRemembered);
            if (isValidUser)
            {
                System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;

                showAuthorizedWindow?.Invoke(Username);
            }
            else
            {
                ErrorMessage = Application.Current.FindResource("ErrorLoginMessage") as string;
            }
        }
    }
}
