using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormJSONLine : ConfigFormLine<ConfigFormJSONLine>
    {
        private static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();
        public static RegexOptions RegexCompiledOption
        {
            get
            {
                return platformType == Platform.X86 ? RegexOptions.Compiled : RegexOptions.None;
            }
        }

        private readonly FastColoredTextBox control;

        private Action<string> textChangedAction;

        public ConfigFormJSONLine()
        {
            this.control = new FastColoredTextBox();
            this.control.Language = FastColoredTextBoxNS.Text.Language.JSON;

            this.control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object sender, EventArgs args)
        {
            var text = this.control.Text;
            textChangedAction?.Invoke(text);
        }

        public ConfigFormJSONLine ControlText(IConvertible value)
        {
            this.control.Text = (value ?? "").ToString();
            this.control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigFormJSONLine TextChanged(Action<string> action)
        {
            this.textChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
