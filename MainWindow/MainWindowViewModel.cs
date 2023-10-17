using FilmFlow.MainWindow.NavigationViews.HomeView;
using FilmFlow.MainWindow.NavigationViews.SettingsView;
using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilmFlow.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        //Private properties
        private ViewModelBase _childContentView;
        private bool _logoutConfirmVisibility { get; set; } = false;

        //Child Views
        private ViewModelBase homeView;
        private ViewModelBase settingsView;

        //Public properties
        public ViewModelBase ChildContentView { get { return _childContentView; } set { _childContentView = value; OnPropertyChanged(nameof(ChildContentView)); } }
        public bool LogoutConfirmVisibility { get { return _logoutConfirmVisibility;  }  set { _logoutConfirmVisibility = value; OnPropertyChanged(nameof(LogoutConfirmVisibility)); } }
        //Commands
        public ICommand LogoutButton { get; }
        public ICommand ShowHomeSection { get; }
        public ICommand ShowSettingsSection { get; }

        //Repositories
        UserRepository UserRepository { get; set; }
        public User User {  get; set; }

        //Actions
        public Action showStartWindow { get; set; }

        //Methods
        public MainWindowViewModel()
        {
            LogoutButton = new ViewModelCommand(LogoutButtonCommand);
            ShowHomeSection = new ViewModelCommand(ShowHomeSectionCommand);
            ShowSettingsSection = new ViewModelCommand(ShowSettingsSectionCommand);

            UserRepository = new UserRepository();
            User = UserRepository.LoadUserData(Thread.CurrentPrincipal.Identity.Name);

            homeView = new HomeViewModel();
            settingsView = new SettingsViewModel();
            ChildContentView = homeView;
        }

        private void ShowHomeSectionCommand(object obj) => ChildContentView = homeView;
        private void ShowSettingsSectionCommand(object obj) => ChildContentView = settingsView;

        private void LogoutButtonCommand(object obj)
        {
            if (obj == null)//Button pressed first
            {
                LogoutConfirmVisibility = true;
            }
            else if (obj.Equals("returnBack"))//Action 'no' on confirmation window
            {
                LogoutConfirmVisibility = false;
            }
            else if (obj.Equals("logout"))//Action 'yes' on confirmation window
            {
                UserRepository.Logout(User);
                showStartWindow?.Invoke();
            }
        }
    }
}
