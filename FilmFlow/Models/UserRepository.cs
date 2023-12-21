﻿using System.Linq;
using System.Security.Cryptography;
using System;
using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Collections.ObjectModel;

namespace FilmFlow.Models
{
    public class UserRepository : IUserRepository
    {
        private RepositoryBase db;
        public UserRepository() 
        {
            db = new RepositoryBase();
        }
        public UserRepository(RepositoryBase db)
        {
            this.db = db;
        }

        public User AuthenticateUser(string username, string password, bool createSession)
        {
            if(username.Length < 1 || password.Length < 1)
                throw new ArgumentException();
            var user = db.users.SingleOrDefault(u => (u.Username.Equals(username) || u.Email.Equals(username)) && u.Password.Equals(MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))));
            if (user.SubscriptionId != null)
                user = db.users
                .Include(e => e.Subscription)
                    .ThenInclude(e => e.SubGenre)
                        .ThenInclude(e => e.Genre)
                .SingleOrDefault(u => (u.Username.Equals(username) || u.Email.Equals(username)) && u.Password.Equals(MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))));
            if (user != default(User) && createSession)
            {
                FilmFlow.Properties.Settings.Default.userSessionKey = string.Concat(user.Email, user.Id.ToString(), user.Username, GenerateToken());
                FilmFlow.Properties.Settings.Default.Save();
                user.Session = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(string.Concat(FilmFlow.Properties.Settings.Default.appSessionKey, FilmFlow.Properties.Settings.Default.userSessionKey)));
                db.users.Update(user);
                db.SaveChanges();
            }
            return user;
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
            return db.users.Where(s => s.Session.Equals(sessionkey)).Select(user => user.Username).FirstOrDefault();
        }
        public bool isUniqueUser(string username)
        {
            var users = db.users.ToList();
            if (users.Where(i => i.Username.Equals(username)).Count() > 0)
                return false;
            return true;
        }
        public bool isUniqueEmail(string email)
        {
            var users = db.users.ToList();
            if (users.Where(i => i.Email.Equals(email)).Count() != 0)
                return false;
            return true;
        }

        public void createUser(User user)
        {
            if(user == null || user == default(User) || user.Username.Length < 1)
                throw new ArgumentNullException(nameof(user));
            db.users.Add(user);
            db.SaveChanges();
        }

        public void Logout(User user)
        {
            user.Session = null;
            db.users.Update(user);
            db.SaveChanges();
        }

        public User LoadUserData(string username)
        {
            var user = db.users.FirstOrDefault(i => i.Username.Equals(username));
            if (user.SubscriptionId == null)
                return user;
            return db.users
                        .Where(i => i.Username.Equals(username))
                        .Include(e => e.Subscription)
                            .ThenInclude(e => e.SubGenre)
                                .ThenInclude(e => e.Genre)
                        .Select(i => i).Single();
        }

        public User? GetByEmailOrUsername(string value)
        {
            if (value == null || value.Length < 1)
                return null;
            var user = (User)db.users
                            .Where(i => i.Username.Equals(value) || i.Email.Equals(value))
                            .Select(i => i)
                            .SingleOrDefault();
            if (user == null)
                return null;
            if (user.SubscriptionId == null)
                return user;
            return (User)db.users
                            .Where(i => i.Username.Equals(value) || i.Email.Equals(value))
                            .Include(e => e.Subscription)
                                .ThenInclude(e => e.SubGenre)
                                    .ThenInclude(e => e.Genre)
                            .Select(i => i)
                            .SingleOrDefault();
        }

        public void ChangePassword(string username, string password)
        {
            User userToChange = db.users
                                .Where(i => i.Username.Equals(username))
                                .Select(i => i)
                                .Single();
            userToChange.Password = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            db.SaveChanges();
        }

        public void BanUser(int userId)
        {
            User contextUser = db.users.First(i => i.Id == userId);
            contextUser.Banned = true;
            db.SaveChanges();
        }

        public void CreatePayment(int userId, int value)
        {
            db.replenishments.Add(new Replenishment() { UserId = userId, Value = value, Date = DateTime.UtcNow.ToUniversalTime() });
            User user = db.users.Find(userId);
            user.Balance += value;
            db.users.Update(user);
            db.SaveChanges();
        }
        public ObservableCollection<Subscription> GetSubscriptions(int userid, int limit)
        {
            return new ObservableCollection<Subscription>(db.subscriptions.Where(x => x.UserId == userid).Include(i => i.SubGenre).ThenInclude(e => e.Genre).OrderByDescending(i => i.EndDate).Take(limit).ToList());
        }

        public void CreateSubscription(Subscription subscription, User user)
        {
            user.Balance -= subscription.Price;
            user.Subscription = subscription;
            db.Update(user);

            db.subscriptions.Add(subscription);
            db.SaveChanges();
        }

        public void DisableSubscription(User user)
        {
            user.Subscription = null;
            user.SubscriptionId = null;
            db.Update(user);
            db.SaveChanges();
        }

        public void RenewSubscription(User user, Subscription newSubscription)
        {
            user.Subscription.EndDate = DateTime.UtcNow.ToUniversalTime();
            db.subscriptions.Update(user.Subscription);

            db.Add(newSubscription);
            user.Subscription = newSubscription;
            user.Balance -= newSubscription.Price;
            db.users.Update(user);
            db.SaveChanges();
        }

        public void AddSubscriptionGenres(ObservableCollection<SubscriptionGenre> genres, int price, User user)
        {
            db.sub_genres.AddRange(genres);

            user.Balance -= price;
            db.users.Update(user);

            var subscription = db.subscriptions.Find(genres.First().SubscriptionId);
            subscription.Price += price;
            db.subscriptions.Update(subscription);

            db.SaveChanges();
        }
    }
}
