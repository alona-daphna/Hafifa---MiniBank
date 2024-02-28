using MiniBank.Enums;
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
        private Dictionary<MenuAction, (Action action, Func<string> description, bool guestsAllowed)> Actions { get; set; }
        private SessionManager SessionManager { get; set; }

        public BankView()
        {
            SessionManager = new SessionManager();
            ColorWriter = new ColorWriter();
            AccountView = new AccountView(SessionManager);
            UserView = new UserView(SessionManager);
            Actions = new Dictionary<MenuAction, (Action, Func<string>, bool)>()
            {
                { MenuAction.ListAccountsByOwner, (AccountView.ListAccounts, () => "LIST ACCOUNTS", false) },
                { MenuAction.ListUsers, (UserView.ListUsers, () => "LIST USERS", true )},
                { MenuAction.CreateUser, (UserView.Create, () => "CREATE USER", true)},
                { MenuAction.DeleteUser, (UserView.Delete, () => "DELETE USER", false) },
                { MenuAction.CreateAccount, (AccountView.CreateAccount, () => "CREATE ACCOUNT", false) },
                { MenuAction.DeleteAccount, (AccountView.DeleteAccount, () => "DELETE ACCOUNT", false) },
                { MenuAction.Deposit, (AccountView.Deposit, () => "DEPOSIT", false) },
                { MenuAction.Withdraw, (AccountView.Withdraw, () => "WITHDRAW", false) },
                { MenuAction.Auth, (SessionManager.Authenticate, () => SessionManager.IsUserLoggedIn ? "LOGOUT" : "LOGIN", true) },
            };
        }


        internal void Start()
        {
            Console.WriteAscii("MiniBank", ColorWriter.PrimaryColor);

            var exitCondition = false;

            do
            {
                DisplayMenu();
                PrintPrompt();
                var input = Console.ReadLine();

                if (TryParseAction(input, out MenuAction action))
                {
                    if (action == MenuAction.Exit)
                    {
                        exitCondition = true;
                    }
                    else
                    {
                        Actions[action].action();
                    }
                }
                else
                {
                    ColorWriter.DisplayErrorMessage("Invalid action.");
                }

            } while (!exitCondition);
        }


        private void DisplayMenu()
        {
            ColorWriter.DisplayPrimary("############### MENU ###############");
            Actions.ToList().ForEach(action =>
            {
                if (SessionManager.IsUserLoggedIn || action.Value.guestsAllowed)
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