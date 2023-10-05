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
        string AuthenticateUser();
        void Add(UserModel user);

    }
}
