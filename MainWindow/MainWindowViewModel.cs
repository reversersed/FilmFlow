using FilmFlow.Models;
using FilmFlow.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FilmFlow.MainWindow.MainWindowModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MovieModel> _movies;
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        MovieRepository MovieRepository { get; set; }
        public MainWindowViewModel()
        {
            Movies = new ObservableCollection<MovieModel>();
            MovieRepository = new MovieRepository();
            MovieRepository.LoadMovies(Movies);
        }
        private void LoadMovies()
        {

        }
    }
}
