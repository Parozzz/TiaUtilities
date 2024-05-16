using DocumentFormat.OpenXml.Vml.Office;
using Esprima.Ast;
using InfoBox;
using Jint;
using Jint.Native;
using Jint.Native.ShadowRealm;
using TiaUtilities.Generation.Configuration.Lines;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler.Events;
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
        private const string ENGINE_LOG_FUNCTION = "log";
        private const string ENGINE_ROW_VARIABLE = "row";

        private readonly GridHandler<C, T> gridHandler;
        private readonly JavascriptErrorReportThread jsErrorHandlingThread;

        private ConfigTextBoxLine? logTextBoxLine;
        private ConfigJavascriptLine? javascriptLine;
        private ConfigJSONLine? jsonLine;

        private int singleExecutionLastRow = -1;

        public GridScript(GridHandler<C, T> gridHandler, JavascriptErrorReportThread jsErrorThread)
        {
            this.gridHandler = gridHandler;
            this.jsErrorHandlingThread = jsErrorThread;
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
            variableList.Add(ENGINE_ROW_VARIABLE + " [int]");

            gridHandler.Events.ScriptShowVariableEvent(showVariableEventArgs);

            var mainGroup = configForm.Init();
            mainGroup.AddLabel()
                .Label("Variables: " + showVariableEventArgs.VariableList.Aggregate((a, b) => a + ", " + b))
                .LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular));

            var debugGroup = mainGroup.AddVerticalGroup().Height(150).SplitterDistance(95);
            this.logTextBoxLine = debugGroup.AddTextBox()
                .Label("Log > " + ENGINE_LOG_FUNCTION + "(string)").LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop()
                .Readonly().Multiline();
            this.jsonLine = debugGroup.AddJSON().Readonly()
                .Label("Context Json").LabelFont(ConfigForm.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop();


            var loadScriptArgs = new GridScriptEventArgs();
            this.gridHandler.Events.ScriptLoadEvent(loadScriptArgs);
            this.javascriptLine = mainGroup.AddJavascript().Height(350)
                .ControlText(loadScriptArgs.Script)
                .TextChanged(str =>
                {
                    var scriptChangedArgs = new GridScriptEventArgs() { Script = str };
                    this.gridHandler.Events.ScriptChangedEvent(scriptChangedArgs);
                });


            mainGroup.AddButtonPanel()
                 .AddButton("AutoFormattazione", () => this.javascriptLine?.GetJavascriptFCTB().GetFCTB().DoAutoIndent())
                 .AddButton("Esegui", () => this.ParseJS())
                 .AddButton("Esegui per Riga", () => this.ParseJS(singleExecution: true))
                 .AddButton("Esegui in DebugMode", () => this.ParseJS(debugRun: true));

            configForm.Shown += (sender, args) =>
            {
                this.javascriptLine.GetJavascriptFCTB().RegisterErrorReport(this.jsErrorHandlingThread);

                this.ParseJS(ignoreLog: true, debugRun: true);
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

        private bool ParseJS(bool singleExecution = false, bool ignoreLog = false, bool debugRun = false)
        {
            if (!singleExecution)
            {
                this.singleExecutionLastRow = -1;
            }

            if (javascriptLine == null)
            {
                return false;
            }

            try
            {
                var tableScript = this.javascriptLine.GetControl().Text;

                var scriptTimer = new ScriptTimer();

                var preparedScript = Engine.PrepareScript(tableScript, strict: true);

                using var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(1000)); // Set a timeout to 500 ms.
                    options.MaxStatements(int.MaxValue);
                    options.LimitRecursion(1);
                    options.Strict = true;
                });

                Action<string> logAction = ignoreLog ? str => { } : new Action<string>(Log);
                engine.SetValue(ENGINE_LOG_FUNCTION, logAction);

                var addVariablesArgs = new GridScriptAddVariableEventArgs();
                this.gridHandler.Events.ScriptAddVariablesEvent(addVariablesArgs);
                foreach (var entry in addVariablesArgs.VariableDict)
                {
                    engine.SetValue(entry.Key, entry.Value);
                }

                if (debugRun)
                {
                    var newData = this.gridHandler.DataHandler.CreateInstance();
                    engine.SetValue(ENGINE_ROW_VARIABLE, 0);
                    ExecuteTimedJS(scriptTimer, engine, preparedScript, newData);
                }
                else if(!singleExecution)
                {
                    this.singleExecutionLastRow = - 1;

                    var changedDataDict = new Dictionary<int, T>();

                    var dataDict = gridHandler.DataSource.GetNotEmptyDataDict();
                    foreach (var entry in dataDict)
                    {
                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        engine.SetValue(ENGINE_ROW_VARIABLE, rowIndex);

                        var newIOData = ExecuteTimedJS(scriptTimer, engine, preparedScript, data);
                        if (newIOData != null)
                        {
                            changedDataDict.Add(rowIndex, newIOData);
                        }
                    }

                    this.gridHandler.ChangeMultipleRows(changedDataDict);
                }
                else
                {
                    var dataEnumerable = gridHandler.DataSource.GetNotEmptyDataDict().Where(e => e.Value > this.singleExecutionLastRow);

                    var anyFound = dataEnumerable.Any();
                    if (anyFound)
                    {
                        var entry = dataEnumerable.First();

                        var rowIndex = entry.Value;
                        var data = entry.Key;

                        engine.SetValue(ENGINE_ROW_VARIABLE, rowIndex);

                        var newIOData = ExecuteTimedJS(scriptTimer, engine, preparedScript, data);
                        if (newIOData != null)
                        {
                            this.gridHandler.ChangeRow(rowIndex, newIOData);
                        }

                        this.singleExecutionLastRow = rowIndex;
                    }
                    
                    if(!anyFound || (anyFound && dataEnumerable.Count() == 1))
                    {
                        this.singleExecutionLastRow = -1;
                    }
                }

                this.UpdateJsonContext(engine);

                scriptTimer.Log(tableScript, typeof(T).Name);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex, silent: ignoreLog);
            }

            return false;
        }

        private T? ExecuteTimedJS(ScriptTimer scriptTimer, Engine engine, Prepared<Script> script, T data)
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

                changed |= Utils.AreDifferentObject(ioJSVariable.OldValue, ioJSVariable.NewValue);
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

        private void UpdateJsonContext(Engine engine)
        {
            if (jsonLine == null)
            {
                return;
            }

            var contextJsonJSValue = engine.Evaluate(@"JSON.stringify(this, null, 2);");
            if (contextJsonJSValue.IsString())
            {
                jsonLine.GetControl().Text = contextJsonJSValue.AsString();
            }
        }

        private class GridJSVariable(GridDataColumn column)
        {
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public GridDataColumn Column { get; init; } = column;
        }
    }
}
