using DeviceId;
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
            using (NpgsqlConnection connection = GetConnection())
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "select users.id from users where username=@username and password=@password limit 1";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return false;
                    if (FilmFlow.Properties.Settings.Default.RememberPassword)
                    {
                        FilmFlow.Properties.Settings.Default.UserId = reader.GetInt32(0);
                        FilmFlow.Properties.Settings.Default.Save();
                    }
                }
            }
            return true;
        }

        public string? AuthenticateUser()
        {
            using (NpgsqlConnection connection = GetConnection())
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "select users.username from users where users.id = @userid limit 1";
                command.Parameters.AddWithValue("@userid", FilmFlow.Properties.Settings.Default.UserId);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    return reader.GetString(0);
                }
            }
        }
    }
}
