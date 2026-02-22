using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsCheckBoxEditor : SettingsEditor
    {

        private readonly RJToggleButton toggleButton;
        public SettingsCheckBoxEditor(SettingsFormValueImpl value) : base(value)
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

            var _ = SettingsUtils.AddContextualMenu(this.toggleButton, value);

            base.RegisterPropertyChanged(this.toggleButton);
            this.LoadFromConfiguration();
        }

        public override Control GetControl()
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
