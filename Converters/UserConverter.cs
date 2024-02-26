using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Converters
{
    internal class UserConverter
    {
        internal User Convert(DbDataReader dataReader)
        {
            var user = new User();

            if (dataReader !=  null && dataReader.HasRows)
            {
                user.ID = dataReader.GetString(dataReader.GetOrdinal("ID"));
                user.Name = dataReader.GetString (dataReader.GetOrdinal("Name"));
            }

            return user;
        }
    }
}
