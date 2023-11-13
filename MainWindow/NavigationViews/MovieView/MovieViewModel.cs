using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    public class MovieViewModel : ViewModelBase
    {
        //Private properties
        private string _genreString { get; set; } = string.Empty;
        private float _ratingValue { get; set; }
        private float CurrentRating { get; set; }
        private string _reviewText { get; set; }
        private ObservableCollection<Review> _reviews { get; set; } = new ObservableCollection<Review>();

        //Public properties
        public string GenreString { get { return _genreString; } set { _genreString = value; OnPropertyChanged(nameof(GenreString)); } }
        public float RatingValue { get { return _ratingValue; } set { _ratingValue = value; OnPropertyChanged(nameof(RatingValue)); } }
        public string ReviewText { get { return _reviewText; } set { _reviewText = value; OnPropertyChanged(nameof(ReviewText)); } }
        public EventHandler<float> MouseRatingMoved { get; set; }
        public EventHandler<bool> RatingChanged { get; set; }
        public ObservableCollection<Review> Reviews { get { return _reviews; } set { _reviews = value; OnPropertyChanged(nameof(Reviews)); } }
            
        //Models
        private IMovieRepository MovieRepository { get; set; }
        private IUserRepository userRepository { get; set; }
        private IReviewRepository ReviewRepository { get; set; }
        public MovieModel Movie { get; set; }
        public User Account;

        //Commands
        public ICommand BackToHome { get; }
        public ICommand ReloadPage { get; }
        public ICommand SendReview { get; }

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
        public MovieViewModel(int movieId, ICommand BackAction, ICommand RestartAction)
        {
            BackToHome = BackAction;
            ReloadPage = RestartAction;
            SendReview = new ViewModelCommand(SendReviewCommand, x => CurrentRating > 0 && ReviewText?.Length > 20);

            MovieRepository = new MovieRepository();
            userRepository = new UserRepository();
            ReviewRepository = new ReviewRepository();

            Account = userRepository.LoadUserData(userRepository.AuthenticateUser());
            Movie = MovieRepository.LoadMovieById(movieId);
            for (int i = 0; i < Movie.Genres.Count; i++)
                GenreString += (i == 0 ? Movie.Genres[i].Name : Movie.Genres[i].Name.ToLower()) + (i != Movie.Genres.Count - 1 ? ", " : string.Empty);
            MouseRatingMoved += RatingMouse;
            RatingChanged += RatingChangedHandler;

            Reviews = ReviewRepository.LoadReviews(movieId);
        }
        private void SendReviewCommand(object var)
        {
            ReviewRepository.AddReview(CurrentRating, ReviewText, Movie.Id, Account.Id);
            ReloadPage?.Execute(Movie.Id);
        }
    }
}
