using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

using System.Data;

namespace MiniBank
{
    internal class DBConnection
    {
        private string ConnectionString { get; set; }

        public DBConnection()
        {
            ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DB") ?? throw new Exception("Can't find connection string.");
        }

        internal SqlConnection GetConnection() => new(ConnectionString);
    }
}