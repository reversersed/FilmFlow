using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    public class MovieViewModel : ViewModelBase
    {
        //Private properties
        private string _genreString { get; set; } = string.Empty;
        private string? _countryString { get; set; } = null;
        private float _ratingValue { get; set; }
        private float CurrentRating { get; set; }
        private string _reviewText { get; set; }
        private ObservableCollection<Review> _reviews { get; set; } = new ObservableCollection<Review>();
        private int _currentPage { get; set; } = 1;
        private int _totalPages { get; set; }
        private const int _reviewPerPage = 10;
        private Dictionary<ReviewFilter, string> _filters = new Dictionary<ReviewFilter, string>() {
            { ReviewFilter.Default, Application.Current.FindResource("Default") as string },
            { ReviewFilter.RatingDesc, Application.Current.FindResource("RatingDesc") as string },
            { ReviewFilter.RatingAsc, Application.Current.FindResource("RatingAsc") as string },
            { ReviewFilter.DateDesc, Application.Current.FindResource("DateDesc") as string },
            { ReviewFilter.DateAsc, Application.Current.FindResource("DateAsc") as string }
        };
        private ReviewFilter _currentFilter { get; set; } = ReviewFilter.Default;
        private bool _confirmVisible { get; set; } = false;
        private string _confirmText { get; set; } = string.Empty;
        private bool _isBanConfirmationOpened { get; set; } = false;
        private int ConfirmationTag {  get; set; }
        private bool _isSubscribed { get; set; } = false;

        //Public properties
        public string GenreString { get { return _genreString; } set { _genreString = value; OnPropertyChanged(nameof(GenreString)); } }
        public string? CountryString { get { return _countryString; } set { _countryString = value; OnPropertyChanged(nameof(CountryString)); } }
        public float RatingValue { get { return _ratingValue; } set { _ratingValue = value; OnPropertyChanged(nameof(RatingValue)); } }
        public string ReviewText { get { return _reviewText; } set { _reviewText = value; OnPropertyChanged(nameof(ReviewText)); } }
        public ObservableCollection<Review> Reviews { get { return _reviews; } set { _reviews = value; OnPropertyChanged(nameof(Reviews)); } }
        public int TotalPage { get { return _totalPages; } set { _totalPages = value; OnPropertyChanged(nameof(TotalPage)); } }
        public bool IsSubscribed { get { return _isSubscribed; } set { _isSubscribed = value; OnPropertyChanged(nameof(IsSubscribed)); } }
        public int CurrentPage { get { return _currentPage; } 
            set 
            {
                if (value < 1)
                    _currentPage = 1;
                else if (value > TotalPage)
                    _currentPage = TotalPage;
                else
                    _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage)); 
            } 
        }
        public Dictionary<ReviewFilter, string> ReviewFilters { get { return _filters; } }
        public ReviewFilter CurrentFilter {  get { return _currentFilter; } set { _currentFilter = value; OnReviewFilterChanged();  OnPropertyChanged(nameof(CurrentFilter)); } }
        public bool ConfirmationVisibility { get { return _confirmVisible; } set { _confirmVisible = value; OnPropertyChanged(nameof(ConfirmationVisibility)); } }
        public bool IsBanConfirmationOpened { get { return _isBanConfirmationOpened; } set { _isBanConfirmationOpened = value; OnPropertyChanged(nameof(IsBanConfirmationOpened)); } }
        public string ConfirmationText { get { return _confirmText; } set { _confirmText = value; OnPropertyChanged(nameof(ConfirmationText)); } }

        //Models
        private IMovieRepository MovieRepository { get; set; }
        private IUserRepository userRepository { get; set; }
        private IReviewRepository ReviewRepository { get; set; }
        public MovieModel Movie { get; private set; }
        public User Account { get; private set; }

        //Commands
        public ICommand BackToHome { get; }
        public ICommand ReloadPage { get; }
        public ICommand SendReview { get; }
        public ICommand RatingChanged { get; }
        public ICommand MouseRatingMoved { get; }
        public ICommand SetReviewPage { get; }
        public ICommand ClearReviews { get; }
        public ICommand DeleteReview { get; }
        public ICommand BanUser { get; }
        public ICommand FavouriteState { get; }
        private ICommand _confirmCmd { get; set; }
        public ICommand ConfirmationCommand { get { return _confirmCmd; } private set { _confirmCmd = value; OnPropertyChanged(nameof(ConfirmationCommand)); } }

        //Methods
        private void RatingMouse(object arg)
        {
            float rating = (float)arg;
            RatingValue = (float)(rating < 0.5 ? 0.5 : rating < 1.0 ? 1.0 : rating < 1.5 ? 1.5 : rating < 2.0 ? 2.0 : rating < 2.5 ? 2.5 : rating < 3.0 ? 3.0 : rating < 3.5 ? 3.5 : rating < 4.0 ? 4.0 : rating < 4.5 ? 4.5 : 5.0);
        }
        private void RatingChangedHandler(object changed)
        {
            if ((bool)changed)
                CurrentRating = RatingValue;
            else
                RatingValue = CurrentRating;
        }
        public MovieViewModel(int movieId, ICommand BackAction, ICommand RestartAction)
        {

            BackToHome = BackAction;
            ReloadPage = RestartAction;
            SendReview = new ViewModelCommand(SendReviewCommand, x => CurrentRating > 0 && ReviewText?.Length > 20);
            SetReviewPage = new ViewModelCommand(ReviewPage);
            ClearReviews = new ViewModelCommand(ClearReviewsCommand, x=> Reviews.Count > 0);
            DeleteReview = new ViewModelCommand(DeleteReviewCommand);
            BanUser = new ViewModelCommand(BanUserCommand);

            MovieRepository = new MovieRepository();
            userRepository = new UserRepository();
            ReviewRepository = new ReviewRepository();

            Account = userRepository.LoadUserData(FilmFlow.Properties.Settings.Default.CurrentUser);
            Movie = MovieRepository.LoadMovieById(movieId);
            if(Account.Subscription == null || Account.Subscription.EndDate < DateTime.UtcNow.ToUniversalTime())
                IsSubscribed = false;
            else
            {
                foreach(SubscriptionGenre i in Account.Subscription.SubGenre)
                {
                    if(Movie.Genres.Where(x=>x.Id == i.Genre.Id).Any())
                    {
                        IsSubscribed = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < Movie.Genres.Count; i++)
                GenreString += (i == 0 ? Movie.Genres[i].Name : Movie.Genres[i].Name.ToLower()) + (i != Movie.Genres.Count - 1 ? ", " : string.Empty);
            if (Movie.Countries.Count == 0)
                CountryString = null;
            else
            {
                for (int i = 0; i < Movie.Countries.Count; i++)
                    CountryString += Movie.Countries[i].Name + (i != Movie.Countries.Count - 1 ? ", " : string.Empty);
            }
            MouseRatingMoved = new ViewModelCommand(RatingMouse);
            RatingChanged = new ViewModelCommand(RatingChangedHandler);
            FavouriteState = new ViewModelCommand(x => { MovieRepository.ChangeFavouriteState(Movie.Id, Account.Id); Movie.IsInFavourite = MovieRepository.IsInFavourite(Movie.Id)??false; } );

            Reviews = ReviewRepository.LoadReviews(movieId, (CurrentPage-1)*_reviewPerPage, _reviewPerPage);
            int totalReviews = ReviewRepository.CountReviews(movieId);
            TotalPage = totalReviews / _reviewPerPage + (totalReviews % _reviewPerPage > 0 || totalReviews == 0 ? 1 : 0);
        }
        private void BanUserCommand(object obj)
        {
            ConfirmationTag = Int32.Parse(obj.ToString());
            ConfirmationText = Application.Current.FindResource("BanUserConfirmation") as string;
            IsBanConfirmationOpened = true;
            ConfirmationCommand = new ViewModelCommand(BanUserAction);
            ConfirmationVisibility = true;
        }
        private void BanUserAction(object obj)
        {
            if (((string)obj).Equals("yes"))
            {
                userRepository.BanUser(ConfirmationTag);
                ReviewRepository.DeleteUsersReview(ConfirmationTag);
                ReloadPage?.Execute(Movie.Id);
            }
            else if (((string)obj).Equals("no"))
            {
                userRepository.BanUser(ConfirmationTag);
                ReloadPage?.Execute(Movie.Id);
            }
            IsBanConfirmationOpened = false;
            ConfirmationTag = 0;
            ConfirmationVisibility = false;
        }
        private void DeleteReviewCommand(object obj)
        {
            ConfirmationTag = Int32.Parse(obj.ToString());
            ConfirmationText = Application.Current.FindResource("DeleteReviewConfirmation") as string;
            ConfirmationCommand = new ViewModelCommand(DeleteReviewAction);
            ConfirmationVisibility = true;
        }
        private void DeleteReviewAction(object obj)
        {
            if (((string)obj).Equals("yes"))
            {
                ReviewRepository.DeleteReview(ConfirmationTag);
                ReloadPage?.Execute(Movie.Id);
            }
            ConfirmationTag = 0;
            ConfirmationVisibility = false;
        }
        private void ClearReviewsCommand(object obj)
        {
            ConfirmationText = Application.Current.FindResource("ClearReviewsConfirmation") as string;
            ConfirmationCommand = new ViewModelCommand(ClearReviewsAction);
            ConfirmationVisibility = true;
        }
        private void ClearReviewsAction(object obj)
        {
            if (((string)obj).Equals("yes"))
            {
                ReviewRepository.DeleteMovieReviews(Movie.Id);
                ReloadPage?.Execute(Movie.Id);
            }
            ConfirmationVisibility = false;
        }
        private void ReviewPage(object obj)
        {
            if(obj.GetType().Equals(typeof(int)))
                CurrentPage = (int) obj;
            else
                CurrentPage = Int32.Parse((string)obj);

            Reviews = ReviewRepository.LoadReviews(Movie.Id, (CurrentPage - 1) * _reviewPerPage, _reviewPerPage, CurrentFilter);
        }
        private void OnReviewFilterChanged() => Reviews = ReviewRepository.LoadReviews(Movie.Id, (CurrentPage - 1) * _reviewPerPage, _reviewPerPage, CurrentFilter);
        private void SendReviewCommand(object var)
        {
            ReviewRepository.AddReview(CurrentRating, ReviewText, Movie.Id, Account.Id);
            ReloadPage?.Execute(Movie.Id);
        }
    }
}
