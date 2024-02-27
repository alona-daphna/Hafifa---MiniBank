using MiniBank.Controllers;
using MiniBank.Utils;
using System.Security.Cryptography;

namespace MiniBank.Views
{
    internal class UserView(SessionManager sessionManager)
    {
        private UserController UserController { get; set; } = new UserController();
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        private SessionManager SessionManager { get; set; } = sessionManager;
        internal void ListUsers()
        {
            var users = UserController.GetAll();
            if (users.Count == 0)
            {
                Console.WriteLine("No users exist in the bank.");
            }
            
            users.ForEach(x => ColorWriter.DisplaySuccessMessage($"{x.ID} \t {x.Name}"));
        }

        internal void DeleteUser()
        {
            if (!SessionManager.EnsureLogin())
            {
                return;
            }

            ColorWriter.DisplayPrimary("Are you sure you want to delete your user? (y): ");
            var input = Console.ReadLine();

            if (input == "y")
            {
                var (status, _, error) = UserController.Delete(SessionManager.LoggedUser.ID);

                if (status == Enums.OperationStatus.Success)
                {
                    ColorWriter.DisplaySuccessMessage("User deleted successfully.");
                } else
                {
                    ColorWriter.DisplayErrorMessage(error);
                }
            } else
            {
                Console.WriteLine("Deletion aborted.");
            }

        }

        internal void CreateUser()
        {
            ColorWriter.DisplayPrimary("Enter your name: ");
            var name = Console.ReadLine();
            ColorWriter.DisplayPrimary("Create a password: ");
            var password = Console.ReadLine();
            var hashedPassword = new Password().HashPassword(password);

            var (status, id, error) = UserController.Create(name, hashedPassword);

            if (status == Enums.OperationStatus.Success)
            {
                ColorWriter.DisplaySuccessMessage($"User created successfully. Your ID is: {id}");
            } else
            {
                ColorWriter.DisplayErrorMessage(error);
            }
        }
    }
}