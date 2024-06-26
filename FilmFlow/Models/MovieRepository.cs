﻿using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace FilmFlow.Models
{
    public class MovieRepository : IMovieRepository
    {
        private RepositoryBase db;
        public MovieRepository()
        {
            db = new RepositoryBase();
        }
        public MovieRepository(RepositoryBase database)
        {
            db = database;
        }
        public ObservableCollection<MovieModel> GetPopularMovies(int days)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .Select(i => i)
                                        .Where(i => db.reviews.Where(x => x.MovieId == i.Id).Where(x => (DateTime.UtcNow.ToUniversalTime() - x.WriteDate.ToUniversalTime()).Days < days).Any())
                                        .OrderByDescending(x => db.reviews.Include(f => f.Movie).Where(i => i.Movie.Id == x.Id).Where(i => (DateTime.UtcNow.ToUniversalTime() - i.WriteDate.ToUniversalTime()).Days < days).Count())
                                        .Take(10)
                                        .ToList())
            {
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }        
        public ObservableCollection<MovieModel> GetMostRated()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .Select(i => i)
                                        .OrderByDescending(i => i.Rating)
                                            .ThenByDescending(x => db.reviews.Include(f => f.Movie).Where(i => i.Movie.Id == x.Id).Count())
                                        .Take(10)
                                        .ToList())
            {
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(string name)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .Where(i => EF.Functions.Like(i.NameEn.ToLower(), string.Format("%{0}%", name.ToLower())) ||
                                                    EF.Functions.Like(i.NameRu.ToLower(), string.Format("%{0}%", name.ToLower())))
                                        .Select(i => i)
                                        .OrderBy(x => x.Rating)
                                        .ToList())
            {
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }

        public ObservableCollection<MovieModel> LoadFilteredMovies(List<int> genreIds)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
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
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }

        public ObservableCollection<GenreModel> LoadGenreCollection()
        {
            ObservableCollection<GenreModel> Genres = new ObservableCollection<GenreModel>();
            foreach (var genre in db.genrecollection)
            {
                Genres.Add(new GenreModel()
                {
                    Id = genre.Id,
                    Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.NameRu : genre.NameEn
                });
            }
            return Genres;
        }

        public ObservableCollection<MovieModel> LoadMovies(int limit)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .OrderBy(i => i.Rating)
                                        .Take(limit)
                                        .ToList())
            {
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadNewMovies()
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .OrderByDescending(i => i.Year)
                                            .ThenByDescending(i => i.Id)
                                        .ToList())
            {
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            }
            return Movies;
        }
        public ObservableCollection<MovieModel> LoadMoviesByGenre(GenreModel genreSearch)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
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
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            return Movies;
        }

        public void AddMovie(Movie movie)
        {
            db.movies.Add(movie);
            db.SaveChanges();
        }

        public MovieModel LoadMovieById(int id)
        {
            if (id < 1)
                return default;

            Movie movie = db.movies
                                    .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                    .Include(e => e.Genre)
                                        .ThenInclude(i => i.Genre)
                                    .ToList()
                                    .Where(i => i.Id == id)
                                    .FirstOrDefault();
            return new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false);
        }

        public ObservableCollection<CountryModel> GetCountries()
        {
            ObservableCollection<CountryModel> Countries = new ObservableCollection<CountryModel>();
            foreach (var country in db.countries)
            {
                Countries.Add(new CountryModel()
                {
                    Id = country.Id,
                    Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? country.NameRu : country.NameEn
                });
            }
            return Countries;
        }

        public int GetCount() => db.movies.Count();

        public bool? IsInFavourite(int movie)
        {
            if (FilmFlow.Properties.Settings.Default.CurrentUser == null || FilmFlow.Properties.Settings.Default.CurrentUser.Length < 1)
                return null;
            return db.favourite.Where(i => i.MovieId == movie && i.UserId == db.users.Where(i => i.Username.Equals(FilmFlow.Properties.Settings.Default.CurrentUser)).Select(i => i.Id).First()).Any();
        }

        public void ChangeFavouriteState(int movie, int user)
        {
            if (!IsInFavourite(movie) ?? false)
                db.favourite.Add(new Favourite() { UserId = user, MovieId = movie });
            else
                db.favourite.Remove(db.favourite.Where(i => i.MovieId == movie && i.UserId == user).First());
            db.SaveChanges();
        }

        public ObservableCollection<MovieModel> GetFavouriteList(int user)
        {
            ObservableCollection<MovieModel> Movies = new ObservableCollection<MovieModel>();
            foreach (Movie movie in db.movies
                                        .Include(e => e.Country)
                                            .ThenInclude(e => e.Country)
                                        .Include(e => e.Genre)
                                            .ThenInclude(i => i.Genre)
                                        .Select(i => i)
                                        .ToList()
                                        .Where(i => db.favourite.Where(x => x.UserId == user && x.MovieId == i.Id).Any())
                                        .OrderBy(i => i.Rating)
                                        .ToList())
                Movies.Add(new MovieModel(movie, db.reviews.Where(e => e.MovieId == movie.Id).Count(), IsInFavourite(movie.Id) ?? false));
            return Movies;
        }
    }
}
