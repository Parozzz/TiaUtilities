using Acornima.Ast;
using Jint;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Lines;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler.Events;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScript<T>(GridHandler<T> gridHandler, JavascriptErrorReportThread jsErrorThread, GridScriptContainer scriptContainer) where T : IGridData
    {
        private record TabPageScriptRecord(GridScriptContainer.ScriptInfo ScriptInfo, JavascriptEditor JavascriptEditor);

        private const string ENGINE_LOG_FUNCTION = "log";
        private const string ENGINE_ROW_VARIABLE = "row";

        private ConfigTextBoxLine? logTextBoxLine;
        private ConfigJSONLine? jsonLine;

        private TabPageScriptRecord? record;

        private int singleExecutionLastRow = -1;

        public void ShowConfigForm(IWin32Window window)
        {
            var configForm = new ConfigForm("Espressione JS")
            {
                ControlWidth = 750,
                CloseOnOutsideClick = false,
                ShowControlBox = true,
                CloseOnEnter = false,
            };

            GridScriptShowVariableEventArgs showVariableEventArgs = new();

            var variableList = showVariableEventArgs.VariableList;
            variableList.AddRange(gridHandler.DataHandler.DataColumns.Select(c => c.ProgrammingFriendlyName + " [" + c.PropertyInfo.PropertyType.Name + "]"));
            variableList.Add(ENGINE_ROW_VARIABLE + " [int]");

            gridHandler.Events.ScriptShowVariableEvent(gridHandler.DataGridView, showVariableEventArgs);

            var mainGroup = configForm.Init();
            mainGroup.AddLabel()
                .Label("Variables: " + showVariableEventArgs.VariableList.Aggregate((a, b) => a + ", " + b))
                .LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular));

            var debugGroup = mainGroup.AddVerticalGroup().Height(150).SplitterDistance(95);
            logTextBoxLine = debugGroup.AddTextBox()
                .Label("Log > " + ENGINE_LOG_FUNCTION + "(string)").LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop()
                .Readonly().Multiline();

            jsonLine = debugGroup.AddJSON().Readonly()
                .Label("Context Json").LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular)).LabelOnTop();

            var tabLine = mainGroup.AddInteractableTab()
                .Height(500)
                .Label("Espressione JS").LabelOnTop()
                .RequireConfirmationBeforeClosing()
                .TabAdded(tabPage =>
                {
                    var scriptInfo = scriptContainer.AddScript();
                    AddJavascriptControl(tabPage, scriptInfo);
                }).TabRemoved(tabPage =>
                {
                    if (tabPage.Tag is not TabPageScriptRecord record)
                    {
                        return;
                    }

                    scriptContainer.RemoveScript(record.ScriptInfo);
                }).TabChanged(tabPage =>
                {
                    this.record?.JavascriptEditor.UnregisterErrorReport(jsErrorThread);
                    this.record = null;

                    if (tabPage?.Tag is not TabPageScriptRecord record)
                    {
                        return;
                    }

                    this.record = record;
                    this.record.JavascriptEditor.RegisterErrorReport(jsErrorThread);
                }).TabNameUserChanged((tabPage, newName) =>
                {
                    if (tabPage.Tag is not TabPageScriptRecord record)
                    {
                        return;
                    }

                    record.ScriptInfo.Name = newName;
                });

            foreach (var scriptInfo in scriptContainer.Scripts)
            {
                var tabPage = tabLine.AddTabPage();
                AddJavascriptControl(tabPage, scriptInfo);
            }

            mainGroup.AddButtonPanel()
                 .AddButton("AutoFormattazione", () => record?.JavascriptEditor.GetTextBox().DoAutoIndent())
                 .AddButton("Esegui", () => ParseJS())
                 .AddButton("Esegui per Riga", () => ParseJS(singleExecution: true))
                 .AddButton("Esegui in DebugMode", () => ParseJS(debugRun: true));

            configForm.FormClosed += (sender, args) =>
            {
                record?.JavascriptEditor.UnregisterErrorReport(jsErrorThread);
                record = null;
            };

            configForm.StartShowingAtCursor();
            configForm.Show(window);
        }

        private static void AddJavascriptControl(TabPage tabPage, GridScriptContainer.ScriptInfo scriptInfo)
        {
            var jsTabMainGroup = new ConfigGroup(null);

            JavascriptEditor javascriptEditor = new();
            javascriptEditor.InitControl();

            var fctb = javascriptEditor.GetTextBox();
            fctb.AutoSize = true;
            fctb.Dock = DockStyle.Fill;
            fctb.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            tabPage.Text = scriptInfo.Name;
            fctb.Text = scriptInfo.Text;
            fctb.TextChanged += (sender, args) =>
            {
                scriptInfo.Text = javascriptEditor.GetTextBox().Text;
            };

            tabPage.Controls.Add(fctb);

            tabPage.Tag = new TabPageScriptRecord(scriptInfo, javascriptEditor);
        }

        private void Log(string logString)
        {
            if (logTextBoxLine != null)
            {
                var control = logTextBoxLine.GetControl();
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
                singleExecutionLastRow = -1;
            }

            if (record == null)
            {
                return false;
            }

            try
            {
                var tableScript = record.JavascriptEditor.GetTextBox().Text;

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
                gridHandler.Events.ScriptAddVariablesEvent(gridHandler.DataGridView, addVariablesArgs);
                foreach (var entry in addVariablesArgs.VariableDict)
                {
                    engine.SetValue(entry.Key, entry.Value);
                }

                if (debugRun)
                {
                    var newData = gridHandler.DataHandler.CreateInstance();
                    engine.SetValue(ENGINE_ROW_VARIABLE, 0);
                    ExecuteTimedJS(scriptTimer, engine, preparedScript, newData);
                }
                else if (!singleExecution)
                {
                    singleExecutionLastRow = -1;

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

                    gridHandler.ChangeMultipleRows(changedDataDict);
                }
                else
                {
                    var dataEnumerable = gridHandler.DataSource.GetNotEmptyDataDict().Where(e => e.Value > singleExecutionLastRow);

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
                            gridHandler.ChangeRow(rowIndex, newIOData);
                        }

                        singleExecutionLastRow = rowIndex;
                    }

                    if (!anyFound || anyFound && dataEnumerable.Count() == 1)
                    {
                        singleExecutionLastRow = -1;
                    }
                }

                UpdateJsonContext(engine);

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
            var ret = ExecuteJS(engine, script, data);
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
