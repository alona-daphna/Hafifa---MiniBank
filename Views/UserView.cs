using MiniBank.Controllers;
using MiniBank.Utils;
using System.Drawing;

namespace MiniBank.Views
{
    internal class UserView(SessionManager sessionManager)
    {
        private UserController UserController { get; set; } = new UserController();
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        private SessionManager SessionManager { get; set; } = sessionManager;


        internal void ListUsers()
        {
            var (status, users, error) = UserController.GetAll();

            if (status == Enums.OperationStatus.Success)
            {
                if (users.Count == 0)
                {
                    Console.WriteLine("No users exist in the bank.");
                }
            
                users.ForEach(x => ColorWriter.DisplaySuccessMessage($"{x.ID} \t {x.Name}"));
            }
            else
            {
                ColorWriter.DisplayErrorMessage(error);
            }
        }


        internal void Delete()
        {
            SessionManager.Authorize(() =>
            {
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
            });
        }

        internal void Create()
        {
            var name = ColorWriter.GetValidInputString("Enter your name: ");

            var password = ColorWriter.GetValidInputString("Create a password: ");

            var hashedPassword = new PasswordManager().HashPassword(password);

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