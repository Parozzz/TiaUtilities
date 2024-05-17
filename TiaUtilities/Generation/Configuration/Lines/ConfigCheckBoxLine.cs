using System;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigCheckBoxLine : ConfigLine<ConfigCheckBoxLine>
    {
        private readonly CheckBox control;

        private Action<bool>? checkedChangedAction;

        public ConfigCheckBoxLine()
        {
            control = new CheckBox
            {
                FlatStyle = FlatStyle.Flat,
                Text = "",
                AutoSize = true,
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
