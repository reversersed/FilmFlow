using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace FilmFlow.Models
{
    public class MovieRepository : IMovieRepository
    {
        public MovieRepository() 
        {

        }

        public ObservableCollection<MovieModel> GetPopularMovies(int days)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Select(i => i)
                                            .OrderByDescending(x => db.reviews.Include(f => f.Movie).Where(i => i.Movie.Id == x.Id).Where(i => (i.WriteDate.ToUniversalTime()-DateTime.UtcNow.ToUniversalTime()).Days < days).Count())
                                            .Take(30)
                                            .ToList())
                {
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }        
        public ObservableCollection<MovieModel> GetMostRated()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Select(i => i)
                                            .OrderByDescending(i => i.Rating)
                                            .ThenByDescending(x => db.reviews.Include(f => f.Movie).Where(i => i.Movie.Id == x.Id).Count())
                                            .Take(30)
                                            .ToList())
                {
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(string name)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Where(i => EF.Functions.Like(i.NameEn.ToLower(), string.Format("%{0}%",name.ToLower())) || 
                                                        EF.Functions.Like(i.NameRu.ToLower(), string.Format("%{0}%", name.ToLower())))
                                            .Select(i=>i)
                                            .OrderBy(x => x.Rating)
                                            .ToList())
                {
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
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
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }

        public ObservableCollection<GenreModel> LoadGenreCollection()
        {
            ObservableCollection<GenreModel> Genres = new ObservableCollection<GenreModel>();
            using(var db = new RepositoryBase())
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
            using (var db = new RepositoryBase())
            {
                foreach(Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .OrderBy(i => i.Rating)
                                            .ToList()) 
                {
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadNewMovies()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .OrderByDescending(i => i.Year)
                                                .ThenByDescending(i => i.Id)
                                            .ToList())
                {
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
                }
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadMoviesByGenre(GenreModel genreSearch)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            using (var db = new RepositoryBase())
            {
                foreach (Movie movie in db.movies
                                            .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                            .Include(e => e.Genre)
                                                .ThenInclude(i => i.Genre)
                                            .Select(i => i)
                                            .ToList()
                                            .Where(i => i.Genre.Select(i => i.Genre.Id).ToList().Contains(genreSearch.Id))
                                            .OrderBy(i => i.Rating)
                                            .ToList())
                    Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count()));
            }
            return Movies;
        }

        public void AddMovie(Movie movie)
        {
            using(var db = new RepositoryBase())
            {
                db.movies.Add(movie);
                db.SaveChanges();
            }
        }

        public MovieModel LoadMovieById(int id)
        {
            using (var db = new RepositoryBase())
            {
                Movie movie = db.movies
                                        .Include(e => e.Country)
                                                .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .ToList()
                                        .Where(i => i.Id == id)
                                        .FirstOrDefault();
                return new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count());
            }
        }

        public ObservableCollection<CountryModel> GetCountries()
        {
            ObservableCollection<CountryModel> Countries = new ObservableCollection<CountryModel>();
            using (var db = new RepositoryBase())
            {
                foreach (var country in db.countries)
                {
                    Countries.Add(new CountryModel()
                    {
                        Id = country.Id,
                        Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? country.NameRu : country.NameEn
                    });
                }
            }
            return Countries;
        }
    }
}
