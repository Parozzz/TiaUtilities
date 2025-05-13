using System.Collections.Concurrent;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader.Utility
{
    public class LogHandler
    {
        private record Log(string Message, string FileName);

        public static readonly LogHandler INSTANCE = new();

        private readonly BackgroundWorker worker;
        private readonly Timer timer;

        private readonly ConcurrentBag<Log> messages;

        private bool init;

        private LogHandler()
        {
            this.worker = new BackgroundWorker();
            this.timer = new Timer() { Interval = 1000 };

            messages = [];
        }

        public void Init()
        {
            if (init)
            {
                return;
            }

            this.worker.DoWork += (sender, args) => this.WriteToFile();
            timer.Tick += (sender, args) =>
            {
                if (!this.worker.IsBusy)
                {
                    this.worker.RunWorkerAsync();
                }
            };

            init = true;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
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
            messages.Add(new Log(Message: $"{message}\n{timeHeader}\n", FileName: fileName));
        }

        public void WriteToFile()
        {
            try
            {
                if (!messages.TryTake(out Log? log) || log == null)
                {
                    return;
                }

                var filePath = this.GetFilePath(log.FileName);
                if (!File.Exists(filePath) && Path.GetDirectoryName(filePath) is string directoryName)
                {
                    Directory.CreateDirectory(directoryName);
                }

                File.AppendAllText(filePath, log.Message);
            }
            catch { }
        }
    }
}
