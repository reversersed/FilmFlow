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
        private int _selectedMovie = -1;

        //Public properties
        public ObservableCollection<MovieModel> MovieList { get { return _movieList; } set { _movieList = value; OnPropertyChanged(nameof(MovieList)); } }
        public int SelectedMovie { get { return _selectedMovie; } set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); } }

        //Repository
        IMovieRepository movieRepository;

        //Commands
        private ICommand ShowMovie { get; }
        public ICommand OnMovieSelected { get { return new ViewModelCommand(x => ShowMovie?.Execute(MovieList[(int)x].Id)); } }

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
