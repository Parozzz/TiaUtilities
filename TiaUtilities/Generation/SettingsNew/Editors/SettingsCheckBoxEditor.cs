using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsCheckBoxEditor : SettingsEditor
    {

        private readonly RJToggleButton button;
        public SettingsCheckBoxEditor(SettingsValue value) : base(value)
        {
            this.button = new RJToggleButton
            {
                Checked = (bool) value.GetConfigurationValue(),
                FlatStyle = FlatStyle.Flat,
                Text = "",
                AutoSize = true,
                BorderColor = SystemColors.ControlDark,
                OnBackColor = Color.Transparent,
                OffBackColor = Color.Transparent,
                OnToggleColor = Color.DarkGreen,
                OffToggleColor = Color.PaleVioletRed,
            };

            button.CheckedChanged += CheckedChangedEventHandler;

            SettingsUtils.AddContextualMenu(this.button, value);
        }

        private void CheckedChangedEventHandler(object? sender, EventArgs args)
        {
            this.Value.SetConfigurationValue(this.button.Checked);
        }

        public override Control GetControl()
        {
            return this.button;
        }
    }
}
