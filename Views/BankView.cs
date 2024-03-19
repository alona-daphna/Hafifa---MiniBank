using Microsoft.Data.SqlClient;
using MiniBank.Enums;
using MiniBank.Exceptions;
using MiniBank.Utils;
using System.Drawing;
using Console = Colorful.Console;

namespace MiniBank.Views
{
    internal class BankView
    {
        private AccountView AccountView { get; set; }
        private UserView UserView { get; set; }
        private ColorWriter ColorWriter { get; set; }
        private Dictionary<MenuAction, (Action action, Func<string> description, bool loginRequired)> Actions { get; set; }
        private SessionManager SessionManager { get; set; }

        public BankView()
        {
            SessionManager = new SessionManager();
            ColorWriter = new ColorWriter();
            AccountView = new AccountView(SessionManager);
            UserView = new UserView(SessionManager);
            Actions = new Dictionary<MenuAction, (Action, Func<string>, bool)>()
            {
                { MenuAction.ListAccountsByOwner, (AccountView.ListAccounts, () => "LIST ACCOUNTS", true) },
                { MenuAction.ListUsers, (UserView.ListUsers, () => "LIST USERS", false )},
                { MenuAction.CreateUser, (UserView.Create, () => "CREATE USER", false)},
                { MenuAction.DeleteUser, (UserView.Delete, () => "DELETE USER", true) },
                { MenuAction.CreateAccount, (AccountView.CreateAccount, () => "CREATE ACCOUNT", true) },
                { MenuAction.DeleteAccount, (AccountView.DeleteAccount, () => "DELETE ACCOUNT", true) },
                { MenuAction.Deposit, (AccountView.Deposit, () => "DEPOSIT", true) },
                { MenuAction.Withdraw, (AccountView.Withdraw, () => "WITHDRAW", true) },
                { MenuAction.Auth, (SessionManager.Authenticate, () => SessionManager.IsUserLoggedIn ? "LOGOUT" : "LOGIN", false) },
            };
        }


        internal void Start()
        {
            Console.WriteAscii("MiniBank", ColorWriter.PrimaryColor);
            MenuAction action;

            do
            {
                DisplayMenu();
                PrintPrompt();
                var input = Console.ReadLine();

                if (TryParseAction(input, out action))
                {
                    if (action != MenuAction.Exit)
                    {
                        TryAction(action);
                    }
                }
                else
                {
                    ColorWriter.DisplayErrorMessage("Invalid action.");
                }

            } while (action  != MenuAction.Exit);
        }


        private void TryAction(MenuAction action)
        {
            try
            {
                Actions[action].action();

            } catch (SqlException)
            {
                ColorWriter.DisplayErrorMessage("An error occurred unexpectedly. Please try again later.");
            }
            catch (Exception ex) when
                ( ex is UnauthorizedAccessException 
                | ex is ForeignKeyConstraintException 
                | ex is InvalidAmountException 
                | ex is NotFoundException)
            {
                ColorWriter.DisplayErrorMessage(ex.Message);
            }
        }


        private void DisplayMenu()
        {
            ColorWriter.DisplayPrimary("############### MENU ###############");
            Actions.ToList().ForEach(action =>
            {
                if (SessionManager.IsUserLoggedIn || !action.Value.loginRequired)
                {
                    ColorWriter.DisplayPrimary($"{(int)action.Key} \t {action.Value.description()}");
                } else
                {
                    Console.WriteLine($"{(int)action.Key} \t {action.Value.description()}");
                }
            });

            ColorWriter.DisplayPrimary($"{(int)MenuAction.Exit} \t EXIT");
            ColorWriter.DisplayPrimary("####################################");
        }


        private void PrintPrompt() => Console.Write("(" + (SessionManager.IsUserLoggedIn ? SessionManager.LoggedUser.Name : "guest") + ")@MiniBank: ");


        private bool TryParseAction(string input, out MenuAction action) => Enum.TryParse(input, out action) && Enum.IsDefined(typeof(MenuAction), action);
    }
}