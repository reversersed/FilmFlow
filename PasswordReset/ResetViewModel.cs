using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.PasswordReset
{
    public class ResetViewModel : ViewModelBase
    {
        //Private properties
        private Visibility _emailCodeVisibility = Visibility.Collapsed;
        private Visibility _newPasswordVisibility = Visibility.Collapsed;
        private Regex validateEnglish = new Regex("^[A-Za-z\\d*$@^&_-]+$");
        private string _emailCode {  get; set; }
        private int createdEmailCode = -1;
        private string _codeErrorMessage {  get; set; }
        private string _username {  get; set; }
        private string _errorMessage { get; set; }
        private string _buttonStatus { get; set; } = Application.Current.FindResource("SendEmailButton") as string;
        private string _password { get; set; }
        private string _confirmPassword { get; set; }
        private Visibility _mainPanelVisibility { get; set; } = Visibility.Visible;
        private Visibility _successChangedVisibility { get; set; } = Visibility.Collapsed;

        //Public properties
        public Visibility EmailCodeVisibility { get { return _emailCodeVisibility; } set {  _emailCodeVisibility = value; OnPropertyChanged(nameof(EmailCodeVisibility)); } }
        public Visibility NewPasswordVisibility { get { return _newPasswordVisibility; } set { _newPasswordVisibility = value; OnPropertyChanged(nameof(NewPasswordVisibility)); } }
        public Visibility MainPanelVisibility { get { return _mainPanelVisibility; } set { _mainPanelVisibility = value; OnPropertyChanged(nameof(MainPanelVisibility)); } }
        public Visibility SuccessChangedVisibility { get { return _successChangedVisibility; } set { _successChangedVisibility = value; OnPropertyChanged(nameof(SuccessChangedVisibility)); } }
        public string EmailCode
        {
            get { return _emailCode; }
            set
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
        public string CodeErrorMessage { get { return _codeErrorMessage; } set { _codeErrorMessage = value; OnPropertyChanged(nameof(CodeErrorMessage)); } }
        public string Username { get { return _username; } set { if(_emailCodeVisibility == Visibility.Collapsed && _newPasswordVisibility == Visibility.Collapsed) _username = value; OnPropertyChanged(nameof(Username)); } }
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        public string ButtonStatus { get { return _buttonStatus; } set { _buttonStatus = value; OnPropertyChanged(nameof(ButtonStatus)); } }
        public string Password
        {
            get { return _password; }
            set
            {
                if (value != null && value.Length > 0 && !validateEnglish.IsMatch(value))
                    return;
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ConfirmPassword { get { return _confirmPassword; } set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); } }

        //Commands
        public ICommand BackToLogin { get; }
        public ICommand CloseApplication { get; }
        public ICommand ResetButton { get; }

        //Actions
        public Action<object?> BackToLoginAction;

        //Models
        private UserRepository user {  get; set; }
        private SmtpModel smtpModel;

        //Methods
        public ResetViewModel()
        {
            BackToLogin = new ViewModelCommand(BackToLoginCommand);
            CloseApplication = new ViewModelCommand(CloseApplicationCommand);
            ResetButton = new ViewModelCommand(ResetButtonCommand);

            user = new UserRepository();
            smtpModel = new SmtpModel();
        }

        private async void ResetButtonCommand(object obj)
        {
            if (NewPasswordVisibility == Visibility.Visible)
            {
                if(Password == null || ConfirmPassword == null)
                    return;
                Regex passwordValidation = new Regex("^(?=.*?[A-Z])(?=.*?[a-z]).{6,}$");
                if (!passwordValidation.IsMatch(Password))
                {
                    ErrorMessage = Application.Current.FindResource("NotValidatedPassword") as string;
                    return;
                }
                if (!Password.Equals(ConfirmPassword))
                {
                    ErrorMessage = Application.Current.FindResource("PasswordsNotEqual") as string;
                    return;
                }
                var loadedUser = user.GetByEmailOrUsername(Username);
                user.ChangePassword(loadedUser.Username, Password);

                MainPanelVisibility = Visibility.Collapsed;
                SuccessChangedVisibility = Visibility.Visible;

                await Task.Delay(3000);
                BackToLogin?.Execute(null);
            }
            else if (EmailCodeVisibility == Visibility.Collapsed)
            {
                var loadedUser = user.GetByEmailOrUsername(Username);
                if(loadedUser == default(User))
                {
                    ErrorMessage = Application.Current.FindResource("rpUserNotFound") as string;
                    return;
                }
                createdEmailCode = new Random().Next(100000, 1000000);
                if (!smtpModel.sendEmail(loadedUser.Email, Application.Current.FindResource("rpMessageTopic") as string, string.Format(Application.Current.FindResource("rpMessageBody") as string,loadedUser.Username,createdEmailCode)))
                {
                    ErrorMessage = Application.Current.FindResource("EmailNotRespond") as string;
                    return;
                }
                ErrorMessage = null;
                ButtonStatus = Application.Current.FindResource("ResetPasswordButton") as string;
                EmailCodeVisibility = Visibility.Visible;
            }
        }

        private void CloseApplicationCommand(object obj)
        {
            Application.Current.Shutdown();
        }

        private void BackToLoginCommand(object obj)
        {
            BackToLoginAction?.Invoke(null);
        }
        private void ValidateEmailCode(int code)
        {
            if (code != createdEmailCode)
            {
                CodeErrorMessage = Application.Current.FindResource("WrongEmailCode") as string;
                return;
            }
            NewPasswordVisibility = Visibility.Visible;
            EmailCodeVisibility = Visibility.Collapsed;
        }
    }
}
