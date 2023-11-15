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

        public ObservableCollection<Review> LoadReviews(int movie, int offset = 0, int limit = 10)
        {
            var reviews = new ObservableCollection<Review>();
            using(RepositoryBase db = new())
            {
                foreach(Review review in db.reviews
                                            .Include(e => e.Movie)
                                            .Include(e => e.User)
                                            .ToList()
                                            .Where(i => i.Movie.Id == movie)
                                            .Skip(offset)
                                            .Take(limit)
                                            .ToList())
                {
                    reviews.Add(review);
                }
            }
            return reviews;
        }
    }
}
