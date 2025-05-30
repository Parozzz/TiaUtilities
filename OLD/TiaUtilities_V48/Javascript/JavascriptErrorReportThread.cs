﻿using Esprima;
using Esprima.Utils;
using Jint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Javascript
{
    public class JavascriptErrorReportThread
    {
        public const int RUN_TIME_MS = 333;

        public class JSError
        {
            public int Line { get; set; }
            public int Column { get; set; }
            public string Description { get; set; }
        }

        public class JSScriptReport
        {
            public Func<string> GetScriptSyncFunc;
            public Action DoneAction;
            public volatile string Script;
            public JSError JSError;
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

            this.scriptReportList = new List<JSScriptReport>();
            this.asyncScriptReportList = new List<JSScriptReport>();
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
            var scriptReport = new JSScriptReport()
            {
                GetScriptSyncFunc = getScriptSyncFunc,
                DoneAction = doneAction
            };

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
                    var parsingOptions = new ParserOptions()
                    {
                        AllowReturnOutsideFunction = true
                    };
                    var type = parsingOptions.GetType();
                    type.GetProperty(nameof(ParserOptions.Tokens)).SetValue(parsingOptions, true);
                    type.GetProperty(nameof(ParserOptions.Tolerant)).SetValue(parsingOptions, true);
                    type.GetProperty(nameof(ParserOptions.Comments)).SetValue(parsingOptions, true);

                    var program = new JavaScriptParser(parsingOptions).ParseScript(scriptReport.Script, strict: true);
                }
                catch (ParserException parseEx)
                {
                    var error = parseEx.Error;
                     scriptReport.JSError = new JSError()
                    {
                        Line = error.Position.Line,
                        Column = error.Position.Column,
                        Description = error.Description
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JavascriptErrorHandlingThread exception.\n" + ex.ToString());
                }
            }

            Busy = false;
        }

    }
}
