using DocumentFormat.OpenXml.Vml.Office;
using Esprima;
using Esprima.Ast;
using FastColoredTextBoxNS;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.GridHandler.Events;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridScript<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridHandler<C, T> gridHandler;
        private readonly JavascriptErrorReportThread jsErrorHandlingThread;

        private Func<string> readScriptFunc;
        private Action<string> writeScriptAction;

        private ConfigFormTextBoxLine logTextBoxLine;
        private ConfigFormJavascriptLine? javascriptLine;
        private ConfigFormJSONLine jsonLine;

        public bool Valid { get => readScriptFunc != null && writeScriptAction != null; }

        public GridScript(GridHandler<C, T> gridHandler, JavascriptErrorReportThread jsErrorThread)
        {
            this.gridHandler = gridHandler;
            this.jsErrorHandlingThread = jsErrorThread;
        }

        public GridScript<C, T> SetReadScriptFunc(Func<string> getJSTableScriptFunc)
        {
            this.readScriptFunc = getJSTableScriptFunc;
            return this;
        }

        public GridScript<C, T> SetWriteScriptAction(Action<string> setJSTableScriptAction)
        {
            this.writeScriptAction = setJSTableScriptAction;
            return this;
        }

        public void ShowConfigForm(IWin32Window window)
        {
            var configForm = new ConfigForm("Espressione JS")
            {
                ControlWidth = 750,
                CloseOnOutsideClick = false,
                ShowControlBox = true,
                CloseOnEnter = false,
            };

            var showVariableEventArgs = new GridScriptShowVariableEventArgs();

            var variableList = showVariableEventArgs.VariableList;
            variableList.AddRange(gridHandler.DataHandler.DataColumns.Select(c => c.ProgrammingFriendlyName + " [" + c.PropertyInfo.PropertyType.Name + "]"));
            variableList.Add("row [int]");

            gridHandler.Events.ScriptShowVariableEvent(this.gridHandler, showVariableEventArgs);

            var mainGroup = configForm.Init();
            mainGroup.AddLine(ConfigFormLineTypes.LABEL)
                .LabelText("Variables: " + showVariableEventArgs.VariableList.Aggregate((a, b) => a + ", " + b))
                .LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular));

            this.logTextBoxLine = mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).Height(100)
                .LabelText("Log > log(string)").LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop()
                .Readonly().Multiline();

            this.javascriptLine = mainGroup.AddLine(ConfigFormLineTypes.JAVASCRIPT).Height(350)
                .ControlText(readScriptFunc.Invoke())
                .TextChanged(writeScriptAction);

            this.jsonLine = mainGroup.AddLine(ConfigFormLineTypes.JSON).Height(150)
                .LabelText("Context Json").LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop();

            mainGroup.AddLine(ConfigFormLineTypes.BUTTON_PANEL)
                 .AddButton("AutoFormattazione", () => this.javascriptLine?.GetJavascriptFCTB().GetFCTB().DoAutoIndent())
                 .AddButton("Esegui Script", () => this.ParseJS());

            configForm.Shown += (sender, args) =>
            {
                this.javascriptLine.GetJavascriptFCTB().RegisterErrorReport(this.jsErrorHandlingThread);
            };

            configForm.FormClosed += (sender, args) =>
            {
                this.javascriptLine.GetJavascriptFCTB().UnregisterErrorReport(this.jsErrorHandlingThread);
                this.javascriptLine = null;
            };

            configForm.Init();
            configForm.StartShowingAtCursor();
            configForm.Show(window);
        }

        private void Log(string logString)
        {
            if (this.logTextBoxLine != null)
            {
                var control = this.logTextBoxLine.GetControl();
                if (control != null)
                {
                    var timeString = DateTime.Now.ToString("HH:mm:ss fff") + "ms";

                    var text = control.Text;
                    text += timeString + ") " + logString + "\r\n";
                    control.Text = text;
                }
            }
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
                    engine.SetValue("log", new Action<string>(Log));

                    var addVariablesArgs = new GridScriptAddVariableEventArgs();
                    this.gridHandler.Events.ScriptAddVariablesEvent(gridHandler, addVariablesArgs);
                    foreach (var entry in addVariablesArgs.VariableDict)
                    {
                        engine.SetValue(entry.Key, entry.Value);
                    }

                    var changedDataDict = new Dictionary<int, T>();
                    foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
                    {
                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        engine.SetValue("row", rowIndex);

                        var newIOData = ExecuteTimedJS(scriptTimer, engine, preparedScript, data);
                        if (newIOData != null)
                        {
                            changedDataDict.Add(rowIndex, newIOData);
                        }
                    }

                    var contextJsonJSValue = engine.Evaluate(@"JSON.stringify(this, null, 2);");
                    if (contextJsonJSValue.IsString())
                    {
                        jsonLine.GetControl().Text = contextJsonJSValue.AsString();
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

        private T? ExecuteJS(Engine engine, Prepared<Script> script, T data)
        {
            var dataValuesDict = new Dictionary<string, GridJSVariable>();
            foreach (var dataColumn in gridHandler.DataHandler.DataColumns)
            {
                var value = dataColumn.GetValueFrom<object>(data);
                if (value == null && dataColumn.PropertyInfo.PropertyType == typeof(string))
                {
                    value = "";
                }

                dataValuesDict.Add(dataColumn.ProgrammingFriendlyName, new GridJSVariable(dataColumn) { OldValue = value });
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
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public GridDataColumn Column { get; init; }

            public GridJSVariable(GridDataColumn column)
            {
                this.Column = column;
            }
        }
    }
}
