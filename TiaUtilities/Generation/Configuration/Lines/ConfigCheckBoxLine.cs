using CustomControls.RJControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigCheckBoxLine : ConfigLine<ConfigCheckBoxLine>
    {
        private readonly RJToggleButton control;

        private Action<bool>? checkedChangedAction;

        public ConfigCheckBoxLine()
        {
            control = new RJToggleButton
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
            control.CheckedChanged += CheckedChangedEventHandler;
        }

        private void CheckedChangedEventHandler(object? sender, EventArgs args)
        {
            checkedChangedAction?.Invoke(control.Checked);
        }

        public ConfigCheckBoxLine Value(bool value)
        {
            control.Checked = value;
            return this;
        }

        public ConfigCheckBoxLine CheckedChanged(Action<bool> action)
        {
            checkedChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
