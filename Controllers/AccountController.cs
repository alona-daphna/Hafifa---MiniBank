using Microsoft.Data.SqlClient;
using MiniBank.Models;
using MiniBank.Utils;
using Serilog;
using MiniBank.Nhibernate;
using MiniBank.Exceptions;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        private decimal MaxBalance { get; set; } = 100000000000000;

        internal List<Account> GetByOwnerId(string id)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var user = session.Get<User>(id);

                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }

                var accounts = user.Accounts;

                Logger.Information("user {id} retrieves accounts", id);

                return accounts;
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving accounts");

                throw;
            }
        }

        internal Account GetByID(string id)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var account = session.Get<Account>(id);

                return account ?? throw new NotFoundException("Account not found");
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving account with ID {id}", id);

                throw;
            }
        }

        internal void Delete(string ownerId, string accountId)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var transaction = session.BeginTransaction();
                var account = session.Get<Account>(accountId) ?? throw new NotFoundException();

                EnsureOwnership(account, ownerId);
                session.Delete(account);
                transaction.Commit();

                Logger.Information("account {accountId} got deleted by {ownerId}", accountId, ownerId);
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in deleting account {accountId} owned by {ownerId}", accountId, ownerId);

                throw;
            }
        }

        internal string Create(string ownerId, int type)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                using var transaction = session.BeginTransaction();

                var owner = session.Get<User>(ownerId);
                var account = new AccountFactory().Create(type, owner);

                session.Save(account);
                transaction.Commit();

                Logger.Information("Created account for user {ownerId}", ownerId);

                return account.ID;
            } catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                   throw new ForeignKeyConstraintException("Invalid account type.");
                }

                Logger.Error(ex, "Error in creating account of type {type} for user {ownerId}", type, ownerId);

                throw;
            }
        }

        internal decimal Deposit(string ownerId, string accountId, decimal amount)
        {
            try
            {
                return UpdateBalance(accountId, ownerId, amount, (account, amount) => account.Balance += amount, "Deposited");
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in depositing to account {accountId} owned by {ownerId}", accountId, ownerId);

                throw;
            }
        }

    internal decimal Withdraw(string ownerId, string accountId, decimal amount)
        { 
            try
            {
                return UpdateBalance(accountId, ownerId, amount, (account, amount) => { 
                    if (account.GetType() == typeof(SimpleAccount) && account.Balance - amount < 0) 
                        throw new UnauthorizedOperationException("This account cannot be in overdraft"); 
                    account.Balance -= amount; 
                }, "Withdrew");
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in withdrawing from account {accountId} owned by {ownerId}", accountId, ownerId);

                throw;
            }
        }
        
        private decimal UpdateBalance(string accountId, string ownerId, decimal amount, Action<Account, decimal> updateBalance, string actionText)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var account = session.Get<Account>(accountId) ?? throw new NotFoundException();

            EnsureAmountPositive(amount);
            PreventOverflow(amount + account.Balance);
            EnsureOwnership(account, ownerId);

            updateBalance(account, amount);
            session.Save(account);
            transaction.Commit();

            Logger.Information("{action} {amount} account {accountId} owned by {ownerId}", actionText, amount, accountId, ownerId);

            return account.Balance;
        }

        private void PreventOverflow(decimal amount)
        {
            if (amount > MaxBalance) throw new InvalidAmountException("Amount exceeds limit");
        }

        private void EnsureAmountPositive(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException("Amount must be positive.");
            }
        }

        private void EnsureOwnership(Account account, string ownerID)
        {
            if (ownerID != account.Owner.ID)
            {
                throw new UnauthorizedAccessException("You don't own this account, operation is not authorized.");
            }
        }
    }
}