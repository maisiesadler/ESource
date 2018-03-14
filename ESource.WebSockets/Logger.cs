using System;
using System.IO;

namespace ESource.WebSockets
{
    public interface ILogger
    {
        void Log(string message);
        void LogException(string message, Exception exception);
    }

    public class FileLogger : ILogger
    {
        string file = "../esource.log";
        public FileLogger()
        {
            if (!File.Exists(file))
            {
                File.Create(file);
            }
        }
        public void Log(string message)
        {
            WriteToFile(new[] { message });
        }

        public void LogException(string message, Exception exception)
        {
            string[] exceptionMsg = new [] { message, exception.Message, exception.InnerException?.Message, exception.StackTrace };
            WriteToFile(exceptionMsg);
        }

        private void WriteToFile(string[] message)
        {
            File.AppendAllText(file, DateTime.Now + "-");
            File.AppendAllLines(file, message);
        }
    }
}
