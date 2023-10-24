using FilmFlow.Models;
using FilmFlow.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    public class HomeViewModel : ViewModelBase
    {
        //Private properties
        private ObservableCollection<MovieModel> _movies;
        private ObservableCollection<MovieModel> _popularMovies;
        private ObservableCollection<GenreModel> _genres;
        private List<int> _filteredGenres = new List<int>();
        private string _movieSearchName;
        private string _genreFilterIcon = "CircleArrowUp";
        private int _selectedMovie = -1;
        private int _popularSelectedMovie = -1;
        private Visibility _filterVisibility = Visibility.Collapsed;
        private Visibility _genreFilterVisibility = Visibility.Visible;
        private GenreModel _popularGenreToday;

        //Public Properties
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public ObservableCollection<MovieModel> PopularMovies { get { return _popularMovies; } set { _popularMovies = value; OnPropertyChanged(nameof(PopularMovies)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public int PopularSelectedMovie { get { return _popularSelectedMovie; } set { _popularSelectedMovie = value; OnPropertyChanged(nameof(PopularSelectedMovie)); } }
        public string MovieSearchName { get { return _movieSearchName; } set { _movieSearchName = value; OnPropertyChanged(nameof(MovieSearchName)); } }
        public string GenreFilterIcon { get { return _genreFilterIcon; } set { _genreFilterIcon = value; OnPropertyChanged(nameof(GenreFilterIcon)); } }
        public Visibility FilterVisibility { get { return _filterVisibility; } set { _filterVisibility = value; OnPropertyChanged(nameof(FilterVisibility)); } }
        public Visibility GenreFilterVisibility { get { return _genreFilterVisibility; } set { _genreFilterVisibility = value; OnPropertyChanged(nameof(GenreFilterVisibility)); } }
        public GenreModel PopularGenreToday { get { return _popularGenreToday; } set { _popularGenreToday = value; OnPropertyChanged(nameof(PopularGenreToday)); } }

        //Models
        MovieRepository MovieRepository { get; set; }

        //Commands
        public ICommand MovieListSelected { get; }
        public ICommand PopularMovieListSelected { get; }
        public ICommand SwitchFilter { get; }
        public ICommand CollapseGenreFilter { get; }
        public ICommand GenreFilterChecked { get; }
        public ICommand GenreFilterUnchecked { get; }
        public ICommand SearchByName { get; }
        public ICommand SearchByFilter { get; }
        public ICommand ClearNameSearch { get; }

        //Methods
        public HomeViewModel()
        {
            MovieListSelected = new ViewModelCommand(MovieListSelectedCommand);
            PopularMovieListSelected = new ViewModelCommand(PopularMovieListSelectedCommand);
            CollapseGenreFilter = new ViewModelCommand(CollapseGenreFilterCommand);
            SwitchFilter = new ViewModelCommand(SwitchFilterCommand);
            GenreFilterChecked = new ViewModelCommand(GenreFilterCheckedCommand);
            GenreFilterUnchecked = new ViewModelCommand(GenreFilterUncheckedCommand);
            SearchByName = new ViewModelCommand(SearchByNameCommand);
            SearchByFilter = new ViewModelCommand(SearchByFilterCommand);
            ClearNameSearch = new ViewModelCommand(ClearNameSearchCommand);

            MovieRepository = new MovieRepository();

            Movies = MovieRepository.LoadMovies();
            Genres = MovieRepository.LoadGenreCollection();
            PopularGenreToday = MovieRepository.GetPopularGenre();
            PopularMovies = MovieRepository.LoadMoviesByGenre(PopularGenreToday);
        }

        private void SearchByFilterCommand(object obj)
        {
            if(!_filteredGenres.Any())
                Movies = MovieRepository.LoadMovies();
            else
                Movies = MovieRepository.LoadFilteredMovies(_filteredGenres);
        }

        private void ClearNameSearchCommand(object obj) { MovieSearchName = string.Empty; FilterMovieSearch(); }
        private void FilterMovieSearch()
        {
            if (MovieSearchName == null || MovieSearchName.Length < 1)
                Movies = MovieRepository.LoadMovies();
            else
                Movies = MovieRepository.LoadFilteredMovies(MovieSearchName);
        }
        private void SearchByNameCommand(object obj) => FilterMovieSearch();

        private void GenreFilterCheckedCommand(object obj) => _filteredGenres.Add((int)obj);
        private void GenreFilterUncheckedCommand(object obj) => _filteredGenres.Remove((int)obj);

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
        private void PopularMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            Debug.WriteLine(PopularMovies[(int)obj].Name);
        }
    }
}
