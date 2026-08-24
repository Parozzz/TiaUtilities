using Jint;
using System.Collections.ObjectModel;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.JSScript
{
    public class JSScriptHandler() : ICleanable, ISaveable<JSScriptSave>
    {
        public const string ENGINE_CONSOLE_CLASS = "console";
        public const string ENGINE_ROW_VARIABLE = "row";

        public ObservableCollection<ScriptInfo> Scripts { get; init; } = [];

        private readonly ObservableObject<string> jsonContext = new("");

        private IJsScriptExecutionData? executionData;
        private JSScriptForm? form;

        private readonly JsScriptConsole console = new();

        public void Init()
        {
            this.console.LogEvent += (sender, args) =>
            {
                this.form?.InsertLog(args.DateTime, args.LogLevel, args.Message);
            };

            this.jsonContext.Changed += (sender, args) => form?.UpdateJsonContext(args.NewValue);

            if (this.Scripts.Count == 0)
            {
                this.Scripts.Add(new());
            }
        }

        public bool IsDirty() => this.Scripts.Any(x => x.IsDirty());
        public void Wash() => this.Scripts.ForEach(x => x.Wash());

        public JSScriptSave CreateSave()
        {
            JSScriptSave save = new();
            foreach (var scriptInfo in this.Scripts)
            {
                save.Scripts.Add(scriptInfo);
            }
            return save;
        }

        public void LoadSave(JSScriptSave save)
        {
            this.Scripts.Clear();
            this.Scripts.AddRange(save.Scripts);
        }

        public void SetExecutionData(IJsScriptExecutionData? executionData) 
        { 
            this.executionData = executionData;
            if(form != null && executionData != null)
            {
                form.DataDescriptor = executionData.Descriptor;
            }
        }

        public void ShowForm(IWin32Window? window = null)
        {
            if (form == null)
            {
                this.jsonContext.Value = "";

                form = new(this)
                {
                    Width = 1150,
                    Height = 950
                };
                form.Init();

                if (this.executionData != null)
                {
                    form.DataDescriptor = executionData.Descriptor;
                }

                form.FormClosed += (sender, args) => form = null;
                form.Show(window);
            }
            else
            {
                form.Activate();
            }
        }

        public bool ParseJS(JSScriptForm.TabPageScriptRecord? record, bool singleExecution = false, bool ignoreLog = false)
        {
            if (record == null)
            {
                return false;
            }

            try
            {
                var tableScript = record.Editor.Text;

                var preparedScript = Engine.PrepareScript(tableScript, strict: true);

                using var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(1000)); // Set a timeout to 1000 ms.
                    options.MaxStatements(int.MaxValue);
                    options.LimitRecursion(1);
                    options.Strict = true;
                });


                engine.SetValue(ENGINE_CONSOLE_CLASS, console);

                ScriptTimeLogger timeLogger = new();

                try
                {
                    Object? data = null;
                    if(executionData != null)
                    {
                        data = executionData.RequestData();
                        engine.SetValue(executionData.Descriptor.Name, data);
                    }

                    timeLogger.Restart();

                    var eval = engine.Evaluate(preparedScript);
                    if (!eval.IsBoolean() || eval.AsBoolean())
                    {
                        executionData?.Done(data);
                    }

                    timeLogger.StopAndSave();
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }

                //Update JSON Context Text
                var contextJsonJSValue = engine.Evaluate(@"JSON.stringify(this, null, 2);");
                if (contextJsonJSValue.IsString())
                {
                    this.jsonContext.Value = contextJsonJSValue.AsString();
                }

                timeLogger.Log(tableScript, this.GetType().Name);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex, silent: ignoreLog);
            }

            return false;
        }

        private class GridJSVariable(JSScriptVariable scriptVariable)
        {
            public object? OldValue { get; set; }
            public object? NewValue { get; set; }
            public JSScriptVariable ScriptVariable { get; init; } = scriptVariable;
        }

    }
}
