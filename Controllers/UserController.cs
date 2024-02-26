using MiniBank.Converters;
using MiniBank.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sql;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private UserConverter UserConverter { get; set; } = new UserConverter();

        internal string Create(string name) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("INSERT Users (ID, Name) VALUES (@ID, @Name)", conn);
            command.Parameters.AddWithValue("Name", name);
            var id = Guid.NewGuid().ToString();
            command.Parameters.AddWithValue("ID", id);

            command.ExecuteNonQuery();
            return id;
        }

        internal void Delete(string id) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("DELETE FROM Users WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows == 0) throw new ArgumentException("user not found");
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


        internal User GetById(string id) 
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT ID, Name FROM Users WHERE ID = @ID", conn);
            command.Parameters.AddWithValue("ID", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                return UserConverter.Convert(reader);
            }

            throw new ArgumentException("user not found");
        }
    }
}
