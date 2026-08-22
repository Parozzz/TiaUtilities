using TiaUtilities.Editors;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsJavascriptEditor : SettingsEditor
    {
        private readonly JavascriptEditor jsEditor;

        public SettingsJavascriptEditor(SettingsFormValueImpl value, bool useContextMenu) : base(value, useContextMenu)
        {
            this.jsEditor = new JavascriptEditor();
            this.jsEditor.InitControl();

            this.jsEditor.TextChanged += (sender, args) => this.SaveToConfiguration();

            var control = this.jsEditor.GetControl();
            control.MinimumSize = new Size(0, 400);
            control.Font = SettingsFormConstants.VALUE_CONTROL_FONT;

            if (useContextMenu)
            {
                var _ = SettingsFormUtils.AddContextualMenu(this.jsEditor.GetControl(), value);
            }
        }

        public override Control GetControl()
        {
            return this.jsEditor.GetControl();
        }

        protected override Control GetControlForEvents()
        {
            return this.jsEditor.GetControl();
        }

        public override void LoadFromConfiguration()
        {
            this.jsEditor.Text = "" + this.Value.GetConfigurationValue();
        }

        public override void SaveToConfiguration()
        {
            this.Value.SetConfigurationValue(this.jsEditor.Text);
        }

        public override void AddFormCallbacks(Form form)
        {
        }
    }
}
