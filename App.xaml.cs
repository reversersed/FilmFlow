using FilmFlow.Login;
using FilmFlow.Registration;
using FilmFlow.MainWindow;
using System;
using System.Globalization;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FilmFlow.PasswordReset;

namespace FilmFlow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        LoginView loginView;
        ResetView resetView;
        RegistrationView registrationView;
        MainWindow.MainWindow mainWindow;
        IHost? ApplicationHost;
        public App()
        {
            ApplicationHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<FilmFlow.MainWindow.MainWindow>();
                    services.AddSingleton<FilmFlow.Models.IUserRepository, FilmFlow.Models.UserRepository>();
                })
                .Build();
        }
        public async void ApplicationStart(object sender, EventArgs e)
        {
            ResourceDictionary languageDictionary = new ResourceDictionary();
            if (FilmFlow.Properties.Settings.Default.Language.Length < 2)
            {
                FilmFlow.Properties.Settings.Default.Language = CultureInfo.CurrentCulture.Name;
                FilmFlow.Properties.Settings.Default.Save();
            }
            languageDictionary.Source = new Uri("..\\Lang\\Lang." + FilmFlow.Properties.Settings.Default.Language + ".xaml", UriKind.Relative);

            App.Current.Resources.MergedDictionaries.Add(languageDictionary);

            await ApplicationHost!.StartAsync();

            ShowLoginWindow(null);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            await ApplicationHost!.StopAsync();
            base.OnExit(e);
        }
        private void showMainWindow(object? obj)
        {
            mainWindow = ApplicationHost.Services.GetRequiredService<FilmFlow.MainWindow.MainWindow>();
            MainWindowViewModel viewModel = mainWindow.DataContext as MainWindowViewModel;
            viewModel.showStartWindow = () => { this.ShowLoginWindow(new object()); mainWindow.Close(); };
            mainWindow.Show();
            loginView.Close();
        }

        private void ShowRegistration(object? obj)
        {
            registrationView = new RegistrationView();
            RegistrationViewModel viewModel = registrationView.DataContext as RegistrationViewModel;
            viewModel.backToLoginWindow = ShowLoginWindow;
            registrationView.Show();
            loginView.Close();
        }
        private void ShowLoginWindow(object? obj)
        {
            loginView = new LoginView();
            LoginViewModel viewModel = loginView.DataContext as LoginViewModel;
            viewModel.showRegistrationWindow = ShowRegistration;
            viewModel.showAuthorizedWindow = showMainWindow;
            viewModel.showPasswordRecover = showPasswordRecover;
            if (viewModel.IsViewVisible)
                loginView.Show();

            registrationView?.Close();
            resetView?.Close();
        }

        private void showPasswordRecover(object? obj)
        {
            resetView = new ResetView();
            ResetViewModel viewModel = resetView.DataContext as ResetViewModel;
            viewModel.BackToLoginAction = ShowLoginWindow;
            resetView.Show();
            loginView.Close();
        }
    }
}
