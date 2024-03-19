using MiniBank.Models;
using Microsoft.Data.SqlClient;
using MiniBank.Utils;
using MiniBank.Enums;
using Serilog;
using MiniBank.Nhibernate;
using MiniBank.Exceptions;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        internal string Create(string username, string password) 
        {
            try
            {
                var id = Guid.NewGuid().ToString();

                using (var session = NhibernateConfig.SessionFactory.OpenSession())
                {
                    using var transaction = session.BeginTransaction();
                    var user = new User(username, password);
                    session.Save(user);
                    transaction.Commit();
                }

                Logger.Information("Created a new user with ID {id}", id);

                return id;
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in creating user.");

                throw;
            }
        }

        internal void Delete(string id) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var transaction = session.BeginTransaction();
                var user = GetByID(id);

                session.Delete(user);
                transaction.Commit();

                Logger.Information("Deleted user with ID {id}", id);
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in deleting user with ID {id}", id);

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
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving users");

                throw;
            }
        }


        internal User GetByID(string id) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var user = session.Get<User?>(id) ?? throw new NotFoundException("User not found");

                return user;

            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving user with ID {id}", id);

                throw;
            }
        }
    }
}