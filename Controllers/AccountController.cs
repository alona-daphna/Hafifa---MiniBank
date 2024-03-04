using Microsoft.Data.SqlClient;
using MiniBank.Models;
using MiniBank.Utils;
using MiniBank.Enums;
using Serilog;
using MiniBank.Nhibernate;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        internal Response<List<Account>> GetByOwnerId(string id)
        {
            var accounts = new List<Account>();

            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                accounts = session.Query<Account>().Where(x => x.OwnerID == id).ToList();

                Logger.Information("user {id} retrieves accounts", id);

                return new Response<List<Account>> { Status = OperationStatus.Success, Data = accounts };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving accounts");

                return new Response<List<Account>> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }
        

        internal Response<Account> GetByID(string id)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var account = session.Get<Account>(id);

                if (account == null)
                {
                    return new Response<Account> { Status = OperationStatus.NotFound };
                }

                return new Response<Account> { Status = OperationStatus.Success, Data = account };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving account with ID {id}", id);

                return new Response<Account> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<Account?> Delete(string ownerId, string accountId)
        {
            try
            {
                using var session = NhibernateConfig.SessionFactory.OpenSession();
                var transaction = session.BeginTransaction();
                var account = session.Get<Account?>(accountId);

                if (account == null)
                {
                    return new Response<Account?> { Status = OperationStatus.NotFound };
                }

                account.EnsureOwnership(ownerId);
                session.Delete(account);
                transaction.Commit();

                Logger.Information("account {accountId} got deleted by {ownerId}", accountId, ownerId);

                return new Response<Account?> { Status = OperationStatus.Success };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new Response<Account?> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in deleting account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<Account?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<string> Create(string ownerId, int type)
        {
            try
            {
                var id = Guid.NewGuid().ToString();

                using (var session = NhibernateConfig.SessionFactory.OpenSession())
                {
                    using var transaction = session.BeginTransaction();
                    var account = new AccountFactory().Create(type);
                    account.ID = id;
                    account.Balance = 0;
                    account.OwnerID = ownerId;

                    session.Save(account);
                    transaction.Commit();
                }

                    Logger.Information("Created account for user {ownerId}", ownerId);

                return new Response<string> { Status = OperationStatus.Success, Data = id };
            } catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                   return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "Invalid account type." };
                }

                Logger.Error(ex, "Error in creating account of type {type} for user {ownerId}", type, ownerId);

                return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<decimal> Deposit(string ownerId, string accountId, decimal amount)
        {
            try
            {
                return UpdateBalance(accountId, ownerId, amount, MenuAction.Deposit);
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in depositing to account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<decimal> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
            {
                return new Response<decimal> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }
        }


        internal Response<decimal> Withdraw(string ownerId, string accountId, decimal amount)
        { 
            try
            {
                return UpdateBalance(accountId, ownerId, amount, MenuAction.Withdraw);
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in withdrawing from account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<decimal> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
            {
                return new Response<decimal> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }
        }
        

        private Response<decimal> UpdateBalance(string accountId, string ownerId, decimal amount, MenuAction action)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var account = session.Get<Account>(accountId);

            if (account == null)
            {
                return new Response<decimal> { Status = OperationStatus.NotFound };
            }

            account.EnsureOwnership(ownerId);

            string actionText;

            if (action == MenuAction.Withdraw)
            {
                account.Withdraw(amount);
                actionText = "Withdrew";
            }
            else
            {
                account.Deposit(amount);
                actionText = "Deposited";
            }

            session.Save(account);
            transaction.Commit();

            Logger.Information("{action} {amount} account {accountId} owned by {ownerId}", actionText, amount, accountId, ownerId);

            return new Response<decimal> { Status = OperationStatus.Success, Data = account.Balance };
        }
    }
}