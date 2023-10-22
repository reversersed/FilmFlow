using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace FilmFlow.Models
{
    public class MovieRepository : IMovieRepository
    {
        public MovieRepository() 
        {

        }

        public void LoadMovies(ObservableCollection<MovieModel> Movies)
        {
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach(Movie movie in db.movies.Include(e => e.Cover).Include(e => e.Genre).ThenInclude(i => i.Genre).ToList()) 
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel() { 
                        Id = movie.Id, 
                        Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.NameRu : movie.NameEn, 
                        Cover = movie.Cover.Url,
                        Desription = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.DescriptionRu : movie.DescriptionEn,
                        Price = movie.Price,
                        Genres = new ObservableCollection<GenreModel>(genres),
                        Rating = movie.Rating
                    });
                }
            }
        }
    }
}
