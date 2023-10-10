using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilmFlow.MainWindow
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MovieModel> _movies;

        private int _selectedMovie = -1;
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public ICommand MovieListSelected { get; }
        MovieRepository MovieRepository { get; set; }

        public MainWindowViewModel()
        {
            Movies = new ObservableCollection<MovieModel>();
            MovieListSelected = new ViewModelCommand(MovieListSelectedCommand);

            MovieRepository = new MovieRepository();

            MovieRepository.LoadMovies(Movies);
        }

        private void MovieListSelectedCommand(object obj)
        {
            Debug.WriteLine(Movies[(int)obj].Name);
        }
    }
}
