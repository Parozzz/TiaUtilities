using Acornima.Ast;
using FastColoredTextBoxNS;
using Jint;
using System.Collections.ObjectModel;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public partial class GridScriptForm : Form
    {
        private const string ENGINE_LOG_FUNCTION = "log";
        private const string ENGINE_ROW_VARIABLE = "row";

        private record TabPageScriptRecord(ScriptInfo ScriptInfo, JavascriptEditor JavascriptEditor);

        private readonly GridScriptHandler scriptHandler;

        private readonly List<ScriptInfo> scriptInfoList;
        private readonly ObservableCollection<GridScriptVariable> variableList;

        private GridHandlerBind? handlerBind;
        private TabPageScriptRecord? record;

        public GridScriptForm(GridScriptHandler scriptHandler, List<ScriptInfo> scriptInfoList)
        {
            InitializeComponent();

            this.scriptHandler = scriptHandler;
            this.scriptInfoList = scriptInfoList;

            this.variableList = [];
        }

        public void Init()
        {
            variableList.CollectionChanged += (sender, args) => this.UpdateVariableView();

            #region JSON_CONTEXT_TEXT_BOX_CONFIGURATION
            this.jsonContextTextBox.Language = Language.JSON;
            // == INDENTATION ==
            this.jsonContextTextBox.AutoIndent = true;
            this.jsonContextTextBox.AutoIndentExistingLines = true;
            this.jsonContextTextBox.AutoIndentChars = true;
            this.jsonContextTextBox.TabLength = 4;
            // == LINE NUMBERS ==
            this.jsonContextTextBox.ShowLineNumbers = false;
            this.jsonContextTextBox.LineNumberStartValue = 0;
            // == CARET ==
            this.jsonContextTextBox.CaretVisible = true;
            this.jsonContextTextBox.CaretBlinking = true;
            this.jsonContextTextBox.ShowCaretWhenInactive = true;
            this.jsonContextTextBox.WideCaret = false;

            this.jsonContextTextBox.CharHeight = 16; //Default 14
            this.jsonContextTextBox.LineInterval = 4; //Default 0

            this.jsonContextTextBox.AcceptsTab = true;
            this.jsonContextTextBox.AcceptsReturn = true;
            this.jsonContextTextBox.ShowFoldingLines = true;
            #endregion


            this.scriptTabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;

                var scriptInfo = scriptHandler.AddScript();
                AddJavascriptControl(tabPage, scriptInfo);
            };

            this.scriptTabControl.TabPreRemoved += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                scriptHandler.RemoveScript(record.ScriptInfo);
                if(this.record == record)
                {
                    this.record = null;
                }
            };

            this.scriptTabControl.Selected += (sender, args) =>
            {
                var tabPage = args.TabPage;

                this.record?.JavascriptEditor.UnregisterErrorReport(this.scriptHandler.JSErrorThread);
                this.record = null;

                if (tabPage?.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                this.record = record;
                this.record.JavascriptEditor.RegisterErrorReport(this.scriptHandler.JSErrorThread);
            };

            this.scriptTabControl.TabNameUserChanged += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                record.ScriptInfo.Name = newName;
            };

            this.autoFormatButton.Click += (sender, args) => record?.JavascriptEditor.GetTextBox().DoAutoIndent();
            this.executeAllButton.Click += (sender, args) => { };

            mainGroup.AddButtonPanel()
                 .AddButton(Locale.GRID_SCRIPT_AUTO_FORMAT, () => record?.JavascriptEditor.GetTextBox().DoAutoIndent())
                 .AddButton(Locale.GRID_SCRIPT_EXECUTE_ALL, () => ParseJS())
                 .AddButton(Locale.GRID_SCRIPT_EXECUTE_ONE_LINE, () => ParseJS(singleExecution: true));

            this.FormClosed += (sender, args) =>
            {
                record?.JavascriptEditor.UnregisterErrorReport(jsErrorThread);
                record = null;
            };

            if (this.scriptInfoList.Count == 0)
            {
                this.AddScript();
            }

            foreach (var scriptInfo in this.scriptInfoList)
            {
                var tabPage = tabLine.AddTabPage();
                AddJavascriptControl(tabPage, scriptInfo);
            }
        }

        private void Translate()
        {
            this.topLabel.Text = Locale.GRID_SCRIPT_JS_EXPRESSION;
            this.logLabel.Text = $"Log > {ENGINE_LOG_FUNCTION} [string]";
            this.jsonContextLabel.Text = Locale.GRID_SCRIPT_JSON_CONTEXT;

            this.autoFormatButton.Text = Locale.GRID_SCRIPT_AUTO_FORMAT;
            this.executeAllButton.Text = Locale.GRID_SCRIPT_EXECUTE_ALL;
            this.executeLineButton.Text = Locale.GRID_SCRIPT_EXECUTE_ONE_LINE;
        }

        private void UpdateVariableView()
        {
            this.variablesTreeView.SuspendLayout();
            this.variablesTreeView.Nodes.Clear();

            foreach (var v in this.variableList)
            {
                this.variablesTreeView.Nodes.Add($"{v.Name}, {v.ValueType}");
            }

            this.variablesTreeView.ResumeLayout();
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
            var timeString = DateTime.Now.ToString("HH:mm:ss fff") + "ms";
            this.jsonContextTextBox.Text = $"{this.jsonContextTextBox.Text}{timeString}) {logString} \r\n";
        }

        private bool ParseJS(bool singleExecution = false, bool ignoreLog = false)
        {
            if (this.record == null || this.handlerBind == null)
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

                this.handlerBind.ClearCachedCellChange();

                ScriptTimeLogger timeLogger = new();

                List<int> rowIndexes = [];
                if (singleExecution)
                {
                    int rowIndex = this.handlerBind.DataGridView.CurrentCell?.RowIndex ?? 0;
                    if (rowIndex >= 0 && rowIndex <= this.handlerBind.DataGridView.RowCount)
                    {
                        if (!this.handlerBind.IsGridDataEmpty(rowIndex))
                        {
                            rowIndexes.Add(rowIndex);
                        }

                        var nextRow = this.handlerBind.GetFirstFullIndexStartingAt(rowIndex + 1);
                        //If there is no next row, start from top (and now we are here).
                        nextRow = nextRow < 0 ? this.handlerBind.GetFirstFullIndexStartingAt(0) : nextRow;
                        if (nextRow >= 0)
                        {
                            this.handlerBind.SelectRow(nextRow);
                        }
                    }
                }
                else
                {
                    rowIndexes.AddRange(this.handlerBind.GetNotEmptyRowIndexesStartingAt(0));
                }

                foreach (var rowIndex in rowIndexes)
                {
                    engine.SetValue(ENGINE_ROW_VARIABLE, rowIndex);
                    foreach (var scriptVariable in this.variableList)
                    {
                        var value = scriptVariable.Get?.Invoke(rowIndex);
                        if (value != null)
                        {
                            engine.SetValue(scriptVariable.Name, value);
                        }
                    }

                    timeLogger.Restart();
                    ExecuteJS(engine, preparedScript, rowIndex);
                    timeLogger.StopAndSave();
                }

                this.handlerBind.ApplyCachedCellChange();

                //Update JSON Context Text
                var contextJsonJSValue = engine.Evaluate(@"JSON.stringify(this, null, 2);");
                if (contextJsonJSValue.IsString())
                {
                    this.jsonContextTextBox.Text = contextJsonJSValue.AsString();
                }

                timeLogger.Log(tableScript, this.handlerBind.DataTypeName);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex, silent: ignoreLog);
            }

            return false;
        }

        private void ExecuteJS(Engine engine, Prepared<Script> script, int row)
        {
            Dictionary<string, GridJSVariable> dataValuesDict = [];

            foreach (var scriptVariable in this.variableList)
            {
                var value = scriptVariable.Get?.Invoke(row);
                if (value == null)
                {
                    continue;
                }

                dataValuesDict.Add(scriptVariable.Name, new(scriptVariable) { OldValue = value });
            }

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

        private class GridJSVariable(GridScriptVariable scriptVariable)
        {
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public GridScriptVariable ScriptVariable { get; init; } = scriptVariable;
        }
    }
}
