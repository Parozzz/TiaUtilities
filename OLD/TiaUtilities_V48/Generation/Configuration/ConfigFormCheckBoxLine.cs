﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormCheckBoxLine : ConfigFormLine<ConfigFormCheckBoxLine>
    {
        private readonly CheckBox control;

        private Action<bool> checkedChangedAction;

        public ConfigFormCheckBoxLine()
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
