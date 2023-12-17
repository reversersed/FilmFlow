using FilmFlow.MainWindow.NavigationViews.HomeView;
using FilmFlow.MainWindow.NavigationViews.SettingsView;
using FilmFlow.MainWindow.NavigationViews.ModerationView;
using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using FilmFlow.MainWindow.NavigationViews.MovieView;
using FilmFlow.MainWindow.NavigationViews.FavouriteView;
using FilmFlow.MainWindow.NavigationViews.ProfileView;

namespace FilmFlow.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        //Private properties
        private ViewModelBase _childContentView;
        private bool _logoutConfirmVisibility { get; set; } = false;
        private Visibility _userAccessVisibility { get; set; } = Visibility.Collapsed;

        //Child Views
        private ViewModelBase settingsView;
        private ViewModelBase moderationView;
        private ViewModelBase profileView;

        //Public properties
        public ViewModelBase ChildContentView { get { return _childContentView; } set { _childContentView = value; OnPropertyChanged(nameof(ChildContentView)); } }
        public bool LogoutConfirmVisibility { get { return _logoutConfirmVisibility; } set { _logoutConfirmVisibility = value; OnPropertyChanged(nameof(LogoutConfirmVisibility)); } }
        public Visibility UserAccessVisibility { get { return _userAccessVisibility; } set { _userAccessVisibility = value; OnPropertyChanged(nameof(UserAccessVisibility)); } }

        //Commands
        public ICommand LogoutButton { get; }
        public ICommand ShowHomeSection { get; }
        public ICommand ShowSettingsSection { get; }
        public ICommand ShowAdminSection { get; }
        public ICommand ShowFavouriteSection { get; }
        public ICommand ShowProfileSection { get; }

        //Models
        IUserRepository UserRepository { get; set; }
        public User User { get; set; }

        //Actions
        public Action showStartWindow { get; set; }

        //Methods
        public MainWindowViewModel()
        {
            LogoutButton = new ViewModelCommand(LogoutButtonCommand);
            ShowHomeSection = new ViewModelCommand(ShowHomeSectionCommand);
            ShowSettingsSection = new ViewModelCommand(ShowSettingsSectionCommand);
            ShowAdminSection = new ViewModelCommand(ShowAdminSectionCommand);
            ShowFavouriteSection = new ViewModelCommand(ShowFavouriteSectionCommand);
            ShowProfileSection = new ViewModelCommand(ShowProfileSectionCommand);

            UserRepository = new UserRepository();
            User = UserRepository.LoadUserData(FilmFlow.Properties.Settings.Default.CurrentUser);

            UserAccessVisibility = User.Admin > 0 ? Visibility.Visible : Visibility.Collapsed;

            settingsView = new SettingsViewModel();
            moderationView = new ModerationViewModel();
            profileView = new ProfileViewModel(UpdateProfileSection);

            ChildContentView = new HomeViewModel(new ViewModelCommand(ShowMovieSection));
        }

        //Views changing
        private void ShowHomeSectionCommand(object obj) => ChildContentView = new HomeViewModel(new ViewModelCommand(ShowMovieSection));
        private void ShowSettingsSectionCommand(object obj) => ChildContentView = settingsView;
        private void ShowAdminSectionCommand(object obj) => ChildContentView = moderationView;
        private void ShowProfileSectionCommand(object obj) => ChildContentView = profileView;
        private void ShowFavouriteSectionCommand(object obj) => ChildContentView = new FavouriteViewModel(User, new ViewModelCommand(ShowMovieSection));
        private void ShowMovieSection(object movieId) => ChildContentView = new MovieViewModel((int)movieId, new ViewModelCommand(ShowHomeSectionCommand), new ViewModelCommand(ShowMovieSection));
        private void UpdateProfileSection() 
        { 
            profileView = new ProfileViewModel(UpdateProfileSection); 
            ShowProfileSectionCommand(null); 
        }
        //
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
