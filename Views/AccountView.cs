using MiniBank.Controllers;
using MiniBank.Utils;
using MiniBank.Enums;

namespace MiniBank.Views
{
    internal class AccountView
    {
        private AccountController AccountController { get; set; } = new AccountController();
        private UserController UserController { get; set; } = new UserController();
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        internal void ListAccountsByOwner()
        {
            ColorWriter.DisplayPrimary("Enter user ID: ");
            var userID = Console.ReadLine();
            var (_, user, _) = UserController.GetByID(userID);

            if (user == null)
            {
                ColorWriter.DisplayErrorMessage("User does not exist.");               
            }
            else
            {
                var accounts = AccountController.GetByOwnerId(userID);

                if (accounts.Count == 0)
                {
                    Console.WriteLine("User do not own any accounts. Consider creating one.");
                }

                accounts.ForEach(x => ColorWriter.DisplaySuccessMessage($"{x.ID} --> Balance: {x.Balance}"));
            }
        }

        private (string, string, float) UpdateBalancePrompts()
        {
            ColorWriter.DisplayPrimary("Enter user ID: ");
            var userID = Console.ReadLine();

            if (UserController.GetByID(userID).Status == OperationStatus.NotFound) 
                throw new ArgumentException("User does not exist.");

            ColorWriter.DisplayPrimary("Enter account ID: ");
            var accountID = Console.ReadLine();

            if (AccountController.GetByID(accountID).Status == OperationStatus.NotFound) 
                throw new ArgumentException("Account does not exist.");

            ColorWriter.DisplayPrimary("Enter amount: ");
            var amount = float.TryParse(Console.ReadLine(), out float value) ? value : throw new ArgumentException("Invalid amount.");

            return (userID, accountID, amount);
        }

        internal void Deposit()
        {
            try
            {
                var (userID, accountID, amount) = UpdateBalancePrompts();
                var (status, data, error) = AccountController.Deposit(userID, accountID, amount);

                if (status == OperationStatus.Success)
                {
                    ColorWriter.DisplaySuccessMessage($"Successful deposit. Your current balance is {data}");
                } else
                {
                    ColorWriter.DisplayErrorMessage(status == OperationStatus.NotFound ? "Account does not exist" : error);
                }
            } catch (ArgumentException ex)
            {
                ColorWriter.DisplayErrorMessage(ex.Message);
            }
        }

        internal void Withdraw()
        {
            try
            {
                var (user, account, amount) = UpdateBalancePrompts();
                var (status, data, error) = AccountController.Withdraw(user, account, amount);

                if (status == OperationStatus.Success)
                {
                    ColorWriter.DisplaySuccessMessage($"Successful withdraw. Your current balance is {data}");
                }
                else
                {
                    ColorWriter.DisplayErrorMessage(status == OperationStatus.NotFound ? "Account does not exist" : error);
                }
            }
            catch (ArgumentException ex)
            {
                ColorWriter.DisplayErrorMessage(ex.Message);
            }
        }

        internal void CreateAccount()
        {
            ColorWriter.DisplayPrimary("Enter user ID: ");
            var userID = Console.ReadLine();

            ColorWriter.DisplayPrimary("Choose account type: ");
            new AccountFactory().AccountCreators.ToList().ForEach(x => Console.WriteLine($"{x.Key} \t {x.Value.name}"));
            var accountType = Console.ReadLine();

            if (int.TryParse(accountType, out var type)) 
            {
                var (status, data, error) = AccountController.Create(userID, type);

                if (status == OperationStatus.Success)
                {
                    ColorWriter.DisplaySuccessMessage($"Account created successfully. Your new account ID is {data}");
                } else
                {
                    ColorWriter.DisplayErrorMessage(error);
                }
            } else
            {
                ColorWriter.DisplayErrorMessage("Invalid account type.");
            }
        }

        internal void DeleteAccount()
        {
            ColorWriter.DisplayPrimary("Enter user ID: ");
            var userID = Console.ReadLine();

            ColorWriter.DisplayPrimary("Enter account ID: ");
            var accountID = Console.ReadLine();
            var (status, _, error) = AccountController.Delete(userID, accountID);

            if (status == OperationStatus.Success)
            {
                ColorWriter.DisplaySuccessMessage("Account deleted successfully.");
            } else
            {
                ColorWriter.DisplayErrorMessage(status == OperationStatus.NotFound ? "Account does not exist." : error);
            }
        }
    }
}