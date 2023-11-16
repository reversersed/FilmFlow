using FilmFlow.Models.BaseTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    interface IReviewRepository
    {
        void AddReview(float rating, string text, int movieid, int userid);
        int CountReviews(int movieid);
        ObservableCollection<Review> LoadReviews(int movie, int offset = 0, int limit = 10, ReviewFilter filter = ReviewFilter.Default);
        void DeleteMovieReviews(int movieid);
        void DeleteReview(int entityId);
        void DeleteUsersReview(int userId);
    }
}
