using FilmFlow.Models.BaseTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    interface IReviewRepository
    {
        void AddReview(float rating, string text, Movie movie, User user);
    }
}
