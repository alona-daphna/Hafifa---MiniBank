using MiniBank.Controllers;
using MiniBank.Utils;
using MiniBank.Enums;

namespace MiniBank.Views
{
    internal class AccountView(SessionManager sessionManager)
    {
        private AccountController AccountController { get; set; } = new AccountController();
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        private SessionManager SessionManager { get; set; } = sessionManager;


        internal void ListAccounts()
        {
            SessionManager.Authorize(() =>
            {
                var (status, accounts, error) = AccountController.GetByOwnerId(SessionManager.LoggedUser.ID);

                if (status == OperationStatus.Success)
                {
                    if (accounts.Count == 0)
                    {
                        Console.WriteLine("User do not own any accounts. Consider creating one.");
                    }

                    accounts.ForEach(x => ColorWriter.DisplaySuccessMessage($"{x.ID} --> Balance: {x.Balance.ToString("F0")}"));
                } else
                {
                    ColorWriter.DisplayErrorMessage(error);
                }

            });
        }


        internal void CreateAccount()
        {
            SessionManager.Authorize(() =>
            {
                ColorWriter.DisplayPrimary("Choose account type: ");
                PrintAccountTypes();
                var accountType = Console.ReadLine();

                if (int.TryParse(accountType, out var type)) 
                {
                    var (status, id, error) = AccountController.Create(SessionManager.LoggedUser.ID, type);

                    if (status == OperationStatus.Success)
                    {
                        ColorWriter.DisplaySuccessMessage($"Account created successfully. Your new account ID is {id}");
                    } else
                    {
                        ColorWriter.DisplayErrorMessage(error);
                    }
                } else
                {
                    ColorWriter.DisplayErrorMessage("Invalid account type.");
                }
            });
        }


        internal void DeleteAccount()
        {
            SessionManager.Authorize(() =>
            {
                ColorWriter.DisplayPrimary("Enter account ID: ");
                var accountID = Console.ReadLine();

                var (status, _, error) = AccountController.Delete(SessionManager.LoggedUser.ID, accountID);

                if (status == OperationStatus.Success)
                {
                    ColorWriter.DisplaySuccessMessage("Account deleted successfully.");
                } else
                {
                    ColorWriter.DisplayErrorMessage(status == OperationStatus.NotFound ? "Account does not exist." : error);
                }
            });
        }


        internal void Deposit()
        {
            SessionManager.Authorize(() =>
            {
                try
                {
                    var (accountID, amount) = UpdateBalancePrompts();
                    var (status, balance, error) = AccountController.Deposit(SessionManager.LoggedUser.ID, accountID, amount);

                    if (status == OperationStatus.Success)
                    {
                        ColorWriter.DisplaySuccessMessage($"Successful deposit. Your current balance is {balance.ToString("F0")}");
                    } else
                    {
                        ColorWriter.DisplayErrorMessage(error);
                    }
                } catch (ArgumentException ex)
                {
                    ColorWriter.DisplayErrorMessage(ex.Message);
                }
            });
        }


        internal void Withdraw()
        {
            SessionManager.Authorize(() =>
            {
                try
                {
                    var (account, amount) = UpdateBalancePrompts();
                    var (status, balance, error) = AccountController.Withdraw(SessionManager.LoggedUser.ID, account, amount);

                    if (status == OperationStatus.Success)
                    {
                        ColorWriter.DisplaySuccessMessage($"Successful withdraw. Your current balance is {balance.ToString("F0")}");
                    }
                    else
                    {
                        ColorWriter.DisplayErrorMessage(error);
                    }
                }
                catch (ArgumentException ex)
                {
                    ColorWriter.DisplayErrorMessage(ex.Message);
                }
            });
        }


        private void PrintAccountTypes() => new AccountFactory().AccountCreators.ToList().ForEach(x => Console.WriteLine($"{x.Key} \t {x.Value.name}"));


        private (string, float) UpdateBalancePrompts()
        {
            ColorWriter.DisplayPrimary("Enter account ID: ");
            var accountID = Console.ReadLine();

            if (AccountController.GetByID(accountID).Status == OperationStatus.NotFound)
            {
                throw new ArgumentException("Account does not exist.");
            }

            ColorWriter.DisplayPrimary("Enter amount: ");
            var amount = float.TryParse(Console.ReadLine(), out float value) ? value : throw new ArgumentException("Invalid amount.");

            return (accountID, amount);
        }
    }
}