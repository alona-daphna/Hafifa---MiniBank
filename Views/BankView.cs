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
        private Dictionary<int, (Action action, Func<string> description)> Actions { get; set; }
        private SessionManager SessionManager { get; set; }

        public BankView()
        {
            SessionManager = new SessionManager();
            ColorWriter = new ColorWriter();
            AccountView = new AccountView(SessionManager);
            UserView = new UserView(SessionManager);
            Actions = new Dictionary<int, (Action, Func<string>)>()
            {
                { (int) MenuAction.ListAccountsByOwner, (AccountView.ListAccountsByOwner, () => "LIST ACCOUNTS OWNED BY USER") },
                { (int) MenuAction.ListUsers, (UserView.ListUsers, () => "LIST USERS" )},
                { (int) MenuAction.CreateUser, (UserView.CreateUser, () => "CREATE USER")},
                { (int) MenuAction.DeleteUser, (UserView.DeleteUser, () => "DELETE USER") },
                { (int) MenuAction.CreateAccount, (AccountView.CreateAccount, () => "CREATE ACCOUNT") },
                { (int) MenuAction.DeleteAccount, (AccountView.DeleteAccount, () => "DELETE ACCOUNT") },
                { (int) MenuAction.Deposit, (AccountView.Deposit, () => "DEPOSIT") },
                { (int) MenuAction.Withdraw, (AccountView.Withdraw, () => "WITHDRAW") },
                { (int) MenuAction.Auth, (SessionManager.Auth, () => SessionManager.IsUserLoggedIn ? "LOGOUT" : "LOGIN") },
            };

        }

        private void DisplayMenu()
        {
            ColorWriter.DisplayPrimary("############### MENU ###############");
            Actions.ToList().ForEach(action =>
                Console.WriteLine($"{action.Key} \t {action.Value.description()}")
            );

            Console.WriteLine($"{(int)MenuAction.Exit} \t EXIT");
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