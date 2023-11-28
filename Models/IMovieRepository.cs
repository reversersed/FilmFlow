using FilmFlow.Models.BaseTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

namespace FilmFlow.Models
{
    interface IMovieRepository
    {
        ObservableCollection<MovieModel> LoadMovies(int limit);
        ObservableCollection<MovieModel> LoadNewMovies();
        ObservableCollection<MovieModel> LoadFilteredMovies(string name);
        ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds);
        ObservableCollection<MovieModel> LoadMoviesByGenre(GenreModel genregenreSearch);
        ObservableCollection<GenreModel> LoadGenreCollection();
        ObservableCollection<MovieModel> GetPopularMovies(int days);
        ObservableCollection<MovieModel> GetMostRated();
        ObservableCollection<CountryModel> GetCountries();
        int GetCount();
        MovieModel LoadMovieById(int id);
        void AddMovie(Movie movie);
    }
}
