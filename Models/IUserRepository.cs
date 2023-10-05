using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FilmFlow.Models
{
    public interface IUserRepository
    {
        bool AuthenticateUser(string username, string password);
        bool isUniqueUser(string username);
        bool isUniqueEmail(string email);
        string AuthenticateUser();
        void createUser(string username, string password, string email);

    }
}
