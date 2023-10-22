using FilmFlow.Models.BaseTables;
using System.Collections.ObjectModel;

namespace FilmFlow.Models
{
    interface IMovieRepository
    {
        ObservableCollection<MovieModel> LoadMovies();
        ObservableCollection<MovieModel> LoadFilteredMovies(string name);
        ObservableCollection<GenreModel> LoadGenreCollection();
    }
}
