using TiaUtilities.Editors;
using TiaUtilities.Languages;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.JSScript
{
    public partial class GridScriptForm : Form
    {
        public record TabPageScriptRecord(ScriptInfo Script, JavascriptEditor Editor);

        private readonly GridScriptHandler scriptHandler;
        
        public IEnumerable<GridScriptVariable> Variables {
            get => _variables;
            set 
            { 
                _variables = value;
                this.UpdateVariables();
            }
        }
        private IEnumerable<GridScriptVariable> _variables = [];

        private readonly JsonEditor contextEditor;

        public GridScriptForm(GridScriptHandler scriptHandler)
        {
            InitializeComponent();
            this.scriptHandler = scriptHandler;

            this.contextEditor = new(this.jsonContextScintilla);
        }

        public void Init()
        {
            this.contextEditor.InitControl(borderStyle: ScintillaNET.BorderStyle.FixedSingle, backColor: SystemColors.Control);
            this.contextEditor.Text = "{}"; //Avoid throwing errors at startup.

            var contextEditorControl = this.contextEditor.GetControl();
            contextEditorControl.ReadOnly = true;

            #region LOG_TEXT_BOX
            var menuItem = new ToolStripMenuItem(Locale.GENERICS_CLEAR);
            menuItem.Click += (sender, args) => this.scriptHandler.ClearLog();

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(menuItem);
            this.logTextBox.ContextMenuStrip = contextMenu;

            //Scroll to the end after text is changed!
            this.logTextBox.TextChanged += (sender, args) =>
            {
                this.logTextBox.Select(this.logTextBox.TextLength + 1, 0);
                this.logTextBox.ScrollToCaret();
            };
            #endregion

            this.variablesTreeView.NodeMouseDoubleClick += (sender, args) =>
            {
                var currentRecord = this.GetCurrentTabPageRecord();
                if(currentRecord == null)
                {
                    return;
                }

                if(args.Node.Tag is GridScriptVariable variable)
                {
                    var editor = currentRecord.Editor;
                    editor.InsertText(variable.Name);
                    editor.FocusControl();
                }
            };

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
            this.logLabel.Text = $"Log > {GridScriptHandler.ENGINE_LOG_FUNCTION} [string]";
            this.jsonContextLabel.Text = Locale.GRID_SCRIPT_JSON_CONTEXT;
            this.executeAllButton.Text = Locale.GRID_SCRIPT_EXECUTE_ALL;
            this.executeLineButton.Text = Locale.GRID_SCRIPT_EXECUTE_ONE_LINE;
        }

        private void UpdateVariables()
        {
            if(_variables == null || !_variables.Any())
            {
                return;
            }

            this.variablesTreeView.SuspendLayout();
            this.variablesTreeView.Nodes.Clear();

            foreach (var v in _variables)
            {
                var node = this.variablesTreeView.Nodes.Add($"{v.Name}, {v.ValueType}");
                node.Tag = v;
            }

            var suggestions = this.CreateEditorSuggestion();
            this.scriptTabControl.TabPages.Cast<TabPage>()
                .Where(t => t.Tag is TabPageScriptRecord)
                .Select(t => t.Tag)
                .Cast<TabPageScriptRecord>()
                .ForEach(r => r.Editor.Suggestions = suggestions);

            this.variablesTreeView.ResumeLayout();
        }

        private void AddJavascriptControl(TabPage tabPage, ScriptInfo scriptInfo)
        {
            JavascriptEditor jsEditor = new();
            jsEditor.InitControl();
            jsEditor.Text = scriptInfo.Text;
            jsEditor.TextChanged += (sender, args) => scriptInfo.Text = jsEditor.Text;
            jsEditor.Suggestions = CreateEditorSuggestion(); ;

            tabPage.Text = scriptInfo.Name;

            tabPage.Controls.Add(jsEditor.GetControl());
            //tabPage.Controls.Add(fctb);
            tabPage.Tag = new TabPageScriptRecord(scriptInfo, jsEditor);
        }

        private string CreateEditorSuggestion() => String.Join(' ', this._variables.Select(v => $"{v.Name}"));

        public void UpdateLog(string logString)
        {
            this.logTextBox.Text = logString;
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
