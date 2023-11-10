using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    public class MovieViewModel : ViewModelBase
    {
        //Private properties
        private string _genreString { get; set; } = string.Empty;
        private float _ratingValue { get; set; }
        private float CurrentRating { get; set; }

        //Public properties
        public string GenreString { get { return _genreString; } set { _genreString = value; OnPropertyChanged(nameof(GenreString)); } }
        public float RatingValue { get { return _ratingValue; } set { _ratingValue = value; OnPropertyChanged(nameof(RatingValue)); } }
        public EventHandler<float> MouseRatingMoved { get; set; }
        public EventHandler<bool> RatingChanged { get; set; }
            
        //Models
        private MovieRepository MovieRepository { get; set; }
        public MovieModel Movie { get; set; }

        //Commands
        public ICommand BackToHome { get; }

        //Methods
        private void RatingMouse(object? sender, float rating)
        {
            RatingValue = (float)(rating < 0.5 ? 0.5 : rating < 1.0 ? 1.0 : rating < 1.5 ? 1.5 : rating < 2.0 ? 2.0 : rating < 2.5 ? 2.5 : rating < 3.0 ? 3.0 : rating < 3.5 ? 3.5 : rating < 4.0 ? 4.0 : rating < 4.5 ? 4.5 : 5.0);
        }
        private void RatingChangedHandler(object? sender, bool chaned)
        {
            if (chaned)
                CurrentRating = RatingValue;
            else
                RatingValue = CurrentRating;
        }
        public MovieViewModel(int movieId, ICommand BackAction)
        {
            BackToHome = BackAction;

            MovieRepository = new MovieRepository();

            Movie = MovieRepository.LoadMovieById(movieId);
            for (int i = 0; i < Movie.Genres.Count; i++)
                GenreString += (i == 0 ? Movie.Genres[i].Name : Movie.Genres[i].Name.ToLower()) + (i != Movie.Genres.Count - 1 ? ", " : string.Empty);
            MouseRatingMoved += RatingMouse;
            RatingChanged += RatingChangedHandler;
        }
    }
}
