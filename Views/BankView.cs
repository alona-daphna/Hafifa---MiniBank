using MiniBank.Enums;
using MiniBank.Logs;
using MiniBank.Utils;

namespace MiniBank.Views
{
    internal class BankView
    {
        private AccountView AccountView { get; set; }
        private UserView UserView { get; set; }
        private ColorWriter ColorWriter { get; set; }
        private Dictionary<int, (Action action, Func<string> description, bool guestsAllowed)> Actions { get; set; }
        private SessionManager SessionManager { get; set; }

        public BankView()
        {
            SessionManager = new SessionManager();
            ColorWriter = new ColorWriter();
            AccountView = new AccountView(SessionManager);
            UserView = new UserView(SessionManager);
            Actions = new Dictionary<int, (Action, Func<string>, bool)>()
            {
                { (int) MenuAction.ListAccountsByOwner, (AccountView.ListAccounts, () => "LIST ACCOUNTS", false) },
                { (int) MenuAction.ListUsers, (UserView.ListUsers, () => "LIST USERS", true )},
                { (int) MenuAction.CreateUser, (UserView.CreateUser, () => "CREATE USER", true)},
                { (int) MenuAction.DeleteUser, (UserView.DeleteUser, () => "DELETE USER", false) },
                { (int) MenuAction.CreateAccount, (AccountView.CreateAccount, () => "CREATE ACCOUNT", false) },
                { (int) MenuAction.DeleteAccount, (AccountView.DeleteAccount, () => "DELETE ACCOUNT", false) },
                { (int) MenuAction.Deposit, (AccountView.Deposit, () => "DEPOSIT", false) },
                { (int) MenuAction.Withdraw, (AccountView.Withdraw, () => "WITHDRAW", false) },
                { (int) MenuAction.Auth, (SessionManager.Auth, () => SessionManager.IsUserLoggedIn ? "LOGOUT" : "LOGIN", true) },
            };

        }

        private void DisplayMenu()
        {
            ColorWriter.DisplayPrimary("############### MENU ###############");
            Actions.ToList().ForEach(action =>
            {
                if (SessionManager.IsUserLoggedIn || (action.Value.guestsAllowed && !SessionManager.IsUserLoggedIn))
                {
                    ColorWriter.DisplayPrimary($"{action.Key} \t {action.Value.description()}");
                } else
                {
                    Console.WriteLine($"{action.Key} \t {action.Value.description()}");
                }
            });

            ColorWriter.DisplayPrimary($"{(int)MenuAction.Exit} \t EXIT");
            ColorWriter.DisplayPrimary("####################################");
        }

        private bool TryParseAction(string input, out int action) => int.TryParse(input, out action) && Enum.IsDefined(typeof(MenuAction), action);

        internal void Start()
        {
            var exitCondition = false;

            do
            {
                DisplayMenu();

                Console.Write("(" + (SessionManager.IsUserLoggedIn ? SessionManager.LoggedUser.Name : "guest") + "): ");
                var input = Console.ReadLine();

                if (TryParseAction(input, out int action))
                {
                    if (action == (int)MenuAction.Exit)
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
    }
}