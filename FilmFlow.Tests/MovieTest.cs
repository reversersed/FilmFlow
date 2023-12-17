using FilmFlow.Models;
using FilmFlow.Models.BaseTables;

namespace FilmFlow.Tests
{
    public class MovieTest
    {
        IMovieRepository repository;
        [SetUp]
        public void Setup()
        {
            repository = new MovieRepository();
        }

        [Test]
        public void MovieListLoadingTest()
        {
            const int movieToLoad = 100;
            int loaded = repository.LoadMovies(movieToLoad).Count;
            Assert.IsTrue(loaded == movieToLoad || loaded == repository.GetCount());
        }
        [Test]
        public void MovieLoadingTest()
        {
            Assert.That(repository.LoadMovieById(-1), Is.EqualTo(default(MovieModel)));
        }
        [Test]
        public void InvalidUserCheck()
        {
            var result = repository.IsInFavourite(Random.Shared.Next(1, 10));
            Assert.IsNull(result);
        }
    }
}