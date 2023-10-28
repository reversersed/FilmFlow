using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView
{
    public class ModerationViewModel : ViewModelBase
    {
        //Private properties
        private Visibility _addMoviePanel = Visibility.Collapsed;
        private ObservableCollection<GenreModel> _genres;
        private List<int> _selectedGenres = new List<int>();
        private Movie _addMovieModel = new Movie();
        private string _coverUrl;
        private string _movieUrl = String.Empty;
        private string _error;

        //Public properties
        public Visibility AddMoviePanel { get { return _addMoviePanel; } set {  _addMoviePanel = value; OnPropertyChanged(nameof(AddMoviePanel)); } }
        public Movie AddMovieModel { get { return _addMovieModel; } set { _addMovieModel = value; OnPropertyChanged(nameof(AddMovieModel)); } }
        public string MovieYear { get { return _addMovieModel.Year.ToString(); } 
            set {
                if(value == null || value.Length < 1)
                    return;
                int year = 0;
                if(Int32.TryParse(value, out year))
                {
                    _addMovieModel.Year = year;
                    OnPropertyChanged(nameof(MovieYear));
                }
            } }
        public string CoverUrl { get { return _coverUrl; } set { _coverUrl = value; OnPropertyChanged(nameof(CoverUrl)); } }
        public string MovieUrl { get { return _movieUrl; } set
            {
                _movieUrl = value;
                Uri url;
                if(Uri.TryCreate(value, UriKind.Absolute, out url))
                    OnPropertyChanged(nameof(MovieUrl));
            } 
        }
        public string Error { get { return _error; } set { _error = value; OnPropertyChanged(nameof(Error)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }

        //Models
        private MovieRepository movieRepository;

        //Commands
        public ICommand InsertMovie { get; }
        public ICommand GenreChecked { get; }
        public ICommand GenreUnchecked { get; }

        //Methods
        public ModerationViewModel()
        {
            movieRepository = new MovieRepository();

            InsertMovie = new ViewModelCommand(InsertMovieCommand);
            GenreChecked = new ViewModelCommand(GenreCheckedCommand);
            GenreUnchecked = new ViewModelCommand(GenreUncheckedCommand);

            Genres = movieRepository.LoadGenreCollection();
        }
        private void GenreCheckedCommand(object obj) => _selectedGenres.Add((int)obj);
        private void GenreUncheckedCommand(object obj) => _selectedGenres.Remove((int)obj);
        private void InsertMovieCommand(object obj)
        {
            if(AddMovieModel.NameEn == null ||
                AddMovieModel.NameEn.Length < 1 ||
                AddMovieModel.NameRu == null ||
                AddMovieModel.NameRu.Length < 1)
            {
                Error = Application.Current.FindResource("SpecifyMovieName") as string;
                return;
            }
            if (AddMovieModel.DescriptionEn == null ||
                AddMovieModel.DescriptionEn.Length < 1 ||
                AddMovieModel.DescriptionRu == null ||
                AddMovieModel.DescriptionRu.Length < 1)
            {
                Error = Application.Current.FindResource("SpecifyMovieDesc") as string;
                return;
            }
            if(AddMovieModel.Year < 1800 || AddMovieModel.Year > 3000)
            {
                Error = Application.Current.FindResource("SpecifyMovieYear") as string;
                return;
            }
            if(CoverUrl == null || CoverUrl.Length < 1 ||
                (!CoverUrl.Contains("http") &&
                !CoverUrl.Contains("www") &&
                !CoverUrl.Contains(".com") &&
                !CoverUrl.Contains(".ru") &&
                !CoverUrl.Contains(".net")))
            {
                Error = Application.Current.FindResource("SpecifyMovieCover") as string;
                return;
            }
            if(MovieUrl == null || MovieUrl.Length < 1)
            {
                Error = Application.Current.FindResource("SpecifyMovieUrl") as string;
                return;
            }
            if(!_selectedGenres.Any())
            {
                Error = Application.Current.FindResource("SelectGenre") as string;
                return;
            }
            foreach(int i in _selectedGenres)
            {
                AddMovieModel.Genre.Add(new MovieGenre() { genreid = i });
            }
            movieRepository.AddMovie(AddMovieModel, CoverUrl, MovieUrl);
            AddMovieModel = new Movie();
            CoverUrl = String.Empty;
            MovieUrl = String.Empty;
            Error = Application.Current.FindResource("MovieAdded") as string;
        }
    }
}
