using Microsoft.Data.SqlClient;
using MiniBank.Converters;
using MiniBank.Models;
using MiniBank.Utils;
using MiniBank.Enums;
using Serilog;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private AccountConverter AccountConverter { get; set; } = new AccountConverter();
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;

        internal Response<List<Account>> GetByOwnerId(string id)
        {
            var accounts = new List<Account>();

            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Balance, OwnerID, Type FROM Accounts WHERE OwnerID = @OwnerID", conn);
            command.Parameters.AddWithValue("OwnerID", id);

            try
            {
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    accounts.Add(AccountConverter.Convert(reader));
                }

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
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Balance, OwnerID, Type FROM Accounts WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            try
            {
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Response<Account> { Status = OperationStatus.Success, Data = AccountConverter.Convert(reader) };
                }

                return new Response<Account> { Status = OperationStatus.NotFound };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving account with ID {id}", id);

                return new Response<Account> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<Account?> Delete(string ownerId, string accountId)
        {
            var ( status, account, error ) = GetByID(accountId);

            if (status != OperationStatus.Success)
            {
                return new Response<Account?> { Status = status, ErrorMessage = error };
            }

            try
            {
                account.EnsureOwnership(ownerId);
            } catch (UnauthorizedAccessException ex) 
            {
                return new Response<Account?> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }

            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("DELETE FROM Accounts WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", accountId);

            try
            {
                command.ExecuteNonQuery();
                Logger.Information("account {accountId} got deleted by {ownerId}", accountId, ownerId);

                return new Response<Account?> { Status = OperationStatus.Success };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in deleting account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<Account?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<string> Create(string ownerId, int type)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("INSERT Accounts (ID, OwnerID, Type) VALUES (@ID, @OwnerID, @Type)", conn);
            var id = Guid.NewGuid().ToString();
            command.Parameters.AddWithValue("ID", id);
            command.Parameters.AddWithValue("OwnerID", ownerId);
            command.Parameters.AddWithValue("Type", type);

            try
            {
                command.ExecuteNonQuery();
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


        internal Response<float> Deposit(string ownerId, string accountId, float amount)
        {
            var (status, account, error) = GetByID(accountId);

            if (status != OperationStatus.Success)
            {
                return new Response<float> { Status = status, ErrorMessage = error };
            }
         
            try
            {
                account.EnsureOwnership(ownerId);
                account.Deposit(amount);

                var response = UpdateBalance(accountId, account.Balance);
                Logger.Information("Deposited {amount} to account {accountId} owned by {ownerId}", amount, accountId, ownerId);

                return response;
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in depositing to account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<float> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
            {
                return new Response<float> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }
        }


        internal Response<float> Withdraw(string ownerId, string accountId, float amount)
        {
            var (status, account, error) = GetByID(accountId);

            if (status != OperationStatus.Success)
            {
                return new Response<float> { Status = status, ErrorMessage = error };
            }

            try
            {
                account.EnsureOwnership(ownerId);
                account.Withdraw(amount);


                var response = UpdateBalance(accountId, account.Balance);
                Logger.Information("Withdrew {amount} from account {accountId} owned by {ownerId}", amount, accountId, ownerId);

                return response;
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in withdrawing from account {accountId} owned by {ownerId}", accountId, ownerId);

                return new Response<float> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
            {
                return new Response<float> { Status = OperationStatus.Error, ErrorMessage = ex.Message };
            }
        }


        private Response<float> UpdateBalance(string accountId, float updatedBalance)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            using var command = new SqlCommand("UPDATE Accounts SET Balance = @UpdatedBalance WHERE ID = @AccountID", conn);
            command.Parameters.AddWithValue("UpdatedBalance", updatedBalance);
            command.Parameters.AddWithValue("AccountID", accountId);

            command.ExecuteNonQuery();

            return new Response<float> { Status = OperationStatus.Success, Data = updatedBalance };
        }
    }
}