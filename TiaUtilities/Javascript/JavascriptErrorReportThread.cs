using Esprima;
using Irony.Parsing;
using Jint;
using System.ComponentModel;
using TiaXmlReader.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TiaXmlReader.Javascript.JavascriptErrorReportThread;
using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader.Javascript
{
    public class JavascriptErrorReportThread
    {
        public const int RUN_TIME_MS = 333;

        public class JSError(int line, int column, string description)
        {
            public int Line { get; init; } = line;
            public int Column { get; init; } = column;
            public string Description { get; init; } = description;
        }

        public class JSScriptReport(Func<string> getScriptSyncFunc, Action doneAction)
        {
            public Func<string> GetScriptSyncFunc { get; init; } = getScriptSyncFunc;
            public Action DoneAction { get; init; } = doneAction;
            public volatile string? Script;
            public JSError? JSError;
        }

        private readonly BackgroundWorker worker;
        private readonly Timer timer;

        private readonly List<JSScriptReport> scriptReportList;
        private readonly List<JSScriptReport> asyncScriptReportList;

        public volatile bool Busy;

        public JavascriptErrorReportThread()
        {
            this.worker = new BackgroundWorker();
            this.timer = new Timer() { Interval = RUN_TIME_MS };

            this.scriptReportList = [];
            this.asyncScriptReportList = [];
        }

        public void Init()
        {
            worker.DoWork += (sender, args) => this.ExecuteAsync();
            timer.Tick += (sender, args) =>
            {
                try
                {
                    if (!worker.IsBusy && !this.Busy)
                    {
                        this.SyncReportDone(); //From previous run! If first, the modulo should handle it.

                        this.Busy = true;
                        this.SyncParseReportList();
                        worker.RunWorkerAsync();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex, silent: true);
                }
            };
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public JSScriptReport RegisterScript(Func<string> getScriptSyncFunc, Action doneAction)
        {
            var scriptReport = new JSScriptReport(getScriptSyncFunc, doneAction);
            this.scriptReportList.Add(scriptReport);
            return scriptReport;
        }

        public void RemoveScript(JSScriptReport jSErrorHandlerResult)
        {
            this.scriptReportList.Remove(jSErrorHandlerResult);
        }

        public void SyncParseReportList()
        {
            this.asyncScriptReportList.Clear();

            foreach (var scriptReport in this.scriptReportList)
            {
                var script = scriptReport.GetScriptSyncFunc.Invoke();
                if (string.IsNullOrEmpty(script))
                {
                    continue;
                }

                scriptReport.Script = script;
                this.asyncScriptReportList.Add(scriptReport);
            }
        }

        public void SyncReportDone()
        {
            foreach (var result in asyncScriptReportList)
            {
                result.DoneAction?.Invoke();
            }
        }

        public void ExecuteAsync()
        {
            foreach (var scriptReport in asyncScriptReportList)
            {
                scriptReport.JSError = null;

                try
                {
                    var errorHandler = new CollectingErrorHandler();
                    var parsingOptions = new ParserOptions()
                    {
                        AllowReturnOutsideFunction = true,
                        Tokens = true,
                        Tolerant = false,
                        Comments = true,
                        ErrorHandler = errorHandler
                    };

                    var program = new JavaScriptParser(parsingOptions).ParseScript(scriptReport.Script, strict: true);
                    foreach (var error in errorHandler.Errors)
                    {
                        scriptReport.JSError = this.CreateError(error);
                        break;
                    }
                }
                catch (ParserException parseEx)
                {
                    scriptReport.JSError = this.CreateError(parseEx.Error);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JavascriptErrorHandlingThread exception.\n" + ex.ToString());
                }
            }

            Busy = false;
        }

        private JSError? CreateError(ParseError? parseError)
        {
            if (parseError == null)
            {
                return null;
            }

            var pos = parseError.Position;
            return new JSError(pos.Line, pos.Column, parseError.Description);
        }

    }
}
