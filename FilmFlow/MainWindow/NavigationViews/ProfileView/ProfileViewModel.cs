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
        private bool _disableSubscription = false;
        private bool _renewSubscription = false;
        private bool _editSubscription = false;
        private string _disableSubText = string.Empty;
        private bool _paymentSucceed = false;
        private int? _paymentValue = null;
        private ObservableCollection<GenreModel> _genres;
        private ObservableCollection<GenreModel> _addGenres;
        private bool _isSubscriptionActive = false;
        private ObservableCollection<int> _selectedGenres = new ObservableCollection<int>();
        private int[] _prices = new int[3];
        private int[] _oldPrices = new int[3];
        private int[] _renewPrices = new int[3];
        private int[] _renewOldPrices = new int[3];
        private int PeriodType = 0;
        private int RenewType = 0;
        private string _confirmText {  get; set; }
        private int _editSubPrice { get; set; } = 0;

        //Public properties
        public User User { get { return _user; } set { _user = value; OnPropertyChanged(nameof(User)); } }
        public bool ReplenishModalVisibility { get { return _replenishModalVisibility; } set { _replenishModalVisibility = value; OnPropertyChanged(nameof(ReplenishModalVisibility)); } }
        public bool ConfirmationModalVisibility { get { return _confirmationModalVisibility; } set { _confirmationModalVisibility = value; OnPropertyChanged(nameof(ConfirmationModalVisibility)); } }
        public Visibility ReplenishmentSumVisibility { get { return _replenishmentSumVisibility; } set { _replenishmentSumVisibility = value; OnPropertyChanged(nameof(ReplenishmentSumVisibility)); } }
        public Visibility ReplenishmentPaymentVisibility { get { return _replenishmentPaymentVisibility; } set { _replenishmentPaymentVisibility = value; OnPropertyChanged(nameof(ReplenishmentPaymentVisibility)); } }
        public bool DisableSubscription { get { return _disableSubscription; } set { _disableSubscription = value; OnPropertyChanged(nameof(DisableSubscription)); } }
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
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public ObservableCollection<GenreModel> AddGenres { get { return _addGenres; } set { _addGenres = value; OnPropertyChanged(nameof(AddGenres)); } }
        public ObservableCollection<int> SelectedGenres { get { return _selectedGenres; } set { _selectedGenres = value; OnPropertyChanged(nameof(SelectedGenres)); } }
        public bool IsSubscriptionActive { get { return _isSubscriptionActive; } set { _isSubscriptionActive = value; OnPropertyChanged(nameof(IsSubscriptionActive)); } }
        public bool ModalRenewSubscription { get { return _renewSubscription; } set { _renewSubscription = value; OnPropertyChanged(nameof(ModalRenewSubscription)); } }
        public bool EditSubscription { get { return _editSubscription; } set { _editSubscription = value; OnPropertyChanged(nameof(EditSubscription)); } }
        public string CurrentLanguage { get { return FilmFlow.Properties.Settings.Default.Language; } }
        public int[] Prices { get { return _prices; } set { _prices = value; OnPropertyChanged(nameof(Prices)); } }
        public int[] OldPrices { get { return _oldPrices; } set { _oldPrices = value; OnPropertyChanged(nameof(OldPrices)); } }
        public string SubConfirmationText { get { return _confirmText; } set { _confirmText = value; OnPropertyChanged(nameof(SubConfirmationText)); } }
        public string DisableSubText { get { return _disableSubText; } set { _disableSubText = value; OnPropertyChanged(nameof(DisableSubText)); } }
        public int[] RenewPrices { get { return _renewPrices; } set { _renewPrices = value; OnPropertyChanged(nameof(RenewPrices)); } }
        public int[] RenewOldPrices { get { return _renewOldPrices; } set { _renewOldPrices = value; OnPropertyChanged(nameof(RenewOldPrices)); } }
        public int EditSubscriptionPrice { get { return _editSubPrice; }set { _editSubPrice = value; OnPropertyChanged(nameof(EditSubscriptionPrice)); } }

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
        public ICommand RenewPeriodChecked { get; }
        public ICommand SubscribeCommand { get; }
        public ICommand CreateSubscription { get; }
        public ICommand RenewSubscription { get; }
        public ICommand ApplyNewSubscription { get; }
        public ICommand CancelSubscription { get; }
        public ICommand DismissSubscription { get; }
        public ICommand ChangeSubscription { get; }
        public ICommand UpdateSubscription { get; }
        public ICommand RecalculateEditPrice { get; }

        //Threads
        Thread? WaitingForResponse;

        //Actions
        Action ResponseFeedback;
        Action UpdateView;

        //Methods
        public ProfileViewModel(Action UpdateView)
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
            RecalculateEditPrice = new ViewModelCommand(x => EditSubscriptionPrice = AddGenres.Where(x => x.IsChecked).Count() * 100);
            PeriodChecked = new ViewModelCommand(x => { PeriodType = Int32.Parse((string)x); });
            RenewPeriodChecked = new ViewModelCommand(x => { RenewType = Int32.Parse((string)x); });
            SubscribeCommand = new ViewModelCommand(x =>
            {
                string[] periods = {
                    FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? "1 месяц" : "1 month",
                    FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? "6 месяцев" : "6 months",
                    FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? "1 год" : "1 year"
                };
                SubConfirmationText = String.Format((Application.Current.FindResource("CreateSubConfirmation") as string).Replace("$n$", "\n")+" ", periods[PeriodType], Prices[PeriodType]);
                
                SelectedGenres.ToList().ForEach(x => SubConfirmationText += Genres.First(i => i.Id == x).Name.ToLower() + ", ");
                SubConfirmationText = SubConfirmationText.Remove(SubConfirmationText.LastIndexOf(", "), 2);
                ConfirmationModalVisibility = true;
            }, x => Prices[PeriodType] <= User.Balance);
            DismissSubscription = new ViewModelCommand(RemoveSubscription);
            CancelSubscription = new ViewModelCommand(x => {
                if (IsSubscriptionActive)
                {
                    DisableSubText = string.Format((Application.Current.FindResource("DisableSubConfirmation") as string).Replace("$n$","\n"),User.Subscription.Price, User.Subscription.EndDate.ToString("dd.MM.yyyy"));
                    DisableSubscription = true;
                }
            });
            RenewSubscription = new ViewModelCommand(x =>
            {
                int basePrice = (int)((80 * (1 - Math.Pow(0.95, User.Subscription.SubGenre.Count)) / (1 - 0.95)));
                RenewPrices = new int[3] { basePrice, 6 * (int)((basePrice) * 0.9), 12 * (int)(basePrice * 0.7) };
                RenewOldPrices = new int[3] { basePrice, 6 * (int)((basePrice)), 12 * (int)(basePrice) };

                ModalRenewSubscription = !ModalRenewSubscription;
            });
            ApplyNewSubscription = new ViewModelCommand(RenewSubscriptionAction, x => RenewPrices[RenewType] <= User.Balance);
            ChangeSubscription = new ViewModelCommand(x => {
                RecalculateEditPrice.Execute(x);
                EditSubscription = !EditSubscription;
            });
            UpdateSubscription = new ViewModelCommand(UpdateSubscriptionCommand, x => EditSubscriptionPrice > 0 && User.Balance >= EditSubscriptionPrice);
            ResponseFeedback = new Action(PaymentCompleted);
            if(User.Subscription != null)
            {
                IsSubscriptionActive = User.Subscription.EndDate > DateTime.UtcNow.ToUniversalTime();
                AddGenres = new ObservableCollection<GenreModel>(movieRepository.LoadGenreCollection().Where(x => !User.Subscription.SubGenre.Where(i => i.Genre.Id == x.Id).Any()).Select(i => i));
            }

            Genres = movieRepository.LoadGenreCollection();
            this.UpdateView = UpdateView;
        }

        private void UpdateSubscriptionCommand(object obj)
        {
            ObservableCollection<SubscriptionGenre> newGenres = new ObservableCollection<SubscriptionGenre>();
            AddGenres.Where(x => x.IsChecked).ToList().ForEach(x => newGenres.Add(new SubscriptionGenre() { GenreId = x.Id, SubscriptionId = User.Subscription.Id }));
            userRepository.AddSubscriptionGenres(newGenres, EditSubscriptionPrice, User);
            UpdateView?.Invoke();
        }

        private void RenewSubscriptionAction(object obj)
        {
            Subscription newSub = new Subscription()
            {
                StartDate = DateTime.UtcNow.ToUniversalTime(),
                EndDate = DateTime.UtcNow.AddMonths(1 + RenewType * 6).ToUniversalTime(),
                Price = RenewPrices[RenewType],
                user = User,
                UserId = User.Id,
                SubGenre = new ObservableCollection<SubscriptionGenre>()
            };
            User.Subscription.SubGenre.ToList().ForEach(x => newSub.SubGenre.Add(new SubscriptionGenre() { Genre = x.Genre, Subscription = newSub }));

            userRepository.RenewSubscription(User, newSub);
            UpdateView?.Invoke();
        }

        private void RemoveSubscription(object obj)
        {
            if(obj != null && obj.Equals("Remove"))
            {
                userRepository.DisableSubscription(User);
                UpdateView?.Invoke();
            }
            else
                DisableSubscription = false;
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
            UpdateView?.Invoke();
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
