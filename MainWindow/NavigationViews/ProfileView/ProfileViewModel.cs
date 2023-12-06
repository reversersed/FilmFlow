using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //Public properties
        public User User { get { return _user; } }
        public bool ReplenishModalVisibility { get { return _replenishModalVisibility; } set { _replenishModalVisibility = value; OnPropertyChanged(nameof(ReplenishModalVisibility)); } }    

        //Repositories
        UserRepository userRepository;

        //Commands
        public ICommand ReplenishModalMode { get; }

        //Methods
        public ProfileViewModel()
        {
            userRepository = new UserRepository();
            _user = userRepository.LoadUserData(FilmFlow.Properties.Settings.Default.CurrentUser);

            ReplenishModalMode = new ViewModelCommand(ReplenishModalModeCommand);
        }

        private void ReplenishModalModeCommand(object obj) => ReplenishModalVisibility = Convert.ToBoolean((string)obj);
    }
}
