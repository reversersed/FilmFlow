using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilmFlow.MainWindow.MainWindowViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MovieModel> _movies;
        private ObservableCollection<string> _genres;
        private string _currentLanguage {  get; set; }
        private string _currentGenre {  get; set; }
        private int _selectedMovie = -1;
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public ObservableCollection<string> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public string CurrentGenre { get { return _currentGenre; } set { _currentGenre = value; GenreChanged(); OnPropertyChanged(nameof(CurrentGenre)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }
        public string CurrentLanguage { get { return _currentLanguage; } set { _currentLanguage = value; OnPropertyChanged(nameof(CurrentLanguage)); } }
        MovieRepository MovieRepository { get; set; }

        public ICommand languageChanged { get; }
        public MainWindowViewModel()
        {
            Movies = new ObservableCollection<MovieModel>();
            Genres = new ObservableCollection<string>();
            Genres.Add(" ");
            MovieRepository = new MovieRepository();

            MovieRepository.LoadMovies(Movies);
            MovieRepository.LoadGenres(Genres);

            CurrentLanguage = FilmFlow.Properties.Settings.Default.Language;
            languageChanged = new ViewModelCommand(LanguageChangedCommand);
        }

        private void LanguageChangedCommand(object obj)
        {
            FilmFlow.Properties.Settings.Default.Language = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? "en-US" : "ru-RU";
            CurrentLanguage = FilmFlow.Properties.Settings.Default.Language;
            FilmFlow.Properties.Settings.Default.Save();
        }
        private void GenreChanged()
        {
            Movies.Clear();
            if (CurrentGenre.Length > 2)
                MovieRepository.LoadMoviesByGenre(CurrentGenre, Movies);
            else
                MovieRepository.LoadMovies(Movies);
        }
    }
}
