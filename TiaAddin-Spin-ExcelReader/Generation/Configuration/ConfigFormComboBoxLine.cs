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
    public class ConfigFormComboBoxLine : ConfigFormLine
    {
        private readonly ComboBox control;

        private bool numericOnly;
        private Action<string> textChangedAction;
        private Action<uint> uintChangedAction;

        public ConfigFormComboBoxLine(string labelText, int height = 0) : base(labelText, height)
        {
            this.control = new FlatComboBox();
            this.control.TextChanged += TextChangedEventHandler;
            this.control.KeyPress += KeyPressEventHandler;
        }

        private void KeyPressEventHandler(object sender, KeyPressEventArgs args)
        {
            if (numericOnly)
            {
                args.Handled = Char.IsLetter(args.KeyChar);
            }
        }

        private void TextChangedEventHandler(object sender, EventArgs args)
        {
            var text = this.control.Text;
            textChangedAction?.Invoke(text);

            if (uintChangedAction != null && uint.TryParse(text, out uint result))
            {
                uintChangedAction.Invoke(result);
            }
        }

        public ConfigFormComboBoxLine ControlText(IConvertible value)
        {
            this.control.Text = value.ToString();
            return this;
        }

        public ConfigFormComboBoxLine Items(object[] items)
        {
            this.control.Items.AddRange(items);
            return this;
        }

        public ConfigFormComboBoxLine TextChanged(Action<string> action)
        {
            this.textChangedAction = action;
            return this;
        }

        public ConfigFormComboBoxLine UIntChanged(Action<uint> action)
        {
            this.uintChangedAction = action;
            return this;
        }

        public ConfigFormComboBoxLine NumericOnly()
        {
            numericOnly = true;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
