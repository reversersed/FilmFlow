using FilmFlow.Models;
using FilmFlow.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    public class HomeViewModel : ViewModelBase
    {
        //Private properties
        private ObservableCollection<MovieModel> _movies;
        private int _selectedMovie = -1;

        //Public Properties
        public ObservableCollection<MovieModel> Movies { get { return _movies; } set { _movies = value; OnPropertyChanged(nameof(Movies)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }

        //Repositories
        MovieRepository MovieRepository { get; set; }

        //Commands
        public ICommand MovieListSelected { get; }

        //Methods
        public HomeViewModel()
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
