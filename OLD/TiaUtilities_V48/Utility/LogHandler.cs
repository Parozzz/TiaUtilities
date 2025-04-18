﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

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

        private readonly BackgroundWorker worker;
        private readonly Timer timer;

        private readonly ConcurrentBag<Log> messages;

        private bool init;

        private LogHandler()
        {
            this.worker = new BackgroundWorker();
            this.timer = new Timer() { Interval = 1000 };

            messages = new ConcurrentBag<Log>();
        }

        public void Init()
        {
            if(init)
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
            messages.Add(new Log()
            {
                Message = message + '\n' + timeHeader + '\n',
                FileName = fileName,
            });
        }

        public void WriteToFile()
        {
            try
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
            } catch { }
        }
    }
}
