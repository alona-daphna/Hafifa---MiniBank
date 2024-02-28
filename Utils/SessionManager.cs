using MiniBank.Controllers;
using MiniBank.Models;

namespace MiniBank.Utils
{
    internal class SessionManager
    {
        internal User LoggedUser { get; set; }
        internal bool IsUserLoggedIn => LoggedUser != null;
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();


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
            ColorWriter.DisplayPrimary("Enter User ID: ");
            var userID = Console.ReadLine();

            ColorWriter.DisplayPrimary("Enter password: ");
            var password = Console.ReadLine();

            var (status, user, _) = new UserController().GetByID(userID);

            if (status == Enums.OperationStatus.Success)
            {
                if (new PasswordManager().VerifyPassword(password, user.Password))
                {
                    LoggedUser = user;
                } else
                {
                    ColorWriter.DisplayErrorMessage("Incorrect password.");
                }
            } else
            {
                ColorWriter.DisplayErrorMessage("User does not exist.");
            }
        }


        private void LogOut() => LoggedUser = null;
    }
}