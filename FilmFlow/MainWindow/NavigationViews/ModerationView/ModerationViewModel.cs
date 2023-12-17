﻿using FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.AddMovie;
using FilmFlow.ViewModels;
using FilmFlow.MainWindow.NavigationViews.ModerationView.Sections;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView
{
    public class ModerationViewModel : ViewModelBase
    {
        //Private properties
        private bool _addMoviePanel = false;
        private ViewModelBase _childContentView;


        //Public properties
        public bool AddMoviePanel { get { return _addMoviePanel; } set { _addMoviePanel = value; ChangePanel(); OnPropertyChanged(nameof(AddMoviePanel)); } }
        public ViewModelBase ChildContentView { get { return _childContentView; } set { _childContentView = value; OnPropertyChanged(nameof(ChildContentView)); } }

        //Models


        //Commands


        //Methods
        public ModerationViewModel()
        {

        }
        private void ChangePanel()
        {
            if (AddMoviePanel)
                ChildContentView = new AddMovieViewModel();
            else
                ChildContentView = null;
        }
    }
}