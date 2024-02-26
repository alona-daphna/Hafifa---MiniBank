using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient

using System.Data;

namespace MiniBank
{
    internal class DBConnection
    {
        private string ConnectionString { get; set; }
        internal DBConnection(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DB");
        }

        internal IDbConnection GetConnection() => new SqlConnection(ConnectionString);
    }
}