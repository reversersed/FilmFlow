using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace FilmFlow.Registration
{
    public sealed class RegistrationViewModel : ViewModelBase
    {
        //Private properties
        private Visibility _emailCodeVisibility = Visibility.Collapsed;
        private Visibility _informationVisiblity = Visibility.Visible;
        private Regex validateEnglish = new Regex("^[A-Za-z\\d*$@^&_-]+$");
        private string _username { get; set; }
        private string _password { get; set; }
        private string _confirmPassword { get; set; }
        private string _errorMessage { get; set; }
        private string _codeErrorMessage { get; set; }
        private string _email { get; set; }
        private string _emailCode { get; set; }
        private int createdEmailCode { get; set; }

        //Public properties
        public Visibility EmailCodeVisibility {  get { return _emailCodeVisibility; } set { _emailCodeVisibility = value; OnPropertyChanged(nameof(EmailCodeVisibility)); } }
        public Visibility InformationVisiblity { get { return _informationVisiblity; } set { _informationVisiblity = value; OnPropertyChanged(nameof(InformationVisiblity)); } }
        public string Username { get { return _username; } set { 
                if (value != null && value.Length > 0 && !validateEnglish.IsMatch(value)) 
                    return;
                _username = value; 
                OnPropertyChanged(nameof(Username)); 
            } 
        }
        public string Password { get { return _password; } set {
                if (value != null && value.Length > 0 && !validateEnglish.IsMatch(value))
                    return;
                _password = value; 
                OnPropertyChanged(nameof(Password)); 
            } 
        }
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        public string CodeErrorMessage { get { return _codeErrorMessage; } set { _codeErrorMessage = value; OnPropertyChanged(nameof(CodeErrorMessage)); } }
        public string ConfirmPassword { get { return _confirmPassword; } set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); } }
        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(nameof(Email)); } }
        public string EmailCode { get { return _emailCode; } set 
            {
                if (value == null || int.TryParse(value, out int n) || value.Length < 1)
                {
                    _emailCode = value;
                    if (EmailCode != null && EmailCode.Length == 6)
                        ValidateEmailCode(Convert.ToInt32(EmailCode));
                    else
                        CodeErrorMessage = null;
                    OnPropertyChanged(nameof(EmailCode));
                }
            } 
        }
        
        //Commands
        public ICommand executeSigning { get; }
        
        public ICommand BackToLogin { get; }
        public ICommand CloseApplication { get; }
        public ICommand ReturnToRegistration { get; }

        //Actions
        public Action<object?> backToLoginWindow;

        //Models
        IUserRepository userRepository;

        //Methods
        public RegistrationViewModel()
        {
            userRepository = new UserRepository();
            executeSigning = new ViewModelCommand(ExecuteSigning, canExecuteSigning);
            BackToLogin = new ViewModelCommand(BackToLoginCommand);
            CloseApplication = new ViewModelCommand(CloseApplicationCommand);
            ReturnToRegistration = new ViewModelCommand(ReturnToRegistrationCommand);
        }

        private void ReturnToRegistrationCommand(object obj)
        {
            EmailCodeVisibility = Visibility.Collapsed;
            InformationVisiblity = Visibility.Visible;

            CodeErrorMessage = null;
            ErrorMessage = null;
            EmailCode = null;
        }

        private void CloseApplicationCommand(object obj)
        {
            Application.Current.Shutdown();
        }

        private void BackToLoginCommand(object obj)
        {
            backToLoginWindow?.Invoke(obj);
        }
        private bool canExecuteSigning(object obj)
        {
            if (EmailCodeVisibility == Visibility.Visible || string.IsNullOrEmpty(Username) || Username.Length < 2 || ConfirmPassword == null || ConfirmPassword.Length < 2 || Password == null || Password.Length < 6 || Email == null || Email.Length < 5)
                return false;
            return true;
        }
        private void ExecuteSigning(object obj)
        {
            Regex passwordValidation = new Regex("^(?=.*?[A-Z])(?=.*?[a-z]).{6,}$");
            if(!passwordValidation.IsMatch(Password))
            {
                ErrorMessage = Application.Current.FindResource("NotValidatedPassword") as string;
                return;
            }
            if (!Password.Equals(ConfirmPassword))
            {
                ErrorMessage = Application.Current.FindResource("PasswordsNotEqual") as string;
                return;
            }
            if(!IsValidEmail(Email))
            {
                ErrorMessage = Application.Current.FindResource("NotValidEmail") as string;
                return;
            }
            if(!userRepository.isUniqueUser(Username))
            {
                ErrorMessage = Application.Current.FindResource("NotUniqueUser") as string;
                return;
            }
            if (!userRepository.isUniqueEmail(Email))
            {
                ErrorMessage = Application.Current.FindResource("NotUniqueEmail") as string;
                return;
            }
            createdEmailCode = new Random().Next(100000, 1000000);
            if(!SmtpModel.sendEmail(Email, Application.Current.FindResource("ConfirmationSubject") as string, string.Format(Application.Current.FindResource("ConfirmationMessage") as string, createdEmailCode)))
            {
                ErrorMessage = Application.Current.FindResource("EmailNotRespond") as string;
                return;
            }

            ErrorMessage = null;
            EmailCodeVisibility = Visibility.Visible;
            InformationVisiblity = Visibility.Collapsed;
        }
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        private void ValidateEmailCode(int code)
        {
            if(code != createdEmailCode)
            {
                CodeErrorMessage = Application.Current.FindResource("WrongEmailCode") as string;
                return;
            }
            userRepository.createUser(new Models.BaseTables.User() { 
                Username = this.Username, 
                Password = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(this.Password)), 
                Email = this.Email 
            });
            BackToLogin.Execute(Username);
        }
    }
}
