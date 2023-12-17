using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using Npgsql.PostgresTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    public class ReviewRepository : IReviewRepository
    {
        public void AddReview(float rating, string text, int movieid, int userid)
        {
            using(RepositoryBase db = new())
            {
                Review review = new()
                {
                    Rating = rating,
                    ReviewText = text,
                    Movie = db.movies.FirstOrDefault(i => i.Id == movieid),
                    User = db.users.FirstOrDefault(i => i.Id == userid),
                    WriteDate = DateTime.UtcNow
                };

                db.reviews.Add(review);
                db.SaveChanges();
            }
        }

        public int CountReviews(int movieid)
        {
            using(RepositoryBase db = new())
            {
                return db.reviews   .Include(e => e.Movie)
                                    .ToList()
                                    .Count(i => i.Movie.Id == movieid);
            }
        }

        public ObservableCollection<Review> LoadReviews(int movie, int offset = 0, int limit = 10, ReviewFilter filter = ReviewFilter.Default)
        {
            if (movie < 1)
                return default;
            using(RepositoryBase db = new())
            {
                switch(filter)
                {
                    case ReviewFilter.Default:
                        return new ObservableCollection<Review>(db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .OrderByDescending(i => i.ReviewText.Length)
                                            .ThenByDescending(i => i.Rating)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList());
                    case ReviewFilter.RatingDesc:
                        return new ObservableCollection<Review>(db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .OrderByDescending(i => i.Rating)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList());
                    case ReviewFilter.RatingAsc:
                        return new ObservableCollection<Review>(db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .OrderBy(i => i.Rating)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList());
                    case ReviewFilter.DateDesc:
                        return new ObservableCollection<Review>(db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .OrderByDescending(i => i.WriteDate)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList());
                    case ReviewFilter.DateAsc:
                        return new ObservableCollection<Review>(db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .OrderBy(i => i.WriteDate)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList());
                    default:
                        return default;
                }    
            }
        }

        public void DeleteMovieReviews(int movieid)
        {
            using(RepositoryBase db = new())
            {
                db.reviews.RemoveRange(db.reviews.Include(i => i.Movie).Where(i => i.Movie.Id == movieid).ToList());
                db.SaveChanges();
            }
        }

        public void DeleteReview(int entityId)
        {
            using (RepositoryBase db = new())
            {
                db.reviews.Remove(db.reviews.First(i => i.Id == entityId));
                db.SaveChanges();
            }
        }

        public void DeleteUsersReview(int userId)
        {
            using (RepositoryBase db = new())
            {
                db.reviews.RemoveRange(db.reviews.Include(i => i.User).Where(i => i.User.Id == userId).ToList());
                db.SaveChanges();
            }
        }
    }
}
