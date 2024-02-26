using Microsoft.Data.SqlClient;
using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                user.Name = reader.GetString (reader.GetOrdinal("Name"));
            }

            return user;
        }
    }
}
