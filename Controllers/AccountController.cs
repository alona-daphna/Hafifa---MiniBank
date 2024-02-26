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

        internal void Deposit(string ownerId, string accountId, float amount) =>
            UpdateBalance(ownerId, accountId, amount, "Deposit");

        internal void Withdraw(string ownerId, string accountId, float amount) =>
            UpdateBalance(ownerId, accountId, amount, "Withdraw");

        private void UpdateBalance(string ownerId, string accountId, float amount, string operation)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var commandText = 
                operation == "Deposit" 
                ? "UPDATE Accounts SET Balance += @Amount WHERE OwnerID = @OwnerID AND ID = @AccountID" 
                : "UPDATE Accounts SET Balance -= @Amount WHERE OwnerID = @OwnerID AND ID = @AccountID";

            using var command = new SqlCommand(commandText, conn);
            command.Parameters.AddWithValue("Amount", amount);
            command.Parameters.AddWithValue("OwnerID", ownerId);
            command.Parameters.AddWithValue("AccountID", accountId);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows == 0) throw new ArgumentException($"Account with this ID does not exist for this user or the operation failed.");
        }
    }
}
