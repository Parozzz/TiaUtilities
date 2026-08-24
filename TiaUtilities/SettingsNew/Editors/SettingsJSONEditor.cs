using TiaUtilities.Editors.Json;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsJSONEditor : SettingsEditor
    {
        private readonly JsonEditor jsonEditor;

        public SettingsJSONEditor(SettingsFormValueImpl value, bool useContextMenu) : base(value, useContextMenu)
        {
            this.jsonEditor = new JsonEditor();
            this.jsonEditor.InitControl();

            var control = this.jsonEditor.GetControl();
            control.Dock = DockStyle.Fill;
            control.MinimumSize = new Size(0, 500);
            control.Font = SettingsFormConstants.VALUE_CONTROL_FONT;
            control.TextChanged += (sender, args) => this.SaveToConfiguration();

            if (useContextMenu)
            {
                var _ = SettingsFormUtils.AddContextualMenu(control, value);
            }
        }

        public override Control GetControl() => this.jsonEditor.GetControl();

        protected override Control GetControlForEvents() => this.GetControl();

        public override void LoadFromConfiguration()
        {
            this.jsonEditor.Text = "" + this.Value.GetConfigurationValue();
        }

        public override void SaveToConfiguration()
        {
            this.Value.SetConfigurationValue(this.jsonEditor.Text);
        }
    }
}
