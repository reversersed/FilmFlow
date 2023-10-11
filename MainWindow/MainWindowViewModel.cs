﻿using FilmFlow.MainWindow.NavigationViews.HomeView;
using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilmFlow.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        //Private properties
        private ViewModelBase _childContentView;
        private bool _logoutConfirm {  get; set; }

        //Child Views
        private ViewModelBase homeView;

        //Public properties
        public ViewModelBase ChildContentView { get { return _childContentView; } set { _childContentView = value; OnPropertyChanged(nameof(ChildContentView)); } }
        public bool LogoutConfirm { get { return _logoutConfirm;  }  set { _logoutConfirm = value; OnPropertyChanged(nameof(LogoutConfirm)); } }
        //Commands
        public ICommand LogoutButton { get; }
        public ICommand ShowHomeSection { get; }

        //Repositories
        UserRepository UserRepository { get; set; }
        public User User {  get; set; }

        //Actions
        public Action showStartWindow { get; set; }

        //Methods
        public MainWindowViewModel()
        {
            LogoutButton = new ViewModelCommand(LogoutButtonCommand);
            ShowHomeSection = new ViewModelCommand(ShowHomeSectionCommand);

            UserRepository = new UserRepository();
            User = UserRepository.LoadUserData(Thread.CurrentPrincipal.Identity.Name);

            homeView = new HomeViewModel();
            ChildContentView = homeView;
        }

        private void ShowHomeSectionCommand(object obj)
        {
            ChildContentView = homeView;
        }

        private void LogoutButtonCommand(object obj)
        {
            UserRepository.Logout(User);
            showStartWindow?.Invoke();
        }
    }
}
