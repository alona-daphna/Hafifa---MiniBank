using Microsoft.Extensions.Configuration;

namespace MiniBank.Logs
{
    internal class Logger
    {
        private string FilePath {  get; set; }
        public Logger()
        {
            FilePath = new ConfigurationManager().AddJsonFile("appsettings.json").Build().GetSection("Logging:File").Value;
            WriteCsvHeaders();
        }

        internal void Info(string message) => WriteToLogFile(message, DateTime.Now, "Info");

        internal void Error(string message) => WriteToLogFile(message, DateTime.Now, "Error");

        private void WriteToLogFile(string message, DateTime dateTime, string severity)
        {
            using var sw = new StreamWriter(FilePath, true);
            sw.WriteLine($"{severity},{dateTime},{message}");
        }

        private void WriteCsvHeaders()
        { 
            if (!File.Exists(FilePath))
            {
                using var sw = new StreamWriter(FilePath);
                sw.WriteLine("Severity,Date,Message");
            }
        }
    }
}
