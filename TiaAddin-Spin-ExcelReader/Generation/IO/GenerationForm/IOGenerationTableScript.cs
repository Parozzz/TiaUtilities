using DocumentFormat.OpenXml.Vml.Office;
using Jint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationTableScript
    {
        private readonly GridHandler<IOConfiguration, IOData> gridHandler;
        private readonly IOGenerationSettings settings;

        public IOGenerationTableScript(GridHandler<IOConfiguration, IOData> gridHandler, IOGenerationSettings settings)
        {
            this.gridHandler = gridHandler;
            this.settings = settings;
        }

        public void ShowConfigForm(IWin32Window window)
        {
            var configForm = new ConfigForm("Espressione JS")
            {
                ControlWidth = 950,
                CloseOnOutsideClick = false,
                ShowControlBox = true,
            };

            configForm.AddLine("Variables: address, ioName, dbName, variable, comment");

            configForm.AddJavascriptTextBoxLine(null, height: 350)
                .ControlText(settings.JSTableScript)
                .TextChanged(str => settings.JSTableScript = str);

            configForm.AddButtonPanelLine(null)
                .AddButton("Conferma", () =>
                {
                    if (this.ParseJS())
                    {
                        configForm.Close();
                    }
                })
                .AddButton("Annulla", () => configForm.Close());

            configForm.StartShowingAtCursor();
            configForm.Init();
            configForm.Show(window);
        }

        private bool ParseJS()
        {
            if (string.IsNullOrEmpty(settings.JSTableScript))
            {
                return false;
            }

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                using (var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(500)); // Set a timeout to 4 seconds.
                    options.MaxStatements(int.MaxValue); // Set limit of 1000 executed statements.
                    options.LimitRecursion(1);
                }))
                {
                    ExecuteJS(engine, new IOData()); //Execute on empty just to execute it once in case the data is empty.

                    var changedDataDict = new Dictionary<int, IOData>();
                    foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
                    {
                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        var newIOData = ExecuteJS(engine, data);
                        if (newIOData != null)
                        {
                            changedDataDict.Add(rowIndex, newIOData);
                        }
                    }

                    this.gridHandler.ChangeMultipleRows(changedDataDict);
                }

                sw.Stop();

                Console.WriteLine("Time[ms]: " + sw.ElapsedMilliseconds + ", Time[Ticks]: " + sw.ElapsedTicks);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }

        private IOData ExecuteJS(Engine engine, IOData data)
        {
            var dataValuesDict = new Dictionary<string, IOJSVariable>()
                {
                    { "address", new IOJSVariable() { OldValue = data.Address ?? "", Column = IOData.ADDRESS } },
                    { "ioName", new IOJSVariable() { OldValue = data.IOName ?? "", Column = IOData.IO_NAME } },
                    { "dbName", new IOJSVariable() { OldValue = data.DBName ?? "", Column = IOData.DB_NAME } },
                    { "variable", new IOJSVariable() { OldValue = data.Variable ?? "", Column = IOData.VARIABLE } },
                    { "comment", new IOJSVariable() { OldValue = data.Comment ?? "", Column = IOData.COMMENT } }
                };

            foreach (var entry in dataValuesDict)
            {
                var ioJSVariable = entry.Value;
                engine.SetValue(entry.Key, ioJSVariable.OldValue);
            }

            var eval = engine.Evaluate(settings.JSTableScript);
            if (eval.IsBoolean() && !eval.AsBoolean())
            {
                return null;
            }

            var changed = false;
            foreach (var entry in dataValuesDict)
            {
                var ioJSVariable = entry.Value;

                var jsValue = engine.GetValue(entry.Key);
                ioJSVariable.NewValue = (jsValue != null && jsValue.IsString()) ? jsValue.AsString() : ioJSVariable.OldValue;

                changed |= Utils.AreStringDifferent(ioJSVariable.OldValue, ioJSVariable.NewValue);
            }

            if (changed)
            {
                var newIOData = new IOData();
                foreach (var ioJSVariable in dataValuesDict.Values)
                {
                    ioJSVariable.Column.SetValueTo(newIOData, ioJSVariable.NewValue);
                }
                return newIOData;
            }

            return null;
        }

        private class IOJSVariable
        {
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public GridDataColumn Column { get; set; }
        }
    }
}
