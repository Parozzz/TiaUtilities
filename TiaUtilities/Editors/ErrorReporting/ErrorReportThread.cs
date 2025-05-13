using System.ComponentModel;
using TiaUtilities.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaUtilities.Editors.ErrorReporting
{
    public class ErrorReportThread
    {
        public const int RUN_TIME_MS = 333;

        private readonly BackgroundWorker worker;
        private readonly Timer timer;

        private readonly List<ErrorReporter> reporterList;
        private readonly List<ErrorReporter> asyncReporterList;

        public ErrorReportThread()
        {
            worker = new BackgroundWorker();
            timer = new Timer() { Interval = RUN_TIME_MS };

            reporterList = [];
            asyncReporterList = [];
        }

        public void Init()
        {
            worker.DoWork += (sender, args) => ExecuteAsync();
            timer.Tick += (sender, args) =>
            {
                try
                {
                    var busy = QueryBusy();
                    if (!worker.IsBusy && !busy)
                    {
                        ExecuteSync();
                        worker.RunWorkerAsync();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex, silent: true);
                }
            };
        }

        public void AddReporter(ErrorReporter reporter)
        {
            if (!reporterList.Contains(reporter))
            {
                reporterList.Add(reporter);
            }
        }

        public void RemoveReporter(ErrorReporter reporter)
        {
            reporterList.Remove(reporter);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void ExecuteSync()
        {
            asyncReporterList.Clear();
            asyncReporterList.AddRange(reporterList);

            foreach (var reporter in reporterList)
            {
                reporter.Busy = true;
                reporter.ExecuteSync();
            }
        }

        private void ExecuteAsync()
        {
            foreach (var reporter in asyncReporterList)
            {
                try
                {
                    reporter.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            }

            asyncReporterList.Clear();
        }

        private bool QueryBusy() => reporterList.Any(reporter => reporter.Busy);

    }
    public record ReportedError(long Line, long Column, string? Description);
    public abstract class ErrorReporter
    {
        public delegate void CompleteEventHandler(object? sender, CompleteEventArgs args);
        public record CompleteEventArgs(List<ReportedError> ErrorList);

        public event CompleteEventHandler CompleteEvent = delegate { };

        public abstract bool Busy { get; set; }

        public abstract void ExecuteSync();
        public abstract void ExecuteAsync();

        protected void Complete(List<ReportedError> errorList)
        {
            this.CompleteEvent(this, new(errorList));
        }
    }
}
