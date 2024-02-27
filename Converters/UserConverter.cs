using Microsoft.Data.SqlClient;
using MiniBank.Models;

namespace MiniBank.Converters
{
    internal class UserConverter
    {
        internal User Convert(SqlDataReader reader)
        {
            var user = new User();

            if (reader !=  null && reader.HasRows)
            {
                user.ID = reader.GetString(reader.GetOrdinal("ID"));
                user.Name = reader.GetString(reader.GetOrdinal("Name"));
                user.Password = reader.GetString(reader.GetOrdinal("Password"));
            }

            return user;
        }
    }
}