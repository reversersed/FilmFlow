using FilmFlow.Models.BaseTables;
using System.Collections.ObjectModel;

namespace FilmFlow.Models
{
    interface IMovieRepository
    {
        void LoadMovies(ObservableCollection<MovieModel> Movies);
    }
}
