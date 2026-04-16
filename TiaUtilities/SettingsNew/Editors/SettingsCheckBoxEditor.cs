using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsCheckBoxEditor : SettingsEditor
    {

        private readonly RJToggleButton toggleButton;
        public SettingsCheckBoxEditor(SettingsFormValueImpl value, bool useContextMenu) : base(value, useContextMenu)
        {
            this.toggleButton = new RJToggleButton
            {
                FlatStyle = FlatStyle.Flat,
                Text = "",
                AutoSize = true,
                BorderColor = SystemColors.ControlDark,
                OnBackColor = Color.Transparent,
                OffBackColor = Color.Transparent,
                OnToggleColor = Color.DarkGreen,
                OffToggleColor = Color.PaleVioletRed,
            };
            toggleButton.CheckedChanged += (sender, args) => this.SaveToConfiguration();

            if(useContextMenu)
            {
                var _ = SettingsFormUtils.AddContextualMenu(this.toggleButton, value);
            }
        }

        public override Control GetControl()
        {
            return this.toggleButton;
        }

        protected override Control GetControlForEvents()
        {
            return this.toggleButton;
        }

        public override void LoadFromConfiguration()
        {
            var boolValue = base.Value.GetConfigurationValue<bool>();
            this.toggleButton.Checked = boolValue;
        }

        public override void SaveToConfiguration()
        {
            this.Value.SetConfigurationValue(this.toggleButton.Checked);
        }
    }
}
