using MiniBank.Models;
using Microsoft.Data.SqlClient;
using MiniBank.Utils;
using MiniBank.Enums;
using Serilog;
using MiniBank.Nhibernate;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        internal Response<string?> Create(string name, string password) 
        {
            try
            {
                var id = Guid.NewGuid().ToString();

                using (var session = NhibernateConfig.SessionFactory.OpenSession())
                {
                    using var transaction = session.BeginTransaction();
                    var user = new User { ID = id, Name = name, Password = password };
                    session.Save(user);
                    transaction.Commit();
                }

                Logger.Information("Created a new user with ID {id}", id);

                return new Response<string?> { Status = OperationStatus.Success, Data = id };
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in creating user.");

                return new Response<string?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }

        internal Response<User?> Delete(string id) 
        {
            try
            {
                using (var session = NhibernateConfig.SessionFactory.OpenSession())
                {
                    var transaction = session.BeginTransaction();
                    var user = session.Get<User?>(id);

                    if (user == null)
                    {
                        return new Response<User?> { Status = OperationStatus.NotFound };
                    }

                    session.Delete(user);
                    transaction.Commit();

                    Logger.Information("Deleted user with ID {id}", id);

                    return new Response<User?> { Status = OperationStatus.Success };
                }
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in deleting user with ID {id}", id);

                return new Response<User?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }

        internal Response<List<User?>> GetAll() 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var users = session.Query<User?>().ToList();

                return new Response<List<User?>> { Status = OperationStatus.Success, Data = users };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving users");

                return new Response<List<User?>> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<User?> GetByID(string id) 
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var user = session.Get<User?>(id);

                if (user == null)
                {
                    return new Response<User?> { Status = OperationStatus.NotFound };
                }

                return new Response<User?> { Status = OperationStatus.Success, Data = user };

            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving user with ID {id}", id);

                return new Response<User?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }
    }
}