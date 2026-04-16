using FastColoredTextBoxNS;
using TiaUtilities.Editors;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsJSONEditor : SettingsEditor
    {
        private readonly JsonEditor editor;
        private FastColoredTextBox Control { get => editor.GetTextBox(); }

        public SettingsJSONEditor(SettingsFormValueImpl value, bool useContextMenu) : base(value, useContextMenu)
        {
            this.editor = new JsonEditor();
            this.editor.InitControl();

            this.Control.Dock = DockStyle.Fill;
            this.Control.MinimumSize = new Size(0, 500);
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
    }
}
