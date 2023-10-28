using FilmFlow.Models;
using FilmFlow.ViewModels;
using System.Diagnostics;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    public class MovieViewModel : ViewModelBase
    {
        //Private properties
        private string _genreString { get; set; } = string.Empty;

        //Public properties
        public string GenreString { get { return _genreString; } set { _genreString = value; OnPropertyChanged(nameof(GenreString)); } }

        //Models
        private MovieRepository MovieRepository { get; set; }
        public MovieModel Movie { get; set; }

        //Commands
        public ICommand BackToHome { get; }

        //Methods
        public MovieViewModel(int movieId, ICommand BackAction)
        {
            BackToHome = BackAction;

            MovieRepository = new MovieRepository();

            Movie = MovieRepository.LoadMovieById(movieId);
            for (int i = 0; i < Movie.Genres.Count; i++)
                GenreString += (i == 0 ? Movie.Genres[i].Name : Movie.Genres[i].Name.ToLower()) + (i != Movie.Genres.Count - 1 ? ", " : string.Empty);
        }
    }
}
