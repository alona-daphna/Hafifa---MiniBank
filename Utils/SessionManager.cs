using MiniBank.Controllers;
using MiniBank.Models;

namespace MiniBank.Utils
{
    internal class SessionManager
    {
        internal User LoggedUser { get; set; }
        internal bool IsUserLoggedIn => LoggedUser != null;
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();

        internal void Auth()
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

            var (status, data, _) = new UserController().GetByID(userID);

            if (status == Enums.OperationStatus.Success)
            {
                LoggedUser = data;
            } else
            {
                ColorWriter.DisplayErrorMessage("User does not exist.");
            }
        }

        private void LogOut() => LoggedUser = null;

        internal bool EnsureLogin()
        {
            if (!IsUserLoggedIn)
            {
                ColorWriter.DisplayErrorMessage("You must login first.");

                return false;
            }

            return true;
        }
    }
}