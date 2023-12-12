using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.ProfileView
{
    public class ProfileViewModel : ViewModelBase
    {
        //Private properties
        private User _user;
        private bool _replenishModalVisibility = false;
        private bool _confirmationModalVisibility = false;
        private Visibility _replenishmentSumVisibility = Visibility.Visible;
        private Visibility _replenishmentPaymentVisibility = Visibility.Collapsed;
        private bool _paymentSucceed = false;
        private int? _paymentValue = null;
        private ObservableCollection<Subscription> _subscriptions;
        private ObservableCollection<GenreModel> _genres;
        private bool _isSubscriptionActive = false;
        private ObservableCollection<int> _selectedGenres = new ObservableCollection<int>();
        private int[] _prices = new int[3];
        private int[] _oldPrices = new int[3];
        private int PeriodType = 0;
        private string _confirmText {  get; set; }

        //Public properties
        public User User { get { return _user; } set { _user = value; OnPropertyChanged(nameof(User)); } }
        public bool ReplenishModalVisibility { get { return _replenishModalVisibility; } set { _replenishModalVisibility = value; OnPropertyChanged(nameof(ReplenishModalVisibility)); } }
        public bool ConfirmationModalVisibility { get { return _confirmationModalVisibility; } set { _confirmationModalVisibility = value; OnPropertyChanged(nameof(ConfirmationModalVisibility)); } }
        public Visibility ReplenishmentSumVisibility { get { return _replenishmentSumVisibility; } set { _replenishmentSumVisibility = value; OnPropertyChanged(nameof(ReplenishmentSumVisibility)); } }
        public Visibility ReplenishmentPaymentVisibility { get { return _replenishmentPaymentVisibility; } set { _replenishmentPaymentVisibility = value; OnPropertyChanged(nameof(ReplenishmentPaymentVisibility)); } }
        public string PaymentValue { get { return _paymentValue == null ? null : _paymentValue.ToString() + " ₽"; } set
            {
                string trimmedValue = value?.Replace(" ₽", "");
                if (trimmedValue == null || trimmedValue.Length == 0)
                    _paymentValue = null;
                else
                {
                    int i;
                    if (Int32.TryParse(trimmedValue, out i) && i > 0)
                        _paymentValue = i;
                }
                OnPropertyChanged(nameof(PaymentValue));
            }
        }
        public bool PaymentSucceed { get { return _paymentSucceed; } set { _paymentSucceed = value; OnPropertyChanged(nameof(PaymentSucceed)); } }
        public ObservableCollection<Subscription> Subscriptions { get { return _subscriptions; } set { _subscriptions = value; OnPropertyChanged(nameof(Subscriptions)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public ObservableCollection<int> SelectedGenres { get { return _selectedGenres; } set { _selectedGenres = value; OnPropertyChanged(nameof(SelectedGenres)); } }
        public bool IsSubscriptionActive { get { return _isSubscriptionActive; } set { _isSubscriptionActive = value; OnPropertyChanged(nameof(IsSubscriptionActive)); } }
        public string CurrentLanguage { get { return FilmFlow.Properties.Settings.Default.Language; } }
        public int[] Prices { get { return _prices; } set { _prices = value; OnPropertyChanged(nameof(Prices)); } }
        public int[] OldPrices { get { return _oldPrices; } set { _oldPrices = value; OnPropertyChanged(nameof(OldPrices)); } }
        public string SubConfirmationText { get { return _confirmText; } set { _confirmText = value; OnPropertyChanged(nameof(SubConfirmationText)); } }

        //Repositories
        IUserRepository userRepository;
        IMovieRepository movieRepository;

        //Commands
        public ICommand ReplenishModalMode { get; }
        public ICommand ConfirmationModalMode { get; }
        public ICommand ProceedPayment { get; }
        public ICommand AbordPayment { get; }
        public ICommand GenreChecked { get; }
        public ICommand GenreUnchecked { get; }
        public ICommand PeriodChecked { get; }
        public ICommand SubscribeCommand { get; }
        public ICommand CreateSubscription { get; }

        //Threads
        Thread? WaitingForResponse;

        //Actions
        Action ResponseFeedback;
        Action<object> UpdateView;

        //Methods
        public ProfileViewModel(Action<object> UpdateView)
        {
            userRepository = new UserRepository();
            movieRepository = new MovieRepository();
            User = userRepository.LoadUserData(FilmFlow.Properties.Settings.Default.CurrentUser);

            ReplenishModalMode = new ViewModelCommand((x) => {
                ReplenishModalVisibility = Convert.ToBoolean((string)x);
                WaitingForResponse?.Interrupt();
                if (ReplenishModalVisibility == true)
                    PaymentValue = null;
                PaymentSucceed = false;
                ReplenishmentSumVisibility = Visibility.Visible;
                ReplenishmentPaymentVisibility = Visibility.Collapsed;
            });
            ConfirmationModalMode = new ViewModelCommand((x) => { ConfirmationModalVisibility = Convert.ToBoolean((string)x); });
            ProceedPayment = new ViewModelCommand(ProceedPaymentCommand, (x) => _paymentValue > 0);
            AbordPayment = new ViewModelCommand(AbordPaymentCommand);
            CreateSubscription = new ViewModelCommand(CreateSubscriptionCommand);
            GenreChecked = new ViewModelCommand(x => { if (!SelectedGenres.Contains((int)x)) SelectedGenres.Add((int)x); CalculatePrice(); });
            GenreUnchecked = new ViewModelCommand(x => { if (SelectedGenres.Contains((int)x)) SelectedGenres.Remove((int)x); CalculatePrice(); });
            PeriodChecked = new ViewModelCommand(x => { PeriodType = Int32.Parse((string)x); });
            SubscribeCommand = new ViewModelCommand(x =>
            {
                if (FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU"))
                    SubConfirmationText = String.Format("Вы уверены, что хотите оформить подписку на {0} стоимостью {1}₽?\nВы сможете просматривать фильмы следуюших жанров: ", PeriodType == 0 ? "1 месяц" : PeriodType == 1 ? "6 месяцев" : "1 год", Prices[PeriodType]);
                else
                    SubConfirmationText = String.Format("Are you sure you want to subscribe for {0} at a cost of {1}₽?\r\nYou will be able to watch films of the following genres: ", PeriodType == 0 ? "1 month" : PeriodType == 1 ? "6 months" : "1 year", Prices[PeriodType]);
                SelectedGenres.ToList().ForEach(x => SubConfirmationText += Genres.First(i => i.Id == x).Name.ToLower() + ", ");
                SubConfirmationText = SubConfirmationText.Remove(SubConfirmationText.LastIndexOf(", "), 2);
                ConfirmationModalVisibility = true;
            }, x => Prices[PeriodType] <= User.Balance);

            ResponseFeedback = new Action(PaymentCompleted);
            Subscriptions = userRepository.GetSubscriptions(User.Id);
            IsSubscriptionActive = User.Subscription == null ? false : User.Subscription.EndDate > DateTime.UtcNow.ToUniversalTime();

            Genres = movieRepository.LoadGenreCollection();
            this.UpdateView = UpdateView;
        }

        private void CreateSubscriptionCommand(object obj)
        {
            Subscription subscription = new Subscription()
            {
                StartDate = DateTime.UtcNow.ToUniversalTime(),
                EndDate = DateTime.UtcNow.AddMonths(1 + PeriodType * 6).ToUniversalTime(),
                Price = Prices[PeriodType],
                user = User,
                UserId = User.Id
            };
            ObservableCollection<SubscriptionGenre> subscriptionGenres = new ObservableCollection<SubscriptionGenre>();
            SelectedGenres.ToList().ForEach(x => subscriptionGenres.Add(new SubscriptionGenre() { GenreId = x, Subscription = subscription }));
            subscription.SubGenre = subscriptionGenres;

            userRepository.CreateSubscription(subscription, User);
            UpdateView?.Invoke(null);
        }

        private void AbordPaymentCommand(object obj)
        {
            WaitingForResponse?.Interrupt();
            ReplenishModalVisibility = false;
            ReplenishmentSumVisibility = Visibility.Visible;
            ReplenishmentPaymentVisibility = Visibility.Collapsed;
        }

        private void ProceedPaymentCommand(object obj)
        {
            ReplenishmentSumVisibility = Visibility.Collapsed;
            ReplenishmentPaymentVisibility = Visibility.Visible;

            WaitingForResponse = new Thread(PaymentResponse);
            WaitingForResponse.Start();
        }
        private void PaymentResponse(object obj)
        {
            try
            {
                Thread.Sleep(Random.Shared.Next(4, 10) * 1000);
                this.ResponseFeedback.Invoke();
            }
            catch (ThreadInterruptedException)
            {

            }
        }
        private void PaymentCompleted()
        {
            PaymentSucceed = true;
            Thread.Sleep(5000);
            ReplenishModalVisibility = false;

            userRepository.CreatePayment(User.Id, _paymentValue??0);
            User = userRepository.LoadUserData(FilmFlow.Properties.Settings.Default.CurrentUser);
        }
        private void CalculatePrice()
        {
            int basePrice = (int)((100*(1-Math.Pow(0.95,SelectedGenres.Count))/(1-0.95)));
            Prices = new int[3] { basePrice, 6*(int)((basePrice) * 0.9), 12*(int)(basePrice * 0.7) };
            OldPrices = new int[3] { basePrice, 6*(int)((basePrice)), 12*(int)(basePrice) };
        }
    }
}
