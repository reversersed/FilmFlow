using FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.AddMovie;
using FilmFlow.ViewModels;
using FilmFlow.MainWindow.NavigationViews.ModerationView.Sections;
using FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.Finance;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView
{
    public class ModerationViewModel : ViewModelBase
    {
        //Private properties
        private bool _addMoviePanel = false;
        private bool _financePanel = false;
        private ViewModelBase _childContentView;


        //Public properties
        public bool AddMoviePanel { get { return _addMoviePanel; } set { _addMoviePanel = value; ChangePanel(); OnPropertyChanged(nameof(AddMoviePanel)); } }
        public bool FinancePanel { get { return _financePanel; } set { _financePanel = value; ChangePanel(); OnPropertyChanged(nameof(FinancePanel)); } }
        public ViewModelBase ChildContentView { get { return _childContentView; } set { _childContentView = value; OnPropertyChanged(nameof(ChildContentView)); } }

        //Models
        ViewModelBase AddMovie;
        ViewModelBase Finance;

        //Methods
        public ModerationViewModel()
        {
            AddMovie = new AddMovieViewModel(); 
            Finance = new FinanceViewModel();
        }
        private void ChangePanel()
        {
            if (AddMoviePanel)
                ChildContentView = AddMovie;
            else if (FinancePanel)
                ChildContentView = Finance;
            else
                ChildContentView = null;
        }
    }
}
