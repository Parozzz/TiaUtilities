using Acornima.Ast;
using Jint;
using System.Collections.ObjectModel;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Utility.Extensions;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptHandler(ErrorReportThread errorThread) : ICleanable, ISaveable<GridScriptSave>, IGridBindable
    {
        public const string ENGINE_LOG_FUNCTION = "log";
        public const string ENGINE_ROW_VARIABLE = "row";

        public ErrorReportThread ErrorThread { get; init; } = errorThread;

        public ObservableCollection<ScriptInfo> Scripts { get; init; } = [];
        private readonly ObservableCollection<GridScriptVariable> gridVariables = [];
        private readonly ObservableCollection<GridScriptVariable> customVariables = [];

        private readonly ObservableObject<string> log = new("");
        private readonly ObservableObject<string> jsonContext = new("");

        public GridHandlerBind? GridHandlerBind { get; private set; }
        private GridScriptForm? form;

        public void Init()
        {
            this.log.Changed += (sender, args) => form?.UpdateLog(args.NewValue);
            this.jsonContext.Changed += (sender, args) => form?.UpdateJsonContext(args.NewValue);

            this.gridVariables.CollectionChanged += (sender, args) => this.UpdateFormVariableView();
            this.gridVariables.CollectionChanged += (sender, args) => this.UpdateFormVariableView();

            if (this.Scripts.Count == 0)
            {
                this.Scripts.Add(new());
            }
        }

        public bool IsDirty() => this.Scripts.Any(x => x.IsDirty());
        public void Wash() => this.Scripts.ForEach(x => x.Wash());

        public GridScriptSave CreateSave()
        {
            GridScriptSave save = new();
            foreach (var scriptInfo in this.Scripts)
            {
                save.Scripts.Add(scriptInfo);
            }
            return save;
        }

        public void LoadSave(GridScriptSave save)
        {
            this.Scripts.Clear();
            this.Scripts.AddRange(save.Scripts);
        }

        public void ClearAllCustomVariables() => this.customVariables.Clear();
        public void AddCustomVariable(GridScriptVariable variable) => this.customVariables.Add(variable);

        public void BindToGridHandler(GridHandlerBind? handlerBind)
        {
            this.GridHandlerBind = handlerBind;

            this.gridVariables.Clear();
            if(handlerBind != null)
            {
                this.gridVariables.AddRange(handlerBind.ScriptVariables);
            }
        }

        public void ShowForm(IWin32Window? window = null)
        {
            if(form == null)
            {
                this.log.Value = "";
                this.jsonContext.Value = "";

                form = new(this)
                {
                    Width = 1150,
                    Height = 950
                };
                form.Init();

                form.FormClosed += (sender, args) => form = null;
                this.UpdateFormVariableView();

                form.Show(window);
            }
            else
            {
                form.Activate();
            }
        }

        public bool ParseJS(GridScriptForm.TabPageScriptRecord? record, bool singleExecution = false, bool ignoreLog = false)
        {
            if (record == null || this.GridHandlerBind == null)
            {
                return false;
            }

            try
            {
                var tableScript = record.Editor.GetTextBox().Text;

                var preparedScript = Engine.PrepareScript(tableScript, strict: true);

                using var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(1000)); // Set a timeout to 500 ms.
                    options.MaxStatements(int.MaxValue);
                    options.LimitRecursion(1);
                    options.Strict = true;
                });

                void log(string str)
                {
                    var timeString = DateTime.Now.ToString("HH:mm:ss fff") + "ms";
                    this.log.Value = $"{this.log.Value}{timeString}) {str} \r\n";
                }
                engine.SetValue(ENGINE_LOG_FUNCTION, log);

                this.GridHandlerBind.ClearCachedCellChange();

                ScriptTimeLogger timeLogger = new();

                List<int> rowIndexes = [];
                if (singleExecution)
                {
                    int rowIndex = this.GridHandlerBind.DataGridView.CurrentCell?.RowIndex ?? 0;
                    if (rowIndex >= 0 && rowIndex <= this.GridHandlerBind.DataGridView.RowCount)
                    {
                        if (!this.GridHandlerBind.IsGridDataEmpty(rowIndex))
                        {
                            rowIndexes.Add(rowIndex);
                        }

                        var nextRow = this.GridHandlerBind.GetFirstFullIndexStartingAt(rowIndex + 1);
                        //If there is no next row, start from top (and now we are here).
                        nextRow = nextRow < 0 ? this.GridHandlerBind.GetFirstFullIndexStartingAt(0) : nextRow;
                        if (nextRow >= 0)
                        {
                            this.GridHandlerBind.SelectRow(nextRow);
                        }
                    }
                }
                else
                {
                    rowIndexes.AddRange(this.GridHandlerBind.GetNotEmptyRowIndexesStartingAt(0));
                }

                var variables = this.JoinAllVariables();
                foreach (var rowIndex in rowIndexes)
                {
                    Dictionary<string, GridJSVariable> variablesDict = [];

                    engine.SetValue(ENGINE_ROW_VARIABLE, rowIndex);
                    foreach (var variable in variables)
                    {
                        var value = variable.Get?.Invoke(rowIndex);
                        engine.SetValue(variable.Name, value);

                        variablesDict.Add(variable.Name, new(variable) { OldValue = value });
                    }

                    timeLogger.Restart();
                    ExecuteJS(engine, preparedScript, variablesDict, rowIndex);
                    timeLogger.StopAndSave();
                }

                this.GridHandlerBind.ApplyCachedCellChange();

                //Update JSON Context Text
                var contextJsonJSValue = engine.Evaluate(@"JSON.stringify(this, null, 2);");
                if (contextJsonJSValue.IsString())
                {
                    this.jsonContext.Value = contextJsonJSValue.AsString();
                }

                timeLogger.Log(tableScript, this.GridHandlerBind.DataTypeName);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex, silent: ignoreLog);
            }

            return false;
        }

        private static void ExecuteJS(Engine engine, Prepared<Script> script, Dictionary<string, GridJSVariable> variablesDict, int row)
        {
            var eval = engine.Evaluate(script);
            if (eval.IsBoolean() && !eval.AsBoolean())
            {
                return;
            }

            var changed = false;
            foreach (var entry in variablesDict)
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

            foreach (var ioJSVariable in variablesDict.Values)
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

        public void ClearLog()
        {
            this.log.Value = "";
        }

        private IEnumerable<GridScriptVariable> JoinAllVariables()
        {
            return this.customVariables.Concat(this.gridVariables);
        }

        private void UpdateFormVariableView()
        {
            form?.UpdateVariableView(this.JoinAllVariables());
        }

        private class GridJSVariable(GridScriptVariable scriptVariable)
        {
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public GridScriptVariable ScriptVariable { get; init; } = scriptVariable;
        }

    }
}
