﻿using MiniBank.Controllers;
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
                var accounts = AccountController.GetByOwnerId(SessionManager.LoggedUser.ID);

                if (accounts.Count == 0)
                {
                    Console.WriteLine("User do not own any accounts. Consider creating one.");
                }

                accounts.ToList().ForEach(x => ColorWriter.DisplaySuccessMessage($"{x.ID} --> Balance: {x.Balance}"));
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
                    var id = AccountController.Create(SessionManager.LoggedUser.ID, type);

                    ColorWriter.DisplaySuccessMessage($"Account created successfully. Your new account ID is {id}");
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

                AccountController.Delete(SessionManager.LoggedUser.ID, accountID);

                ColorWriter.DisplaySuccessMessage("Account deleted successfully.");
            });
        }


        internal void Deposit()
        {
            SessionManager.Authorize(() =>
            {
                try
                {
                    var (accountID, amount) = UpdateBalancePrompts();
                    var balance = AccountController.Deposit(SessionManager.LoggedUser.ID, accountID, amount);
                    ColorWriter.DisplaySuccessMessage($"Successful deposit. Your current balance is {balance:0.00}");
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
                    var balance = AccountController.Withdraw(SessionManager.LoggedUser.ID, account, amount);
                    ColorWriter.DisplaySuccessMessage($"Successful withdraw. Your current balance is {balance:0.00}");
                }
                catch (ArgumentException ex)
                {
                    ColorWriter.DisplayErrorMessage(ex.Message);
                }
            });
        }


        private void PrintAccountTypes() => new AccountFactory().AccountCreators.ToList().ForEach(x => Console.WriteLine($"{x.Key} \t {x.Value.name}"));


        private (string, decimal) UpdateBalancePrompts()
        {
            ColorWriter.DisplayPrimary("Enter account ID: ");
            var accountID = Console.ReadLine();
            if (string.IsNullOrEmpty(accountID)) throw new ArgumentException("Invalid account ID");

            ColorWriter.DisplayPrimary("Enter amount: ");
            var amount = decimal.TryParse(Console.ReadLine(), out decimal value) ? value : throw new ArgumentException("Invalid amount.");

            return (accountID, amount);
        }
    }
}