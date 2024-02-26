using Microsoft.Data.SqlClient;
using MiniBank.Models;
using MiniBank.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Converters
{
    internal class AccountConverter
    {
        internal Account Convert(DbDataReader dataReader) 
        {
            if (dataReader != null && dataReader.HasRows)
            {
                var type = dataReader.GetInt16(dataReader.GetOrdinal("Type"));
                var account = new AccountFactory().Create(type);

                account.ID = dataReader.GetString(dataReader.GetOrdinal("ID"));
                account.Balance = dataReader.GetFloat(dataReader.GetOrdinal("Balance"));
                account.OwnerID = dataReader.GetString(dataReader.GetOrdinal("OwnerID"));

                return account;
            }

            throw new Exception("DataReader empty or not defined.");
        }
    }
}
