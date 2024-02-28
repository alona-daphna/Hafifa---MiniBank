using MiniBank.Converters;
using MiniBank.Models;
using Microsoft.Data.SqlClient;
using MiniBank.Utils;
using MiniBank.Enums;
using Serilog;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private UserConverter UserConverter { get; set; } = new UserConverter();
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;


        internal Response<string> Create(string name, string password) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("INSERT Users (ID, Name, Password) VALUES (@ID, @Name, @Password)", conn);
            var id = Guid.NewGuid().ToString();
            command.Parameters.AddWithValue("ID", id);
            command.Parameters.AddWithValue("Name", name);
            command.Parameters.AddWithValue("Password", password);

            try
            {
                command.ExecuteNonQuery();
                Logger.Information("Created a new user with ID {id}", id);

                return new Response<string> { Status = OperationStatus.Success, Data = id };
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Error in creating user.");

                return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }

        internal Response<User?> Delete(string id) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("DELETE FROM Users WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            try
            {
                if (command.ExecuteNonQuery() == 0)
                {
                    return new Response<User?> { Status = OperationStatus.NotFound };
                }

                Logger.Information("Deleted user with ID {id}", id);

                return new Response<User?> { Status = OperationStatus.Success };
            } catch (SqlException ex)
            {
                Logger.Information("Error in deleting user with ID {id}", id);

                return new Response<User?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }

        internal Response<List<User>> GetAll() 
        {
            var users = new List<User>();

            using var conn = DBConnection.GetConnection();
           
            conn.Open();

            var command = new SqlCommand("SELECT ID, Name, Password FROM Users", conn);

            try
            {
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    users.Add(UserConverter.Convert(reader));
                }

                return new Response<List<User>> { Status = OperationStatus.Success, Data = users };
            } catch (SqlException ex)
            {
                Logger.Error(ex, "Error in retrieving users");

                return new Response<List<User>> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }


        internal Response<User> GetByID(string id) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Name, Password FROM Users WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            try
            {
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Response<User> { Status = OperationStatus.Success, Data = UserConverter.Convert(reader) };
                }

                return new Response<User> { Status = OperationStatus.NotFound };
            } catch (SqlException)
            {
                return new Response<User> { Status = OperationStatus.Error, ErrorMessage = "An error occurred unexpectedly. Please try again later." };
            }
        }
    }
}