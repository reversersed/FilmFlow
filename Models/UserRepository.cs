using System.Linq;
using System.Security.Cryptography;
using System;
using FilmFlow.Models.BaseTables;

namespace FilmFlow.Models
{
    public class UserRepository : IUserRepository
    {
        public User AuthenticateUser(string username, string password, bool createSession)
        {
            using (var db = new RepositoryBase())
            {
                var user = db.users.SingleOrDefault(u => (u.Username.Equals(username) || u.Email.Equals(username)) && u.Password.Equals(MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))));
                if (user != default(User) && createSession)
                {
                    FilmFlow.Properties.Settings.Default.userSessionKey = string.Concat(user.Email, user.Id.ToString(), user.Username, GenerateToken());
                    FilmFlow.Properties.Settings.Default.Save();
                    db.sessions.Add(new Session() { SessionKey = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(string.Concat(FilmFlow.Properties.Settings.Default.appSessionKey, FilmFlow.Properties.Settings.Default.userSessionKey))), UserId = user.Id });
                    db.SaveChanges();
                }
                return user;
            }
        }
        private string GenerateToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, new Random().Next(16,25))
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
        public string? AuthenticateUser()
        {
            var sessionkey = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(string.Concat(FilmFlow.Properties.Settings.Default.appSessionKey, FilmFlow.Properties.Settings.Default.userSessionKey)));
            using (RepositoryBase db  = new RepositoryBase())
            {
                var user = db.sessions.Where(s => s.SessionKey.Equals(sessionkey)).Join(db.users, outer => outer.UserId, inner => inner.Id, (session, user) => new { Session = session, User = user }).Select(i => i);
                return user.Select(user => user.User.Username.ToString()).FirstOrDefault();
            }
        }
        public bool isUniqueUser(string username)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                var users = db.users.ToList();
                if (users.Where(i => i.Username.Equals(username)).Count() > 0)
                    return false;
                return true;
            }
        }
        public bool isUniqueEmail(string email)
        {
            using (RepositoryBase db = new RepositoryBase())
            {
                var users = db.users.ToList();
                if (users.Where(i => i.Email.Equals(email)).Count() != 0)
                    return false;
                return true;
            }
        }

        public void createUser(User user)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                db.users.Add(user);
                db.SaveChanges();
            }
        }

        public void Logout(User user)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                var userSession = db.sessions.Where(i => i.UserId == user.Id).Select(x => x).FirstOrDefault();
                if (userSession != default(Session))
                {
                    db.sessions.Remove(userSession);
                    db.SaveChanges();
                }
            }
        }

        public User LoadUserData(string username)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                return db.users.Where(i => i.Username.Equals(username)).Select(i => i).Single();
            }
        }

        public User GetByEmailOrUsername(string value)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                return (User)db.users.Where(i => i.Username.Equals(value) || i.Email.Equals(value)).Select(i => i).SingleOrDefault();
            }
        }

        public void ChangePassword(string username, string password)
        {
            using(RepositoryBase db = new RepositoryBase())
            {
                User userToChange = db.users.Where(i => i.Username.Equals(username)).Select(i => i).Single();
                userToChange.Password = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                db.SaveChanges();
            }
        }
    }
}
