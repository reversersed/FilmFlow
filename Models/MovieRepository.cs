using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FilmFlow.Models
{
    public class MovieRepository : IMovieRepository
    {
        public MovieRepository() 
        {

        }

        public GenreModel GetPopularGenre()
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                MovieGenre movieGenre = db.genres.Include(e => e.Genre).FirstOrDefault();
                return new GenreModel()
                {
                    Id = movieGenre.Genre.Id,
                    Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movieGenre.Genre.NameRu : movieGenre.Genre.NameEn
                };
            }
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(string name)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Cover)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Where(i => EF.Functions.Like(i.NameEn.ToLower(), string.Format("%{0}%",name.ToLower())) || 
                                                        EF.Functions.Like(i.NameRu.ToLower(), string.Format("%{0}%", name.ToLower())))
                                            .Select(i=>i)
                                            .OrderBy(x => x.Rating)
                                            .ToList())
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), genres));
                }
            }
            return Movies;
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Cover)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Select(i => i)
                                            .ToList()
                                            .Where(i => genreIds.Intersect(
                                                                    i.Genre
                                                                    .Select(i => i.Genre.Id)
                                                                    .ToList()
                                                                ).Count() == genreIds.Count())
                                            .Select(x => x)
                                            .OrderBy(x => x.Rating)
                                            .ToList())
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), genres));
                }
            }
            return Movies;
        }

        public ObservableCollection<GenreModel> LoadGenreCollection()
        {
            ObservableCollection<GenreModel> Genres = new ObservableCollection<GenreModel>();
            using(RepositoryBase db = new RepositoryBase())
            {
                foreach(var genre in db.genrecollection)
                {
                    Genres.Add(new GenreModel() {
                        Id = genre.Id,
                        Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.NameRu : genre.NameEn
                    });
                }
            }
            return Genres;
        }

        public ObservableCollection<MovieModel> LoadMovies()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach(Movie movie in db.movies
                                            .Include(e => e.Cover)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .OrderBy(i => i.Rating)
                                            .ToList()) 
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), genres));
                }
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadNewMovies()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Cover)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .OrderByDescending(i => i.Year)
                                                .ThenByDescending(i => i.Id)
                                            .ToList())
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), genres));
                }
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadMoviesByGenre(GenreModel genreSearch)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (RepositoryBase db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Cover)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Select(i => i)
                                            .ToList()
                                            .Where(i => i.Genre.Select(i => i.Genre.Id).ToList().Contains(genreSearch.Id))
                                            .OrderBy(i => i.Rating)
                                            .ToList())
                {
                    var genres = new ObservableCollection<GenreModel>();
                    foreach (var genre in movie.Genre)
                        genres.Add(new GenreModel
                        {
                            Id = genre.Genre.Id,
                            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                        });
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), genres));
                }
            }
            return Movies;
        }
    }
}
