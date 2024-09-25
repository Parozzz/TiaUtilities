using Acornima.Ast;
using Jint;
using System.Collections.ObjectModel;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Lines;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScript : ICleanable, ISaveable<GridScriptSave>
    {
        private record TabPageScriptRecord(ScriptInfo ScriptInfo, JavascriptEditor JavascriptEditor);

        private const string ENGINE_LOG_FUNCTION = "log";
        private const string ENGINE_ROW_VARIABLE = "row";

        private readonly JavascriptErrorReportThread jsErrorThread;

        private readonly List<ScriptInfo> scriptInfoList;
        private readonly ObservableCollection<GridScriptVariable> gridVariableList;
        private readonly ObservableCollection<GridScriptVariable> customVariableList;

        private readonly ConfigForm configForm;
        private readonly ConfigLabelLine labelTopLine;
        private readonly ConfigTextBoxLine logTextBoxLine;
        private readonly ConfigJSONLine jsonLine;
        private readonly ConfigInteractableTabLine tabLine;

        private TabPageScriptRecord? record;
        private GridScriptHandlerBind? bind;

        public GridScript(JavascriptErrorReportThread jsErrorThread)
        {
            this.jsErrorThread = jsErrorThread;

            this.scriptInfoList = [];
            this.gridVariableList = [];
            this.customVariableList = [];

            this.configForm = new ConfigForm(Locale.GRID_SCRIPT_JS_EXPRESSION)
            {
                ControlWidth = 900,
                CloseOnOutsideClick = false,
                ShowControlBox = true,
                CloseOnEnter = false,
            };

            var variableNamesText = this.CreateVariableNamesText();

            var mainGroup = this.configForm.Init();
            this.labelTopLine = mainGroup.AddLabel()
                .Label($"{Locale.GRID_SCRIPT_VARIABLES} {variableNamesText}")
                .LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular));

            var debugGroup = mainGroup.AddVerticalGroup().Height(150).SplitterDistance(95);
            this.logTextBoxLine = debugGroup.AddTextBox()
                .Label($"Log > {ENGINE_LOG_FUNCTION} [string]")
                .LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular))
                .LabelOnTop()
                .Readonly().Multiline();

            this.jsonLine = debugGroup.AddJSON().Readonly()
                .Label(Locale.GRID_SCRIPT_JSON_CONTEXT)
                .LabelFont(ConfigStyle.LABEL_FONT.Copy(9f, FontStyle.Regular))
                .LabelOnTop();

            this.tabLine = mainGroup.AddInteractableTab()
                .Height(650)
                .LabelOnTop()
                .RequireConfirmationBeforeClosing()
                .TabAdded(tabPage =>
                {
                    var scriptInfo = this.AddScript();
                    AddJavascriptControl(tabPage, scriptInfo);
                }).TabRemoved(tabPage =>
                {
                    if (tabPage.Tag is not TabPageScriptRecord record)
                    {
                        return;
                    }

                    this.RemoveScript(record.ScriptInfo);
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

            mainGroup.AddButtonPanel()
                 .AddButton(Locale.GRID_SCRIPT_AUTO_FORMAT, () => record?.JavascriptEditor.GetTextBox().DoAutoIndent())
                 .AddButton(Locale.GRID_SCRIPT_EXECUTE_ALL, () => ParseJS())
                 .AddButton(Locale.GRID_SCRIPT_EXECUTE_ONE_LINE, () => ParseJS(singleExecution: true));
        }

        public void Init()
        {
            this.configForm.FormClosed += (sender, args) =>
            {
                record?.JavascriptEditor.UnregisterErrorReport(jsErrorThread);
                record = null;
            };

            customVariableList.CollectionChanged += (sender, args) => this.labelTopLine?.Label(this.CreateVariableNamesText());
            gridVariableList.CollectionChanged += (sender, args) => this.labelTopLine?.Label(this.CreateVariableNamesText());
        }

        public ScriptInfo AddScript()
        {
            ScriptInfo scriptInfo = new();
            this.scriptInfoList.Add(scriptInfo);
            return scriptInfo;
        }

        public void RemoveScript(ScriptInfo scriptInfo)
        {
            this.scriptInfoList.Remove(scriptInfo);
        }

        public bool IsDirty() => this.scriptInfoList.Any(x => x.IsDirty());
        public void Wash() => this.scriptInfoList.ForEach(x => x.Wash());

        public GridScriptSave CreateSave()
        {
            GridScriptSave save = new();
            foreach (var scriptInfo in this.scriptInfoList)
            {
                save.Scripts.Add(scriptInfo);
            }
            return save;
        }

        public void LoadSave(GridScriptSave save)
        {
            this.scriptInfoList.Clear();
            scriptInfoList.AddRange(save.Scripts);
        }

        public void ClearAllCustomVariables() => customVariableList.Clear();

        public void AddCustomVariable(GridScriptVariable variable) => customVariableList.Add(variable);

        public void BindHandler<H>(GridHandler<H> gridHandler) where H : IGridData
        {
            if (this.bind != null && this.bind.IsSameGridHandler(gridHandler))
            {
                return;
            }

            this.ClearBind();

            this.bind = GridScriptHandlerBind.CreateBind(gridHandler);
            gridHandler.ScriptVariableList.ForEach(this.gridVariableList.Add);
        }

        public void ClearBind()
        {
            this.bind = null;
            this.gridVariableList.Clear();
        }

        private string CreateVariableNamesText()
        {
            List<string> variableTextList = [];
            variableTextList.AddRange(customVariableList.Select(v => $"{v.Name} [{v.ValueType}]"));
            variableTextList.AddRange(gridVariableList.Select(v => $"{v.Name} [{v.ValueType}]"));
            variableTextList.Add($"{ENGINE_ROW_VARIABLE} [int]");
            return variableTextList.Aggregate((a, b) => $"{a}, {b}");
        }

        public void ShowConfigForm(IWin32Window window)
        {
            if (configForm == null)
            {
                return;
            }

            if (this.scriptInfoList.Count == 0)
            {
                this.AddScript();
            }

            foreach (var scriptInfo in this.scriptInfoList)
            {
                var tabPage = tabLine.AddTabPage();
                AddJavascriptControl(tabPage, scriptInfo);
            }

            configForm.StartShowingAtCursor();
            configForm.Show(window);
        }

        private static void AddJavascriptControl(TabPage tabPage, ScriptInfo scriptInfo)
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
            Control? control = logTextBoxLine?.GetControl();
            if (control == null)
            {
                return;
            }

            var timeString = DateTime.Now.ToString("HH:mm:ss fff") + "ms";
            control.Text = $"{control.Text}{timeString}) {logString} \r\n";
        }

        private bool ParseJS(bool singleExecution = false, bool ignoreLog = false)
        {
            if (this.record == null || this.bind == null)
            {
                return false;
            }

            try
            {
                var tableScript = this.record.JavascriptEditor.GetTextBox().Text;

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

                this.bind.ClearCachedCellChange();

                ScriptTimeLogger timeLogger = new();

                List<int> rowIndexes = [];
                if (singleExecution)
                {
                    int rowIndex = this.bind.DataGridView.CurrentCell?.RowIndex ?? 0;
                    if (rowIndex >= 0 && rowIndex <= this.bind.DataGridView.RowCount)
                    {
                        if (!this.bind.IsGridDataEmpty(rowIndex))
                        {
                            rowIndexes.Add(rowIndex);
                        }

                        var nextRow = this.bind.GetFirstFullIndexStartingAt(rowIndex + 1);
                        //If there is no next row, start from top (and now we are here).
                        nextRow = nextRow < 0 ? this.bind.GetFirstFullIndexStartingAt(0) : nextRow;
                        if (nextRow >= 0)
                        {
                            this.bind.SelectRow(nextRow);
                        }
                    }
                }
                else
                {
                    rowIndexes.AddRange(this.bind.GetNotEmptyRowIndexes());
                }

                foreach (var rowIndex in rowIndexes)
                {
                    SetEngineVariables(engine, rowIndex, this.gridVariableList);
                    SetEngineVariables(engine, rowIndex, this.customVariableList);
                    engine.SetValue(ENGINE_ROW_VARIABLE, rowIndex);
                    ExecuteTimeLoggedJS(timeLogger, engine, preparedScript, rowIndex);
                }

                this.bind.ApplyCachedCellChange();

                UpdateJsonContext(engine);

                timeLogger.Log(tableScript, this.bind.DataTypeName);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex, silent: ignoreLog);
            }

            return false;
        }

        private static void SetEngineVariables(Engine engine, int rowIndex, ICollection<GridScriptVariable> scriptVariables)
        {
            foreach (var scriptVariable in scriptVariables)
            {
                var getFunc = scriptVariable.Get;
                if (getFunc == null)
                {
                    continue;
                }

                engine.SetValue(scriptVariable.Name, getFunc(rowIndex));
            }
        }

        private void ExecuteTimeLoggedJS(ScriptTimeLogger timeLogger, Engine engine, Prepared<Script> script, int row)
        {
            timeLogger.Restart();
            ExecuteJS(engine, script, row);
            timeLogger.StopAndSave();
        }

        private void ExecuteJS(Engine engine, Prepared<Script> script, int row)
        {
            var dataValuesDict = new Dictionary<string, GridJSVariable>();

            FillDataValueDictionary(dataValuesDict, row, this.gridVariableList);
            FillDataValueDictionary(dataValuesDict, row, this.customVariableList);

            foreach (var entry in dataValuesDict)
            {
                var ioJSVariable = entry.Value;
                engine.SetValue(entry.Key, ioJSVariable.OldValue);
            }

            var eval = engine.Evaluate(script);
            if (eval.IsBoolean() && !eval.AsBoolean())
            {
                return;
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
                return;
            }

            foreach (var ioJSVariable in dataValuesDict.Values)
            {
                var scriptVariable = ioJSVariable.ScriptVariable;
                if (scriptVariable.CreateCachedCellChange != null)
                { //THIS HAS THE PRIORITY!
                    scriptVariable.CreateCachedCellChange.Invoke(row, ioJSVariable.NewValue);
                }
                else if (scriptVariable.Set != null)
                {
                    scriptVariable.Set.Invoke(row, ioJSVariable.NewValue);
                }
            }
        }

        private static void FillDataValueDictionary(Dictionary<string, GridJSVariable> dict, int rowIndex, ICollection<GridScriptVariable> gridScriptVariables)
        {
            foreach (var scriptVariable in gridScriptVariables)
            {
                var value = scriptVariable.Get?.Invoke(rowIndex);
                if (value == null)
                {
                    continue;
                }

                dict.Add(scriptVariable.Name, new(scriptVariable) { OldValue = value });
            }
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

        private class GridJSVariable(GridScriptVariable scriptVariable)
        {
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public GridScriptVariable ScriptVariable { get; init; } = scriptVariable;
        }
    }
}
