using FilmFlow.Models;
using FilmFlow.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace FilmFlow.MainWindow.MainWindowModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MovieModel> _movies;
        private int _selectedMovie = -1;
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; SelectedMovieChanged(); OnPropertyChanged(nameof(SelectedMovie)); } }
        MovieRepository MovieRepository { get; set; }
        public MainWindowViewModel()
        {
            Movies = new ObservableCollection<MovieModel>();
            MovieRepository = new MovieRepository();

            MovieRepository.LoadMovies(Movies);
        }
        private void SelectedMovieChanged()
        {
            
        }
    }
}
