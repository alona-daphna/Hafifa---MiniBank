using MiniBank.Models;
using MiniBank.Utils;
using Serilog;
using MiniBank.Nhibernate;
using MiniBank.Exceptions;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        internal void Create(string username, string password) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                using var transaction = session.BeginTransaction();
                var user = new User(username, password);
                session.Save(user);
                transaction.Commit();

                Logger.Information("Created a new user: {username}", user.Username);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in creating user.");

                throw;
            }
        }

        internal void Delete(string username) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var transaction = session.BeginTransaction();
                var user = GetByUsername(username);

                session.Delete(user);
                transaction.Commit();

                Logger.Information("Deleted user: {username}", username);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in deleting user: {username}", username);

                throw;
            }
        }

        internal List<User> GetAll() 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var users = session.Query<User>().ToList();

                return users;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in retrieving users");

                throw;
            }
        }


        internal User GetByUsername(string username) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var user = session.Get<User?>(username) ?? throw new NotFoundException("User not found");

                return user;

            } catch (Exception ex)
            {
                Logger.Error(ex, "Error in retrieving user with username {username}", username);

                throw;
            }
        }
    }
}