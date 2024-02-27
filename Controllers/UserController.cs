using MiniBank.Converters;
using MiniBank.Models;
using Microsoft.Data.SqlClient;
using MiniBank.Utils;
using MiniBank.Enums;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private UserConverter UserConverter { get; set; } = new UserConverter();

        internal Response<string> Create(string name) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("INSERT Users (ID, Name) VALUES (@ID, @Name)", conn);
            command.Parameters.AddWithValue("Name", name);
            var id = Guid.NewGuid().ToString();
            command.Parameters.AddWithValue("ID", id);

            try
            {
                command.ExecuteNonQuery();
                return new Response<string> { Status = OperationStatus.Success, Data = id };
            }
            catch (SqlException)
            {
                return new Response<string> { Status = OperationStatus.Error, ErrorMessage = "An error occurred while creating user. Please try again later." };
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

                return new Response<User?> { Status = OperationStatus.Success };
            } catch (SqlException)
            {
                return new Response<User?> { Status = OperationStatus.Error, ErrorMessage = "An error occurred while deleting the user. Please try again later." };
            }
        }

        internal List<User> GetAll() 
        {
            var users = new List<User>();

            using var conn = DBConnection.GetConnection();
           
            conn.Open();

            var command = new SqlCommand("SELECT ID, Name FROM Users", conn);

            using var reader = command.ExecuteReader();
                
            while (reader.Read())
            {
                users.Add(UserConverter.Convert(reader));
            }
                
            return users;
        }


        internal Response<User> GetByID(string id) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Name FROM Users WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Response<User> { Status = OperationStatus.Success, Data = UserConverter.Convert(reader) };
            }

            return new Response<User> { Status = OperationStatus.NotFound };
        }
    }
}