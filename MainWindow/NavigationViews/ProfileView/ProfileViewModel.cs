using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
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
        private Visibility _replenishmentSumVisibility = Visibility.Visible;
        private Visibility _replenishmentPaymentVisibility = Visibility.Collapsed;
        private bool _paymentSucceed = false;
        private int? _paymentValue = null;

        //Public properties
        public User User { get { return _user; } set { _user = value; OnPropertyChanged(nameof(User)); } }
        public bool ReplenishModalVisibility { get { return _replenishModalVisibility; } set { _replenishModalVisibility = value; OnPropertyChanged(nameof(ReplenishModalVisibility)); } }
        public Visibility ReplenishmentSumVisibility { get { return _replenishmentSumVisibility; } set { _replenishmentSumVisibility = value; OnPropertyChanged(nameof(ReplenishmentSumVisibility)); } }
        public Visibility ReplenishmentPaymentVisibility { get { return _replenishmentPaymentVisibility; } set { _replenishmentPaymentVisibility = value; OnPropertyChanged(nameof(ReplenishmentPaymentVisibility)); } }
        public string PaymentValue { get { return _paymentValue == null ? null : _paymentValue.ToString()+" ₽"; } set
            {
                string trimmedValue = value?.Replace(" ₽", "");
                if(trimmedValue == null || trimmedValue.Length == 0)
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

        //Repositories
        UserRepository userRepository;

        //Commands
        public ICommand ReplenishModalMode { get; }
        public ICommand ProceedPayment { get; }
        public ICommand AbordPayment { get; }

        //Threads
        Thread? WaitingForResponse;

        //Actions
        Action ResponseFeedback;

        //Methods
        public ProfileViewModel()
        {
            userRepository = new UserRepository();
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
            ProceedPayment = new ViewModelCommand(ProceedPaymentCommand, (x) => _paymentValue > 0);
            AbordPayment = new ViewModelCommand(AbordPaymentCommand);

            ResponseFeedback = new Action(PaymentCompleted);
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
    }
}
