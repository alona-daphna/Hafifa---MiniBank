using MiniBank.Controllers;
using MiniBank.Models;
using Serilog;

namespace MiniBank.Utils
{
    internal class SessionManager
    {
        internal User LoggedUser { get; set; }
        internal bool IsUserLoggedIn => LoggedUser != null;
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;


        internal void Authorize(Action action)
        {
            if (!IsUserLoggedIn)
            {
                ColorWriter.DisplayErrorMessage("You must login first.");

                return;
            }

            action();
        }

        internal void Authenticate()
        {
            if (IsUserLoggedIn)
            {
                LogOut();
            } else
            {
                LogIn();
            }
        }


        private void LogIn()
        { 
            var passwordManager = new PasswordManager();
            
            var username = ColorWriter.GetValidInputString("Enter username: ");

            var user = new UserController().GetByUsername(username);

            ColorWriter.DisplayPrimary("Enter password: ");
            var password = passwordManager.GetPasswordInput();

            if (passwordManager.VerifyPassword(password, user.Password))
            {
                LoggedUser = user;
                Logger.Information("User: {username} logged in", user.Username);
            } else
            {
                Logger.Information("Failed login attempt for user: {username}", user.Username);
                ColorWriter.DisplayErrorMessage("Incorrect password.");
            }
        }


        private void LogOut()
        {
            Logger.Information("User: {username} logged out", LoggedUser.Username);
            LoggedUser = null;
        }
    }
}