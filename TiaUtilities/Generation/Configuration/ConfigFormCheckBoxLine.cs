using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormCheckBoxLine : ConfigFormLine
    {
        private readonly CheckBox control;

        private Action<bool> checkedChangedAction;

        public ConfigFormCheckBoxLine(string labelText, int height = 0) : base(labelText, height)
        {
            this.control = new CheckBox
            {
                FlatStyle = FlatStyle.Flat,
                Text = ""
            };
            this.control.CheckedChanged += CheckedChangedEventHandler;
        }

        private void CheckedChangedEventHandler(object sender, EventArgs args)
        {
            checkedChangedAction?.Invoke(this.control.Checked);
        }

        public ConfigFormCheckBoxLine Value(bool value)
        {
            this.control.Checked = value;
            return this;
        }

        public ConfigFormCheckBoxLine CheckedChanged(Action<bool> action)
        {
            this.checkedChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
