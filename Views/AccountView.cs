using MiniBank.Controllers;

namespace MiniBank.Views
{
    internal class AccountView
    {
        private AccountController AccountController { get; set; } = new AccountController();
        internal void DisplayAllAccounts()
        {
            var userID = Console.ReadLine();
            var accounts = AccountController.GetByOwnerId(userID);
            accounts.ForEach(x => Console.WriteLine($"{x.ID} --> Balance: {x.Balance}"));
        }
    }
}