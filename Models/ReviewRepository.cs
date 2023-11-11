using FilmFlow.Models.BaseTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    public class ReviewRepository : IReviewRepository
    {
        public void AddReview(float rating, string text, Movie movie, User user)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                db.reviews.Add(new Review
                {
                    Rating = rating,
                    ReviewText = text,
                    Movie = movie,
                    User = user
                });
                db.SaveChanges();
            }
        }
    }
}
