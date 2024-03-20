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
        private BaseController BaseController { get; set; } = new BaseController();

        internal void Create(string username, string password) 
        {
            try
            {
                var user = new User(username, password);
                BaseController.SaveEntity(user);

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
                BaseController.DeleteEntity<User>(username);

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
                return BaseController.GetAll<User>();
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
                return BaseController.GetEntityByIdentifier<User>(username) 
                    ?? throw new NotFoundException("User not found");
            } catch (Exception ex)
            {
                Logger.Error(ex, "Error in retrieving user with username {username}", username);

                throw;
            }
        }
    }
}