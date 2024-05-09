using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormTextBoxLine : ConfigFormLine<ConfigFormTextBoxLine>
    {
        private readonly TextBox control;

        private bool numericOnly;
        private Action<string> textChangedAction;
        private Action<uint> uintChangedAction;

        public ConfigFormTextBoxLine()
        {
            this.control = new FlatTextBox()
            {
                Margin = new Padding(0)
            };
            this.control.TextChanged += TextChangedEventHandler;
            this.control.KeyPress += KeyPressEventHandler;
        }

        private void KeyPressEventHandler(object sender, KeyPressEventArgs args)
        {
            if(numericOnly)
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

        public ConfigFormTextBoxLine ControlText(IConvertible value)
        {
            this.control.Text = value.ToString();
            return this;
        }

        public ConfigFormTextBoxLine TextChanged(Action<string> action)
        {
            this.textChangedAction = action;
            return this;
        }

        public ConfigFormTextBoxLine UIntChanged(Action<uint> action)
        {
            numericOnly = true;
            this.uintChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
