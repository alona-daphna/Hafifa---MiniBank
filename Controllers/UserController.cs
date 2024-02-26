using MiniBank.Converters;
using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        private UserConverter UserConverter { get; set; } = new UserConverter();

        internal void Create() { }

        internal void Delete() { }

        internal List<User> GetAll() 
        {
            var users = new List<User>();

            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT ID, Name FROM Users";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(UserConverter.Convert(reader));
                    }
                }
            }

            return users;
        }

        internal User GetById(string id) { return new User(); }
    }
}
