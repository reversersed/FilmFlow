using FilmFlow.Models.BaseTables;
using System;
using System.Collections.ObjectModel;

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
        void BanUser(int userId);
        void CreatePayment(int userId, int value);
        void CreateSubscription(Subscription subscription, User user);
        ObservableCollection<Subscription> GetSubscriptions(int userid, int limit);
        void DisableSubscription(User user);
        void RenewSubscription(User user, Subscription newSubscription);
        void AddSubscriptionGenres(ObservableCollection<SubscriptionGenre> genres, int price, User user);
        int GetTotalSubscriptions(DateOnly startDate, DateOnly endDate);
        int GetTotalSubscriptionCost(DateOnly startDate, DateOnly endDate);
        int GetTotalReplenishments(DateOnly startDate, DateOnly endDate);
        int GetTotalReplenishmentValue(DateOnly startDate, DateOnly endDate);
        GenreModel GetPopularSubscriptionGenre(DateOnly startDate, DateOnly endDate);
        ObservableCollection<GenreModel> GetReportList(DateOnly startDate, DateOnly endDate);
    }
}
