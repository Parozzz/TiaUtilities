using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigTextBoxLine : ConfigLine<ConfigTextBoxLine>
    {
        private readonly TextBox control;

        private bool numericOnly;
        private Action<string>? textChangedAction;
        private Action<uint>? uintChangedAction;

        public ConfigTextBoxLine()
        {
            this.control = new FlatTextBox()
            {
                Margin = new Padding(0),
            };
            this.control.TextChanged += TextChangedEventHandler;
            this.control.KeyPress += KeyPressEventHandler;
        }

        private void KeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if (numericOnly)
            {
                args.Handled = char.IsLetter(args.KeyChar);
            }
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = this.control.Text;
            textChangedAction?.Invoke(text);

            if (uintChangedAction != null && uint.TryParse(text, out uint result))
            {
                uintChangedAction.Invoke(result);
            }
        }

        public ConfigTextBoxLine Readonly()
        {
            this.control.ReadOnly = true;
            return this;
        }

        public ConfigTextBoxLine Multiline()
        {
            this.control.Multiline = true;
            this.control.ScrollBars = ScrollBars.Both;
            return this;
        }
        public ConfigTextBoxLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigTextBoxLine UIntChanged(Action<uint> action)
        {
            numericOnly = true;
            uintChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return this.control;
        }
    }
}
