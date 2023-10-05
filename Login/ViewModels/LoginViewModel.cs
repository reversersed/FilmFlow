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

namespace FilmFlow.Login.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
        private string _username { get; set; }
        private string _password { get; set; }
        private string _errorMessage { get; set; }
        private bool _isViewVisible = true;
        
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        public bool IsViewVisible { get { return _isViewVisible; } set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); } }

        public ICommand LoginUser { get; }
        public ICommand RecoverPassword { get; }
        public ICommand ShowRegistration { get; }

        IUserRepository userRepository;

        public Action<object?> showRegistrationWindow;
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginUser = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLogin);
            RecoverPassword = new ViewModelCommand(RecoverPasswordCommand);
            ShowRegistration = new ViewModelCommand(ShowRegistrationCommand);
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
            bool isValidUser = userRepository.AuthenticateUser(Username, Password);
            if (isValidUser)
            {
                System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;
            }
            else
            {
                ErrorMessage = Application.Current.FindResource("ErrorLoginMessage") as string;
            }
        }
    }
}
