using DocumentFormat.OpenXml.Bibliography;
using FastColoredTextBoxNS;
using TiaUtilities.Editors;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsJavascriptEditor : SettingsEditor
    {
        private readonly JavascriptEditor editor;
        private FastColoredTextBox Control { get => editor.GetTextBox(); }

        public SettingsJavascriptEditor(SettingsFormValueImpl value, bool useContextMenu) : base(value, useContextMenu)
        {
            this.editor = new JavascriptEditor();
            this.editor.InitControl();

            this.Control.Dock = DockStyle.Fill;
            this.Control.MinimumSize = new Size(0, 400);
            this.Control.Font = SettingsFormConstants.VALUE_CONTROL_FONT;
            this.Control.TextChanged += (sender, args) => this.SaveToConfiguration();

            if (useContextMenu)
            {
                var _ = SettingsFormUtils.AddContextualMenu(this.Control, value);
            }
        }

        public override Control GetControl()
        {
            return this.Control;
        }

        protected override Control GetControlForEvents()
        {
            return this.Control;
        }

        public override void LoadFromConfiguration()
        {
            this.Control.Text = "" + this.Value.GetConfigurationValue();
        }

        public override void SaveToConfiguration()
        {
            this.Value.SetConfigurationValue(this.Control.Text);
        }

        public override void AddFormCallbacks(Form form)
        {
            this.editor.RegisterErrorReporter(MainForm.JavascriptErrorThread);
            form.FormClosing += (sender, args) => this.editor.UnregisterErrorReporter(MainForm.JavascriptErrorThread);
        }
    }
}
