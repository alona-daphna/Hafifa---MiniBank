using Microsoft.Data.SqlClient;
using MiniBank.Models;
using MiniBank.Utils;
using Serilog;
using MiniBank.Nhibernate;
using MiniBank.Exceptions;
using NHibernate.Linq;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        private decimal MaxBalance { get; set; } = 100000000000000;

        internal List<Account> GetByOwner(string username)
        {
            try
            {
                using (var session = NhibernateConfig.SessionFactory.OpenSession())
                {
                    var user = session.Query<User>().Where(u => u.Username == username).Fetch(u => u.Accounts).SingleOrDefault() ?? throw new NotFoundException("User not found");
                    var accounts = user.Accounts;

                    Logger.Information("user: {username} retrieves accounts", username);

                    return [.. accounts];
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in retrieving account with ID {id}", id);

                throw;
            }
        }

        internal void Delete(string owner, string accountId)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var transaction = session.BeginTransaction();
                var account = session.Get<Account>(accountId) ?? throw new NotFoundException();

                EnsureOwnership(account, owner);
                session.Delete(account);
                transaction.Commit();

                Logger.Information("account {accountId} got deleted by {ownerId}", accountId, owner);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in deleting account {accountId} owned by {ownerId}", accountId, owner);

                throw;
            }
        }

        internal string Create(string username, int type)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                using var transaction = session.BeginTransaction();

                var owner = session.Get<User>(username);
                var account = new AccountFactory().Create(type, owner);

                session.Save(account);
                transaction.Commit();

                Logger.Information("Created account for user: {username}", owner.Username);

                return account.ID;
            } catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                   throw new ForeignKeyConstraintException("Invalid account type.");
                }

                Logger.Error(ex, "Error in creating account of type {type} for user: {usernamw}", type, username);

                throw;
            }
        }

        internal decimal Deposit(string owner, string accountIdLastFour, decimal amount)
        {
            try
            {
                return UpdateBalance(accountIdLastFour, owner, amount, (account, amount) => account.Balance += amount, "Deposited");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in depositing to account {accountId} owned by {ownerId}", accountIdLastFour, owner);

                throw;
            }
        }

    internal decimal Withdraw(string owner, string accountIdLastFour, decimal amount)
        { 
            try
            {
                return UpdateBalance(accountIdLastFour, owner, amount, (account, amount) => { 
                    if (account.GetType() == typeof(SimpleAccount) && account.Balance - amount < 0) 
                        throw new UnauthorizedOperationException("This account cannot be in overdraft"); 
                    account.Balance -= amount; 
                }, "Withdrew");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in withdrawing from account {accountId} owned by {ownerId}", accountIdLastFour, owner);

                throw;
            }
        }
        
        private decimal UpdateBalance(string accountIdLastFour, string owner, decimal amount, Action<Account, decimal> updateBalance, string actionText)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var account = session.Query<User>()
                                 .Where(u => u.Username == owner)
                                 .Fetch(u => u.Accounts)
                                 .FirstOrDefault()
                                 ?.Accounts.FirstOrDefault(a => a.ID.EndsWith(accountIdLastFour))
                                 ?? throw new NotFoundException();

            EnsureAmountPositive(amount);
            PreventOverflow(amount + account.Balance);
            EnsureOwnership(account, owner);
            Console.WriteLine(4);

            updateBalance(account, amount);
            session.Save(account);
            transaction.Commit();

            Logger.Information("{action} {amount} account {accountId} owned by {ownerId}", actionText, amount, account.ID, owner);

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

        private void EnsureOwnership(Account account, string username)
        {
            if (username != account.Owner.Username)
            {
                throw new UnauthorizedAccessException("You don't own this account, operation is not authorized.");
            }
        }
    }
}