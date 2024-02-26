using Microsoft.Data.SqlClient;
using MiniBank.Converters;
using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal Account GetByID(string id)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Balance, OwnerID, Type FROM Accounts WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                return AccountConverter.Convert(reader);
            }

            throw new ArgumentException("Account not found");
        }

        internal void Delete(string ownerId, string accountId)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("DELETE FROM Accounts WHERE OwnerID = @OwnerID AND ID = @ID", conn);
            command.Parameters.AddWithValue("OwnerID", ownerId);
            command.Parameters.AddWithValue("ID", accountId);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows == 0) throw new ArgumentException("Account with this ID of does not exist for this user");
        }

        internal string Create(string ownerId, int type)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("INSERT Accounts (ID, OwnerID, Type) VALUES (@ID, @OwnerID, @Type)", conn);
            var id = Guid.NewGuid().ToString();
            command.Parameters.AddWithValue("ID", id);
            command.Parameters.AddWithValue("OwnerID", ownerId);
            command.Parameters.AddWithValue("Type", type);

            command.ExecuteNonQuery();

            return id;
        }

        internal void Deposit(string ownerId, string accountId, float amount)
        {
            var account = GetByID(accountId);
            AuthorizeAccountOwner(ownerId, account.OwnerID);
            account.Deposit(amount);
            UpdateBalance(accountId, account.Balance);
        }

        internal void Withdraw(string ownerId, string accountId, float amount)
        {
            var account = GetByID(accountId);
            AuthorizeAccountOwner(ownerId, account.OwnerID);
            account.Withdraw(amount);
            UpdateBalance(accountId, account.Balance);
        }

        private void AuthorizeAccountOwner(string requestingOwnerId, string actualOwnerId)
        {
            if (requestingOwnerId != actualOwnerId)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
        }
        

        private void UpdateBalance(string accountId, float updatedBalance)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            using var command = new SqlCommand("UPDATE Accounts SET Balance = @UpdatedBalance WHERE ID = @AccountID", conn);
            command.Parameters.AddWithValue("UpdatedBalance", updatedBalance);
            command.Parameters.AddWithValue("AccountID", accountId);

            command.ExecuteNonQuery();
        }
    }
}
