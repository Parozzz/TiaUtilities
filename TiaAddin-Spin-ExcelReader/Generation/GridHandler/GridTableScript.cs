using DocumentFormat.OpenXml.Vml.Office;
using Jint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridTableScript<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridHandler<C, T> gridHandler;
        private Func<string> readScriptFunc;
        private Action<string> writeScriptAction;
        public bool Valid { get => readScriptFunc != null && writeScriptAction != null; }

        public GridTableScript(GridHandler<C, T> gridHandler)
        {
            this.gridHandler = gridHandler;
        }

        public GridTableScript<C, T> SetReadScriptFunc(Func<string> getJSTableScriptFunc)
        {
            this.readScriptFunc = getJSTableScriptFunc;
            return this;
        }

        public GridTableScript<C, T> SetWriteScriptAction(Action<string> setJSTableScriptAction)
        {
            this.writeScriptAction = setJSTableScriptAction;
            return this;
        }

        public void ShowConfigForm(IWin32Window window)
        {
            var configForm = new ConfigForm("Espressione JS")
            {
                ControlWidth = 950,
                CloseOnOutsideClick = false,
                ShowControlBox = true,
            };

            var text = "Variables: " + gridHandler.DataHandler.DataColumns.Select(c => c.ProgrammingFriendlyName + " [" + c.PropertyInfo.PropertyType.Name + "]").Aggregate((a, b) => a + ", " + b);
            configForm.AddLine(text);

            configForm.AddJavascriptTextBoxLine(null, height: 350)
                .ControlText(readScriptFunc.Invoke())
                .TextChanged(writeScriptAction);

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
            if(!Valid)
            {
                return false;
            }


            var tableScript = this.readScriptFunc.Invoke();
            if (string.IsNullOrEmpty(tableScript))
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
                    ExecuteJS(engine, tableScript, gridHandler.DataHandler.CreateInstance()); //Execute on empty just to execute it once in case the data is empty.

                    var changedDataDict = new Dictionary<int, T>();
                    foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
                    {
                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        var newIOData = ExecuteJS(engine, tableScript, data);
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

        private T ExecuteJS(Engine engine, string script, T data)
        {
            var dataValuesDict = new Dictionary<string, GridJSVariable>();
            foreach (var dataColumn in gridHandler.DataHandler.DataColumns)
            {
                var value = dataColumn.GetValueFrom<object>(data);
                if(value == null)
                {
                    if (dataColumn.PropertyInfo.PropertyType == typeof(string))
                    {
                        value = "";
                    }
                }

                var jsVariable = new GridJSVariable()
                {
                    OldValue = value,
                    Column = dataColumn
                };
                dataValuesDict.Add(dataColumn.ProgrammingFriendlyName, jsVariable);
            }

            foreach (var entry in dataValuesDict)
            {
                var ioJSVariable = entry.Value;
                engine.SetValue(entry.Key, ioJSVariable.OldValue);
            }

            var eval = engine.Evaluate(script);
            if (eval.IsBoolean() && !eval.AsBoolean())
            {
                return default;
            }

            var changed = false;
            foreach (var entry in dataValuesDict)
            {
                var ioJSVariable = entry.Value;

                var jsValue = engine.GetValue(entry.Key);
                ioJSVariable.NewValue = (jsValue != null && jsValue.IsString()) ? jsValue.AsString() : ioJSVariable.OldValue;

                changed |= Utils.AreValuesDifferent(ioJSVariable.OldValue, ioJSVariable.NewValue);
            }

            if (changed)
            {
                var newData = gridHandler.DataHandler.CreateInstance();
                foreach (var ioJSVariable in dataValuesDict.Values)
                {
                    ioJSVariable.Column.SetValueTo(newData, ioJSVariable.NewValue);
                }
                return newData;
            }

            return default;
        }

        private class GridJSVariable
        {
            public object OldValue { get; set; }
            public object NewValue { get; set; }
            public GridDataColumn Column { get; set; }
        }
    }
}
