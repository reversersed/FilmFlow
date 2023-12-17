using FilmFlow.Models;
using FilmFlow.Models.BaseTables;
using System.Collections.ObjectModel;

namespace FilmFlow.Tests
{
    public class ReviewTest
    {
        IReviewRepository repository;
        [SetUp]
        public void Setup()
        {
            repository = new ReviewRepository();
        }

        [Test]
        public void NullReviewLoad()
        {
            Assert.That(repository.LoadReviews(-1), Is.EqualTo(default(ObservableCollection<Review>)));
        }
    }
}