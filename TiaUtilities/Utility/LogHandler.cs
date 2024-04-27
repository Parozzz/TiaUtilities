using Irony;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Utility
{
    public class LogHandler
    {
        private class Log
        {
            public string Message { get; set; }
            public string FileName { get; set; }
        }

        public static readonly LogHandler INSTANCE = new LogHandler();

        private readonly ConcurrentBag<Log> messages;
        private LogHandler()
        {
            messages = new ConcurrentBag<Log>();
        }

        private string GetFilePath(string fileName)
        {
            var dateTime = DateTime.Now;

            var year = dateTime.ToString("yyyy");
            var month = dateTime.ToString("MM");
            return Directory.GetCurrentDirectory() + "\\logs\\" + year + "\\" + month + "_" + fileName + ".log";
        }


        public void AddException(string message)
        {
            AddMessage(message, "exceptions");
        }

        public void AddMessage(string message, string fileName) 
        {
            var timeString = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss fff") + "[ms]";

            var timeHeader = "============================================= ^^^^^ " + timeString + " ^^^^^ =============================================";
            messages.Add(new Log()
            {
                Message = message + '\n' + timeHeader + '\n',
                FileName = fileName,
            });
        }

        public void WriteToFile()
        {
            if (!messages.TryTake(out Log log))
            {
                return;
            }

            var filePath = this.GetFilePath(log.FileName);
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            File.AppendAllText(filePath, log.Message);
        }
    }
}
