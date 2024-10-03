using Acornima.Ast;
using FastColoredTextBoxNS;
using Jint;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public partial class GridScriptForm : Form
    {
        public record TabPageScriptRecord(ScriptInfo Script, JavascriptEditor Editor);

        private readonly GridScriptHandler scriptHandler;
        private readonly FastColoredTextBox jsonContextTextBox;
        
        public GridScriptForm(GridScriptHandler scriptHandler)
        {
            InitializeComponent();

            this.scriptHandler = scriptHandler;
            this.jsonContextTextBox = new();
        }

        public void Init()
        {
            #region JSON_CONTEXT_TEXT_BOX_CONFIGURATION
            this.jsonContextTextBox.ReadOnly = true;
            this.jsonContextTextBox.BorderStyle = BorderStyle.FixedSingle;

            this.jsonContextTextBox.AutoSize = true;
            this.jsonContextTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.jsonContextTextBox.Dock = DockStyle.Fill;

            this.jsonContextTextBox.BackColor = SystemColors.Control;

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
            this.jsonContextTextBox.ShowCaretWhenInactive = false;
            this.jsonContextTextBox.WideCaret = false;

            this.jsonContextTextBox.CharHeight = 16; //Default 14
            this.jsonContextTextBox.LineInterval = 4; //Default 0

            this.jsonContextTextBox.AcceptsTab = true;
            this.jsonContextTextBox.AcceptsReturn = true;
            this.jsonContextTextBox.ShowFoldingLines = true;
            this.jsonContextPanel.Controls.Add(jsonContextTextBox);
            #endregion

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
                    var textBox = currentRecord.Editor.GetTextBox();
                    textBox.InsertText(variable.Name);
                    textBox.Focus();
                }
            };

            this.scriptTabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;

                ScriptInfo script = new();
                AddJavascriptControl(tabPage, script);
                this.scriptHandler.Scripts.Add(script);
            };

            this.scriptTabControl.TabPreRemoved += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                this.scriptHandler.Scripts.Remove(record.Script);
            };

            this.scriptTabControl.Selected += (sender, args) =>
            {
                var tabPage = args.TabPage;

                var currentRecord = this.GetCurrentTabPageRecord();
                currentRecord?.Editor.UnregisterErrorReport(this.scriptHandler.JSErrorThread);

                if (tabPage?.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                record.Editor.RegisterErrorReport(this.scriptHandler.JSErrorThread);
            };

            this.scriptTabControl.TabNameUserChanged += (sender, args) =>
            {
                var tabPage = args.TabPage;
                if (tabPage.Tag is not TabPageScriptRecord record)
                {
                    return;
                }

                record.Script.Name = args.NewName;
            };

            this.autoFormatButton.Click += (sender, args) => this.GetCurrentTabPageRecord()?.Editor.GetTextBox().DoAutoIndent();
            this.executeAllButton.Click += (sender, args) => this.scriptHandler.ParseJS(this.GetCurrentTabPageRecord());
            this.executeLineButton.Click += (sender, args) => this.scriptHandler.ParseJS(this.GetCurrentTabPageRecord(), singleExecution: true);

            foreach (var script in this.scriptHandler.Scripts)
            {
                TabPage tabPage = new();
                AddJavascriptControl(tabPage, script);
                this.scriptTabControl.TabPages.Add(tabPage);
            }

            this.FormClosed += (sender, args) =>
            {
                var currentRecord = this.GetCurrentTabPageRecord();
                currentRecord?.Editor.UnregisterErrorReport(this.scriptHandler.JSErrorThread);
            };

            this.Translate();
        }

        private void Translate()
        {
            this.topLabel.Text = Locale.GRID_SCRIPT_JS_EXPRESSION;
            this.logLabel.Text = $"Log > {GridScriptHandler.ENGINE_LOG_FUNCTION} [string]";
            this.jsonContextLabel.Text = Locale.GRID_SCRIPT_JSON_CONTEXT;

            this.autoFormatButton.Text = Locale.GRID_SCRIPT_AUTO_FORMAT;
            this.executeAllButton.Text = Locale.GRID_SCRIPT_EXECUTE_ALL;
            this.executeLineButton.Text = Locale.GRID_SCRIPT_EXECUTE_ONE_LINE;
        }

        public void UpdateVariableView(IEnumerable<GridScriptVariable> variables)
        {
            this.variablesTreeView.SuspendLayout();
            this.variablesTreeView.Nodes.Clear();

            foreach (var v in variables)
            {
                var node = this.variablesTreeView.Nodes.Add($"{v.Name}, {v.ValueType}");
                node.Tag = v;
            }

            this.variablesTreeView.ResumeLayout();
        }

        private static void AddJavascriptControl(TabPage tabPage, ScriptInfo scriptInfo)
        {
            JavascriptEditor javascriptEditor = new();
            javascriptEditor.InitControl();

            var fctb = javascriptEditor.GetTextBox();
            fctb.AutoSize = true;
            fctb.Dock = DockStyle.Fill;
            fctb.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            tabPage.Text = scriptInfo.Name;
            fctb.Text = scriptInfo.Text;
            fctb.TextChanged += (sender, args) => scriptInfo.Text = javascriptEditor.GetTextBox().Text;

            tabPage.Controls.Add(fctb);
            tabPage.Tag = new TabPageScriptRecord(scriptInfo, javascriptEditor);
        }

        public void UpdateLog(string logString)
        {
            this.logTextBox.Text = logString;
        }

        public void UpdateJsonContext(string contextString)
        {
            this.jsonContextTextBox.Text = contextString;
        }

        private TabPageScriptRecord? GetCurrentTabPageRecord()
        {
            return this.scriptTabControl.SelectedTab?.Tag is TabPageScriptRecord record ? record : null;
        }
    }
}
