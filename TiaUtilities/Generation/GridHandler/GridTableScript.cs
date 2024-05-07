using Esprima;
using Esprima.Ast;
using FastColoredTextBoxNS;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridTableScript<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridHandler<C, T> gridHandler;
        private readonly JavascriptErrorReportThread jsErrorHandlingThread;

        private Func<string> readScriptFunc;
        private Action<string> writeScriptAction;
        private ConfigFormJavascriptTextBoxLine javascriptTextBoxLine;

        public bool Valid { get => readScriptFunc != null && writeScriptAction != null; }

        public GridTableScript(GridHandler<C, T> gridHandler, JavascriptErrorReportThread jsErrorThread)
        {
            this.gridHandler = gridHandler;
            this.jsErrorHandlingThread = jsErrorThread;

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

            configForm.AddButtonPanelLine(null)
                .AddButton("AutoFormattazione", () => this.javascriptTextBoxLine?.GetJavascriptFCTB().GetFCTB().DoAutoIndent())
                .AddButton("Esegui Script", () => this.ParseJS());
                /*.AddButton("Change Hotkeys", () =>
                {
                    var fctb = javascriptTextBoxLine?.GetJavascriptFCTB().GetFCTB();
                    if (fctb == null)
                    {
                        return;
                    }

                    var hotkeysEditorForm = new HotkeysEditorForm(fctb.HotkeysMapping);
                    if (hotkeysEditorForm.ShowDialog() == DialogResult.OK)
                    {
                        fctb.HotkeysMapping = hotkeysEditorForm.GetHotkeys();
                    }
                });*/

            this.javascriptTextBoxLine = configForm.AddJavascriptTextBoxLine(null, height: 350)
                .ControlText(readScriptFunc.Invoke())
                .TextChanged(writeScriptAction);

            configForm.Shown += (sender, args) =>
            {
                this.javascriptTextBoxLine.GetJavascriptFCTB().RegisterErrorReport(this.jsErrorHandlingThread);
            };

            configForm.FormClosed += (sender, args) =>
            {
                this.javascriptTextBoxLine.GetJavascriptFCTB().UnregisterErrorReport(this.jsErrorHandlingThread);
                this.javascriptTextBoxLine = null;
            };

            configForm.Init();
            configForm.StartShowingAtCursor();
            configForm.Show(window);
        }

        private bool ParseJS()
        {
            if (!Valid)
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
                var scriptTimer = new ScriptTimer();

                var preparedScript = Engine.PrepareScript(tableScript, strict: true);
                using (var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(500)); // Set a timeout to 500 ms.
                    options.MaxStatements(int.MaxValue);
                    options.LimitRecursion(1);
                    options.Strict = true;
                }))
                {
                    var changedDataDict = new Dictionary<int, T>();
                    foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
                    {
                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        var newIOData = ExecuteTimedJS(scriptTimer, engine, preparedScript, data);
                        if (newIOData != null)
                        {
                            changedDataDict.Add(rowIndex, newIOData);
                        }
                    }

                    this.gridHandler.ChangeMultipleRows(changedDataDict);
                }

                scriptTimer.Log(tableScript, typeof(T).Name);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }

        private T ExecuteTimedJS(ScriptTimer scriptTimer, Engine engine, Prepared<Script> script, T data)
        {
            scriptTimer.Restart();
            var ret = this.ExecuteJS(engine, script, data);
            scriptTimer.StopAndSave();

            return ret;
        }

        private T ExecuteJS(Engine engine, Prepared<Script> script, T data)
        {
            var dataValuesDict = new Dictionary<string, GridJSVariable>();
            foreach (var dataColumn in gridHandler.DataHandler.DataColumns)
            {
                var value = dataColumn.GetValueFrom<object>(data);
                if (value == null && dataColumn.PropertyInfo.PropertyType == typeof(string))
                {
                    value = "";
                }

                dataValuesDict.Add(dataColumn.ProgrammingFriendlyName, new GridJSVariable()
                {
                    OldValue = value,
                    Column = dataColumn
                });
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

                var jsValue = engine.GetValue(entry.Key); //This will not return null! It will throw an exception instead.
                ioJSVariable.NewValue = jsValue.IsString() ? jsValue.AsString() : ioJSVariable.OldValue;

                changed |= Utils.AreValuesDifferent(ioJSVariable.OldValue, ioJSVariable.NewValue);
                //I do not break the loop here because i want the NewValue property of all values to be compiled;
            }

            if (!changed)
            {
                return default;
            }

            var newData = gridHandler.DataHandler.CreateInstance();
            foreach (var ioJSVariable in dataValuesDict.Values)
            {
                ioJSVariable.Column.SetValueTo(newData, ioJSVariable.NewValue);
            }
            return newData;
        }

        private class GridJSVariable
        {
            public object OldValue { get; set; }
            public object NewValue { get; set; }
            public GridDataColumn Column { get; set; }
        }
    }
}
