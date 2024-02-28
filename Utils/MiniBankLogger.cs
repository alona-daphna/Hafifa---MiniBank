using Microsoft.Extensions.Configuration;
using Serilog;

namespace MiniBank.Utils
{
    internal class MiniBankLogger
    {
        private static MiniBankLogger instance;
        internal ILogger Logger { get; set; }

        private MiniBankLogger()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        internal static MiniBankLogger GetInstance()
        {
            instance ??= new MiniBankLogger();

            return instance;
        }
    }
}