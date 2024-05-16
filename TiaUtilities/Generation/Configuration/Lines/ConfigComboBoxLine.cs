using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigComboBoxLine : ConfigLine<ConfigComboBoxLine>
    {
        private readonly ComboBox control;

        private bool numericOnly;
        private Action<string> textChangedAction;
        private Action<uint> uintChangedAction;

        public ConfigComboBoxLine()
        {
            control = new FlatComboBox();
            control.TextChanged += TextChangedEventHandler;
            control.KeyPress += KeyPressEventHandler;
        }

        private void KeyPressEventHandler(object sender, KeyPressEventArgs args)
        {
            if (numericOnly)
            {
                args.Handled = char.IsLetter(args.KeyChar);
            }
        }

        private void TextChangedEventHandler(object sender, EventArgs args)
        {
            var text = control.Text;
            textChangedAction?.Invoke(text);

            if (uintChangedAction != null && uint.TryParse(text, out uint result))
            {
                uintChangedAction.Invoke(result);
            }
        }

        public ConfigComboBoxLine DisableEdit()
        {
            this.control.DropDownStyle = ComboBoxStyle.DropDownList;
            return this;
        }

        public ConfigComboBoxLine Items(object[] items)
        {
            control.Items.AddRange(items);
            return this;
        }

        public ConfigComboBoxLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigComboBoxLine UIntChanged(Action<uint> action)
        {
            uintChangedAction = action;
            return this;
        }

        public ConfigComboBoxLine NumericOnly()
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
