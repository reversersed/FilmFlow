using Npgsql;
using System.Security.Cryptography;

namespace FilmFlow.Models
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
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
                        reader.Read();
                        FilmFlow.Properties.Settings.Default.UserId = (int)reader["Id"];
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
                    reader.Read();
                    return reader["username"].ToString();
                }
            }
        }
        public bool isUniqueUser(string username)
        {
            using (NpgsqlConnection connection = GetConnection())
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "select users.id from users where users.username = @username limit 1";
                command.Parameters.AddWithValue("@username", username);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return true;
                    return false;
                }
            }
        }
        public bool isUniqueEmail(string email)
        {
            using (NpgsqlConnection connection = GetConnection())
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "select users.id from users where users.email = @email limit 1";
                command.Parameters.AddWithValue("@email", email);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return true;
                    return false;
                }
            }
        }

        public void createUser(string username, string password, string email)
        {
            using (NpgsqlConnection connection = GetConnection())
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "insert into users (username, password, email) values (@username, @password, @email)";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
                command.Parameters.AddWithValue("@email", email);
                command.ExecuteNonQuery();
            }
        }
    }
}
