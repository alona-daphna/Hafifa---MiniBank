using MiniBank.Controllers;
using MiniBank.Enums;
using MiniBank.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Views
{
    internal class BankView
    {
        private AccountView AccountView { get; set; }
        private UserView UserView { get; set; }
        private ColorWriter ColorWriter { get; set; }
        private Dictionary<int, (Action action, string description)> Actions { get; set; }

        public BankView()
        {
            ColorWriter = new ColorWriter();
            AccountView = new AccountView();
            UserView = new UserView();
            Actions = new Dictionary<int, (Action, string)>()
            {
                { (int) MenuAction.ListAccountsByOwner, (AccountView.ListAccountsByOwner, "LIST ACCOUNTS OWNED BY USER") },
                { (int) MenuAction.ListUsers, (UserView.ListUsers, "LIST USERS" )},
                { (int) MenuAction.CreateUser, (UserView.CreateUser, "CREATE USER")},
                { (int) MenuAction.DeleteUser, (UserView.DeleteUser, "DELETE USER") },
                { (int) MenuAction.CreateAccount, (AccountView.CreateAccount, "CREATE ACCOUNT") },
                { (int) MenuAction.DeleteAccount, (AccountView.DeleteAccount, "DELETE ACCOUNT") },
                { (int) MenuAction.Deposit, (AccountView.Deposit, "DEPOSIT") },
                { (int) MenuAction.Withdraw, (AccountView.Withdraw, "WITHDRAW") }
            };

        }

        private void DisplayMenu()
        {
            ColorWriter.DisplayPrimary("############### MENU ###############");
            Actions.ToList().ForEach(action =>
                Console.WriteLine($"{action.Key} \t {action.Value.description}")
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

                Console.Write("Your Selection: ");
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
