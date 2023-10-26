using FilmFlow.Models.BaseTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

namespace FilmFlow.Models
{
    interface IMovieRepository
    {
        ObservableCollection<MovieModel> LoadMovies();
        ObservableCollection<MovieModel> LoadNewMovies();
        ObservableCollection<MovieModel> LoadFilteredMovies(string name);
        ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds);
        ObservableCollection<MovieModel> LoadMoviesByGenre(GenreModel genregenreSearch);
        ObservableCollection<GenreModel> LoadGenreCollection();
        GenreModel GetPopularGenre();
        void AddMovie(Movie movie, string coverUrl, string movieUrl);
    }
}
