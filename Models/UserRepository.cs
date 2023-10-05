using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add(UserModel user)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(string username, string password)
        {
            bool validUser = false;
            using(NpgsqlConnection connection=GetConnection())
            using(NpgsqlCommand command=connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "select * from users where username=@username and password=@password limit 1";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
                validUser = command.ExecuteNonQuery() != -1;
            }
            return validUser;
        }
    }
}
