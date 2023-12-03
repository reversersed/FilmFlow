using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.FavouriteView
{
    public class FavouriteViewModel : ViewModelBase
    {
        //Private properties
        private User user;
        private ObservableCollection<MovieModel> _movieList;

        //Public properties
        public ObservableCollection<MovieModel> MovieList { get { return _movieList; } set { _movieList = value; OnPropertyChanged(nameof(MovieList)); } }

        //Repository
        MovieRepository movieRepository;

        //Commands
        private ICommand ShowMovie;

        //Actions

        //Methods
        public FavouriteViewModel(User user, ICommand ShowMovie)
        {
            this.user = user;
            this.ShowMovie = ShowMovie;

            movieRepository = new MovieRepository();

            MovieList = movieRepository.GetFavouriteList(user.Id);
        }
    }
}
