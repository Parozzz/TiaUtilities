using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;
using TiaUtilities.Utility;

namespace TiaUtilities.Editors.ErrorReporting
{
    public class ErrorReportThread
    {
        public static ErrorReportThread ISTANCE
        {
            get {

                if (_javascriptIstance == null)
                {
                    _javascriptIstance = new ErrorReportThread();
                    _javascriptIstance.Init();
                    _javascriptIstance.Start();
                }

                return _javascriptIstance;
            }
        }
        private static ErrorReportThread? _javascriptIstance;

        public const int RUN_TIME_MS = 333;

        private readonly BackgroundWorker worker;
        private readonly Timer timer;

        private readonly List<ErrorReporter> reporterList;
        private readonly List<ErrorReporter> asyncReporterList;

        private bool init = false;

        private ErrorReportThread()
        {
            worker = new BackgroundWorker();
            timer = new Timer() { Interval = RUN_TIME_MS };

            reporterList = [];
            asyncReporterList = [];
        }

        public void Init()
        {
            if (init)
            {
                throw new InvalidOperationException("Trying to initialize ErrorReportThread twice");
            }

            init = true;

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
            Validate.NotNull(reporter);

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
            this.asyncReporterList.Clear();
            this.asyncReporterList.AddRange(reporterList.Where(r => !r.Paused));

            foreach (var reporter in this.asyncReporterList)
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
    public abstract class ErrorReporter(Func<string> textCallback, Func<bool> pausedCallback)
    {
        public delegate void CompleteEventHandler(object? sender, CompleteEventArgs args);
        public record CompleteEventArgs(List<ReportedError> ErrorList);

        public event CompleteEventHandler CompleteEvent = delegate { };

        public string Text { get => textCallback(); }
        public bool Paused { get => pausedCallback(); }

        public abstract bool Busy { get; set; }

        public abstract void ExecuteSync();
        public abstract void ExecuteAsync();

        protected void Complete(List<ReportedError> errorList)
        {
            this.CompleteEvent(this, new(errorList));
        }
    }
}
