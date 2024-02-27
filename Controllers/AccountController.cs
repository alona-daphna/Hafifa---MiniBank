using Microsoft.Data.SqlClient;
using MiniBank.Converters;
using MiniBank.Models;
using MiniBank.Views;
using MiniBank.Utils;
using MiniBank.Enums;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private AccountConverter AccountConverter { get; set; } = new AccountConverter();   
        
        internal List<Account> GetByOwnerId(string id)
        {
            var accounts = new List<Account>();

            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Balance, OwnerID, Type FROM Accounts WHERE OwnerID = @OwnerID", conn);
            command.Parameters.AddWithValue("OwnerID", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                accounts.Add(AccountConverter.Convert(reader));
            }

            return accounts;
        }       
        internal Response<Account> GetByID(string id)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Balance, OwnerID, Type FROM Accounts WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Response<Account> { Status = OperationStatus.Success, Data = AccountConverter.Convert(reader) };
            }

            return new Response<Account> { Status = OperationStatus.NotFound };
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
                return new Response<Account?> { Status = OperationStatus.Success };
            } catch (SqlException)
            {
                return new Response<Account?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred while deleting the account. Please try again later." };
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
                return new Response<string> { Status = OperationStatus.Success, Data = id };
            } catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                   return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "Invalid user ID or account type." };
                }

                return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "An error occurred while creating the account. Please try again later." };
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

                return UpdateBalance(accountId, account.Balance);
            } catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
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

                return UpdateBalance(accountId, account.Balance);
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

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new Response<float> { Status = OperationStatus.Error, ErrorMessage = "An error occurred while updating account balance. Please try again later." };
            }

            return new Response<float> { Status = OperationStatus.Success, Data = updatedBalance };
        }
    }
}