using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    public class HomeViewModel : ViewModelBase
    {
        //Private properties
        private ObservableCollection<MovieModel> _movies;
        private ObservableCollection<GenreModel> _genres;
        private string _movieSearchName;
        private string _genreFilterIcon = "CircleArrowDown";
        private int _selectedMovie = -1;
        private Visibility _filterVisibility = Visibility.Collapsed;
        private Visibility _genreFilterVisibility = Visibility.Collapsed;

        //Public Properties
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public string MovieSearchName { get { return _movieSearchName; } set { _movieSearchName = value; OnPropertyChanged(nameof(MovieSearchName)); } }
        public string GenreFilterIcon { get { return _genreFilterIcon; } set { _genreFilterIcon = value; OnPropertyChanged(nameof(GenreFilterIcon)); } }
        public Visibility FilterVisibility { get { return _filterVisibility; } set { _filterVisibility = value; OnPropertyChanged(nameof(FilterVisibility)); } }
        public Visibility GenreFilterVisibility { get { return _genreFilterVisibility; } set { _genreFilterVisibility = value; OnPropertyChanged(nameof(GenreFilterVisibility)); } }

        //Models
        MovieRepository MovieRepository { get; set; }

        //Commands
        public ICommand MovieListSelected { get; }
        public ICommand SwitchFilter { get; }
        public ICommand CollapseGenreFilter { get; }
        public ICommand GenreFilterChanged { get; }
        public ICommand SearchByName { get; }
        public ICommand SearchByFilter { get; }
        public ICommand ClearNameSearch { get; }

        //Methods
        public HomeViewModel()
        {
            MovieListSelected = new ViewModelCommand(MovieListSelectedCommand);
            CollapseGenreFilter = new ViewModelCommand(CollapseGenreFilterCommand);
            SwitchFilter = new ViewModelCommand(SwitchFilterCommand);
            GenreFilterChanged = new ViewModelCommand(GenreFilterChangedCommand);
            SearchByName = new ViewModelCommand(SearchByNameCommand);
            SearchByFilter = new ViewModelCommand(SearchByFilterCommand);
            ClearNameSearch = new ViewModelCommand(ClearNameSearchCommand);

            MovieRepository = new MovieRepository();

            Movies = MovieRepository.LoadMovies();
            Genres = MovieRepository.LoadGenreCollection();
        }

        private void SearchByFilterCommand(object obj)
        {
            
        }

        private void ClearNameSearchCommand(object obj) { MovieSearchName = string.Empty; FilterMovieSearch(); }
        private void FilterMovieSearch()
        {
            if (MovieSearchName == null || MovieSearchName.Length < 1)
                Movies = MovieRepository.LoadMovies();
            else
            {
                Movies = MovieRepository.LoadFilteredMovies(MovieSearchName);
            }
        }
        private void SearchByNameCommand(object obj) => FilterMovieSearch();

        private void GenreFilterChangedCommand(object obj)
        {
            
        }

        private void CollapseGenreFilterCommand(object obj)
        {
            GenreFilterVisibility = GenreFilterVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            GenreFilterIcon = GenreFilterVisibility == Visibility.Visible ? "CircleArrowUp" : "CircleArrowDown";
        }
        private void SwitchFilterCommand(object obj) => FilterVisibility = FilterVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        private void MovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            Debug.WriteLine(Movies[(int)obj].Name);
        }
    }
}
