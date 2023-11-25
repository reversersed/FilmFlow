﻿using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
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
        private ObservableCollection<MovieModel> _popularMoviesYear;
        private ObservableCollection<MovieModel> _popularMoviesMonth;
        private ObservableCollection<MovieModel> _popularMoviesDay;
        private ObservableCollection<MovieModel> _mostRatedMovies;
        private ObservableCollection<MovieModel> _newMovies;
        private ObservableCollection<GenreModel> _genres;
        private List<int> _filteredGenres = new List<int>();
        private string _movieSearchName;
        private string _genreFilterIcon = "CircleArrowUp";
        private int _selectedMovie = -1;
        private int _popularYearSelectedMovie = -1;
        private int _popularMonthSelectedMovie = -1;
        private int _popularDaySelectedMovie = -1;
        private int _mostRatedSelectedMovie = -1;
        private int _newSelectedMovie = -1;
        private Visibility _filterVisibility = Visibility.Collapsed;
        private Visibility _genreFilterVisibility = Visibility.Visible;

        //Public Properties
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public ObservableCollection<MovieModel> PopularMoviesYear { get { return _popularMoviesYear; } set { _popularMoviesYear = value; OnPropertyChanged(nameof(PopularMoviesYear)); } }
        public ObservableCollection<MovieModel> PopularMoviesMonth { get { return _popularMoviesMonth; } set { _popularMoviesMonth = value; OnPropertyChanged(nameof(PopularMoviesMonth)); } }
        public ObservableCollection<MovieModel> PopularMoviesDay { get { return _popularMoviesDay; } set { _popularMoviesDay = value; OnPropertyChanged(nameof(PopularMoviesDay)); } }
        public ObservableCollection<MovieModel> MostRatedMovies { get { return _mostRatedMovies; } set { _mostRatedMovies = value; OnPropertyChanged(nameof(MostRatedMovies)); } }
        public ObservableCollection<MovieModel> NewMovies { get { return _newMovies; } set { _newMovies = value; OnPropertyChanged(nameof(NewMovies)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public int PopularYearSelectedMovie { get { return _popularYearSelectedMovie; } set { _popularYearSelectedMovie = value; OnPropertyChanged(nameof(PopularYearSelectedMovie)); } }
        public int PopularMonthSelectedMovie { get { return _popularMonthSelectedMovie; } set { _popularMonthSelectedMovie = value; OnPropertyChanged(nameof(PopularMonthSelectedMovie)); } }
        public int PopularDaySelectedMovie { get { return _popularDaySelectedMovie; } set { _popularDaySelectedMovie = value; OnPropertyChanged(nameof(PopularDaySelectedMovie)); } }
        public int MostRatedSelectedMovie { get { return _mostRatedSelectedMovie; } set { _mostRatedSelectedMovie = value; OnPropertyChanged(nameof(MostRatedSelectedMovie)); } }
        public int NewSelectedMovie { get { return _newSelectedMovie; } set { _newSelectedMovie = value; OnPropertyChanged(nameof(NewSelectedMovie)); } }
        public string MovieSearchName { get { return _movieSearchName; } set { _movieSearchName = value; OnPropertyChanged(nameof(MovieSearchName)); } }
        public string GenreFilterIcon { get { return _genreFilterIcon; } set { _genreFilterIcon = value; OnPropertyChanged(nameof(GenreFilterIcon)); } }
        public Visibility FilterVisibility { get { return _filterVisibility; } set { _filterVisibility = value; OnPropertyChanged(nameof(FilterVisibility)); } }
        public Visibility GenreFilterVisibility { get { return _genreFilterVisibility; } set { _genreFilterVisibility = value; OnPropertyChanged(nameof(GenreFilterVisibility)); } }

        //Models
        MovieRepository MovieRepository { get; set; }

        //Commands
        public ICommand MovieListSelected { get; }
        public ICommand PopularYearMovieListSelected { get; }
        public ICommand PopularMonthMovieListSelected { get; }
        public ICommand PopularDayMovieListSelected { get; }
        public ICommand MostRatedMovieListSelected { get; }
        public ICommand NewMovieListSelected { get; }
        public ICommand SwitchFilter { get; }
        public ICommand CollapseGenreFilter { get; }
        public ICommand GenreFilterChecked { get; }
        public ICommand GenreFilterUnchecked { get; }
        public ICommand SearchByName { get; }
        public ICommand SearchByFilter { get; }
        public ICommand ClearNameSearch { get; }

        //Action
        public Action<int> openMovieCommand;

        //Methods
        public HomeViewModel(Action<int> openMovieCommand)
        {
            MovieListSelected = new ViewModelCommand(MovieListSelectedCommand);
            PopularYearMovieListSelected = new ViewModelCommand(PopularYearMovieListSelectedCommand);
            PopularMonthMovieListSelected = new ViewModelCommand(PopularMonthMovieListSelectedCommand);
            PopularDayMovieListSelected = new ViewModelCommand(PopularDayMovieListSelectedCommand);
            MostRatedMovieListSelected = new ViewModelCommand(MostRatedMovieListSelectedCommand);
            NewMovieListSelected = new ViewModelCommand(NewMovieListSelectedCommand);
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
            PopularMoviesYear = MovieRepository.GetPopularMovies(365);
            PopularMoviesMonth = MovieRepository.GetPopularMovies(31);
            PopularMoviesDay = MovieRepository.GetPopularMovies(1);
            MostRatedMovies = MovieRepository.GetMostRated();
            NewMovies = MovieRepository.LoadNewMovies();

            this.openMovieCommand = openMovieCommand;
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
            openMovieCommand?.Invoke(Movies[(int)obj].Id);
        }
        private void PopularYearMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            openMovieCommand?.Invoke(PopularMoviesYear[(int)obj].Id);
        }
        private void PopularMonthMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            openMovieCommand?.Invoke(PopularMoviesMonth[(int)obj].Id);
        }
        private void PopularDayMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            openMovieCommand?.Invoke(PopularMoviesDay[(int)obj].Id);
        }
        private void MostRatedMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            openMovieCommand?.Invoke(MostRatedMovies[(int)obj].Id);
        }
        private void NewMovieListSelectedCommand(object obj)
        {
            if ((int)obj < 0)
                return;
            openMovieCommand?.Invoke(NewMovies[(int)obj].Id);
        }
    }
}
