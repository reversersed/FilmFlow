using FilmFlow.Models;
using FilmFlow.Models.BaseTables;

namespace FilmFlow.Tests
{
    public class UserTest
    {
        IUserRepository repository;
        [SetUp]
        public void Setup()
        {
            repository = new UserRepository();
        }

        [Test]
        public void NullUserCreation()
        {
            Assert.Throws<ArgumentNullException>(() => repository.createUser(default(User)));
        }
        [Test]
        public void NullUserAuthorization()
        {
            Assert.Throws<ArgumentException>(() => repository.AuthenticateUser("","",false));
        }
        [Test]
        public void NullUserSearch()
        {
            var user = repository.GetByEmailOrUsername("");
            var userInvalid = repository.GetByEmailOrUsername(new string('g',Random.Shared.Next(20,50)));
            Assert.IsNull(user);
            Assert.IsNull(userInvalid);
        }
    }
}