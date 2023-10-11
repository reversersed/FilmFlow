using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilmFlow.MainWindow
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MovieModel> _movies;

        private int _selectedMovie = -1;
        private bool _logoutConfirm {  get; set; }
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public bool LogoutConfirm { get { return _logoutConfirm;  }  set { _logoutConfirm = value; OnPropertyChanged(nameof(LogoutConfirm)); } }
        public ICommand MovieListSelected { get; }
        public ICommand LogoutButton { get; }
        MovieRepository MovieRepository { get; set; }
        UserRepository UserRepository { get; set; }
        public User User {  get; set; }

        public Action showStartWindow { get; set; }
        public MainWindowViewModel()
        {
            Movies = new ObservableCollection<MovieModel>();
            MovieListSelected = new ViewModelCommand(MovieListSelectedCommand);
            LogoutButton = new ViewModelCommand(LogoutButtonCommand);

            MovieRepository = new MovieRepository();
            UserRepository = new UserRepository();
            User = UserRepository.LoadUserData(Thread.CurrentPrincipal.Identity.Name);

            MovieRepository.LoadMovies(Movies);
        }

        private void LogoutButtonCommand(object obj)
        {
            UserRepository.Logout(User);
            showStartWindow?.Invoke();
        }

        private void MovieListSelectedCommand(object obj)
        {
            Debug.WriteLine(Movies[(int)obj].Name);
        }
    }
}
