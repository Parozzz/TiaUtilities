using DocumentFormat.OpenXml.Bibliography;
using TiaUtilities.Editors;
using TiaUtilities.Editors.myScintilla;
using TiaUtilities.Languages;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;
using static TiaUtilities.JSScript.IJsScriptExecutionData;

namespace TiaUtilities.JSScript
{
    public partial class JSScriptForm : Form
    {
        public record TabPageScriptRecord(ScriptInfo Script, JavascriptEditor Editor);

        private readonly JSScriptHandler scriptHandler;
        
        public IJsScriptExecutionData.DataDescriptor? DataDescriptor {
            get => _dataDescriptor;
            set 
            {
                _dataDescriptor = value;
                this.UpdateVariables();
            }
        }
        private IJsScriptExecutionData.DataDescriptor? _dataDescriptor;

        private readonly JsonEditor contextEditor;
        private readonly LoggerScintilla logger;

        public JSScriptForm(JSScriptHandler scriptHandler)
        {
            InitializeComponent();
            this.scriptHandler = scriptHandler;

            this.contextEditor = new(this.jsonContextScintilla);
            this.logger = new(this.loggerScintilla);
        }

        public void Init()
        {
            this.contextEditor.InitControl(borderStyle: ScintillaNET.BorderStyle.FixedSingle, backColor: SystemColors.Control);
            this.contextEditor.Text = "{}"; //Avoid throwing errors at startup.

            var contextEditorControl = this.contextEditor.GetControl();
            contextEditorControl.ReadOnly = true;

            this.logger.InitControl(borderStyle: ScintillaNET.BorderStyle.FixedSingle, backColor: SystemColors.Control);

            var loggerControl = this.logger.Scintilla;

            var contextMenu = loggerControl.ContextMenuStrip;
            if(contextMenu == null)
            {
                contextMenu = new();
            } else
            {
                contextMenu.Items.Add(new ToolStripSeparator());
            }
            var clearLogMenuItem = new ToolStripMenuItem(Locale.GENERICS_CLEAR);
            clearLogMenuItem.Click += (sender, args) => this.ClearLog();

            contextMenu.Items.Add(clearLogMenuItem);

            /*
            this.variablesTreeView.NodeMouseDoubleClick += (sender, args) =>
            {
                var currentRecord = this.GetCurrentTabPageRecord();
                if(currentRecord == null)
                {
                    return;
                }

                if(args.Node.Tag is JSScriptVariable variable)
                {
                    var editor = currentRecord.Editor;
                    editor.InsertText(variable.Name);
                    editor.FocusControl();
                }
            };
            */
            this.scriptTabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;

                ScriptInfo script = new();
                AddJavascriptControl(tabPage, script);

                var tabNames = this.GetTabNames(toIgnore: tabPage);

                var fixedNewName = Utils.CheckEqualityAndAddNumberAtEnd(script.Name, tabNames);
                script.Name = fixedNewName;
                tabPage.Text = fixedNewName;

                this.scriptHandler.Scripts.Add(script);
            };

            this.scriptTabControl.TabPreRemoved += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is TabPageScriptRecord record)
                {
                    this.scriptHandler.Scripts.Remove(record.Script);
                }
            };

            this.scriptTabControl.Selected += (sender, args) => { };

            this.scriptTabControl.TabNameUserChanged += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                var tabNames = this.GetTabNames(toIgnore: tabPage);

                var fixedNewName = Utils.CheckEqualityAndAddNumberAtEnd(args.NewName, tabNames);
                args.NewName = fixedNewName;
                record.Script.Name = fixedNewName;
            };

            this.executeAllButton.Click += (sender, args) => this.scriptHandler.ParseJS(this.GetCurrentTabPageRecord());
            this.executeLineButton.Click += (sender, args) => this.scriptHandler.ParseJS(this.GetCurrentTabPageRecord(), singleExecution: true);

            foreach (var script in this.scriptHandler.Scripts)
            {
                TabPage tabPage = new();
                AddJavascriptControl(tabPage, script);
                this.scriptTabControl.TabPages.Add(tabPage);
            }

            this.Translate();
        }

        private List<string> GetTabNames(TabPage toIgnore)
        {
            List<string> tabNames = [];
            foreach (TabPage tab in this.scriptTabControl.TabPages)
            {
                if (tab != toIgnore)
                {
                    tabNames.Add(tab.Text);
                }
            }
            return tabNames;
        }

        private void Translate()
        {
            this.topLabel.Text = Locale.GRID_SCRIPT_JS_EXPRESSION;
            this.logLabel.Text = $"Log > {JSScriptHandler.ENGINE_CONSOLE_CLASS} [string]";
            this.jsonContextLabel.Text = Locale.GRID_SCRIPT_JSON_CONTEXT;
            this.executeAllButton.Text = Locale.GRID_SCRIPT_EXECUTE_ALL;
            this.executeLineButton.Text = Locale.GRID_SCRIPT_EXECUTE_ONE_LINE;
        }

        private void UpdateVariables()
        {
            if(_dataDescriptor == null || (_dataDescriptor.SimpleProperties.Count == 0 && _dataDescriptor.ObjectProperties.Count == 0))
            {
                return;
            }

            this.variablesTreeView.SuspendLayout();
            this.variablesTreeView.Nodes.Clear();

            var dataName = _dataDescriptor.Name;

            var dataNode = this.variablesTreeView.Nodes.Add(dataName);
            foreach (var (pName, pType) in _dataDescriptor.SimpleProperties)
            {
                var node = dataNode.Nodes.Add($"{pName} [{pType}]");
            }

            this.scriptTabControl.TabPages.Cast<TabPage>()
                .Where(t => t.Tag is TabPageScriptRecord)
                .Select(t => t.Tag)
                .Cast<TabPageScriptRecord>()
                .Select(r => r.Editor)
                .ForEach(this.UpdateEditorSuggestion);

            this.variablesTreeView.ResumeLayout();
        }

        private void AddJavascriptControl(TabPage tabPage, ScriptInfo scriptInfo)
        {
            JavascriptEditor jsEditor = new();
            jsEditor.InitControl();
            jsEditor.Text = scriptInfo.Text;
            jsEditor.TextChanged += (sender, args) => scriptInfo.Text = jsEditor.Text;
            this.UpdateEditorSuggestion(jsEditor);

            tabPage.Controls.Add(jsEditor.GetControl());

            tabPage.Text = scriptInfo.Name;
            tabPage.Tag = new TabPageScriptRecord(scriptInfo, jsEditor);
        }

        private void UpdateEditorSuggestion(JavascriptEditor editor)
        {
            if (_dataDescriptor == null || (_dataDescriptor.SimpleProperties.Count == 0 && _dataDescriptor.ObjectProperties.Count == 0))
            {
                return;
            }

            editor.Suggestions = _dataDescriptor.SimpleProperties.Select(p => $"{_dataDescriptor.Name}.{p.Key}");
        }

        public void ClearLog()
        {
            this.logger.Scintilla.ClearAll();
        }

        public void InsertLog(DateTime dateTime, LoggerScintilla.LogLevel logLevel, string text)
        {
            this.logger.AppendLine(dateTime, logLevel, text);
        }

        public void UpdateJsonContext(string contextString)
        {
            this.contextEditor.Text = contextString;
        }

        private TabPageScriptRecord? GetCurrentTabPageRecord()
        {
            return this.scriptTabControl.SelectedTab?.Tag is TabPageScriptRecord record ? record : null;
        }
    }
}
