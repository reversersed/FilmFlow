using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FilmFlow.Models
{
    interface IMovieRepository
    {
        ObservableCollection<MovieModel> LoadMovies();
        ObservableCollection<MovieModel> LoadFilteredMovies(string name);
        ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds);
        ObservableCollection<GenreModel> LoadGenreCollection();
    }
}
