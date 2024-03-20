using MiniBank.Controllers;
using MiniBank.Exceptions;
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
            var users = UserController.GetAll();

            if (users.Count == 0)
            {
                Console.WriteLine("No users exist in the bank.");
            }
            
            users.ForEach(x => ColorWriter.DisplaySuccessMessage(x.Username));
        }


        internal void Delete()
        {
            SessionManager.Authorize(() =>
            {
                ColorWriter.DisplayPrimary("Are you sure you want to delete your user? (y): ");
                var input = Console.ReadLine();

                if (input == "y")
                {
                    UserController.Delete(SessionManager.LoggedUser.Username);

                    SessionManager.Authenticate();
                    ColorWriter.DisplaySuccessMessage("User deleted successfully.");
                } else
                {
                    Console.WriteLine("Deletion aborted.");
                }
            });
        }

        internal void Create()
        {
            var passwordManager = new PasswordManager();

            var username = ColorWriter.GetValidInputString("Create a username: ");
            EnsureUniqueUsername(username);

            ColorWriter.DisplayPrimary("Create a password: ");
            var password = passwordManager.GetPasswordInput();
            ColorWriter.DisplayPrimary("Retype your password to confirm: ");
            var retypedPassword = passwordManager.GetPasswordInput();

            if (password != retypedPassword)
            {
                Console.WriteLine("Passwords don't match.");
                return;
            } 

            var hashedPassword = passwordManager.HashPassword(password);

            UserController.Create(username, hashedPassword);

            ColorWriter.DisplaySuccessMessage($"User created successfully.");
        }

        private void EnsureUniqueUsername(string username)
        {
            try
            {
                var user = UserController.GetByUsername(username);

                throw new UserAlreadyExistsException($"Username is taken.");
            }
            catch (NotFoundException)
            {
                return;
            }
        }
    }
}