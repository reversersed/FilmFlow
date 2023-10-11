using FilmFlow.Models.BaseTables;

namespace FilmFlow.Models
{
    public interface IUserRepository
    {
        User AuthenticateUser(string username, string password, bool createSession);
        bool isUniqueUser(string username);
        bool isUniqueEmail(string email);
        string AuthenticateUser();
        void createUser(User user);
        void Logout(User user);
        User LoadUserData(string username);
        User GetByEmailOrUsername(string value);
        void ChangePassword(string username, string password);
    }
}
