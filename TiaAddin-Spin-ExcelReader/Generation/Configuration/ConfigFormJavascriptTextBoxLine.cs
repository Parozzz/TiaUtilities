using FastColoredTextBoxNS;
using System;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormJavascriptTextBoxLine : ConfigFormLine
    {
        private readonly FastColoredTextBox control;

        private Action<string> textChangedAction;

        public ConfigFormJavascriptTextBoxLine(string labelText, int height = 0) : base(labelText, height)
        {
            this.control = new FastColoredTextBox();
            this.control.TextChanged += TextChangedEventHandler;
            this.control.LeftBracket = '(';
            this.control.RightBracket = ')';
            this.control.LeftBracket2 = '{';
            this.control.RightBracket2 = '}';
            this.control.AutoIndent = true;
            this.control.AutoIndentExistingLines = true;
            this.control.AutoIndentChars = true;
            this.control.TabLength = 2;
            this.control.Language = Language.JS;
            this.control.HighlightingRangeType = HighlightingRangeType.AllTextRange;
        }

        private void TextChangedEventHandler(object sender, EventArgs args)
        {
            var text = this.control.Text;
            textChangedAction?.Invoke(text);
        }

        public ConfigFormJavascriptTextBoxLine ControlText(IConvertible value)
        {
            this.control.Text = (value ?? "").ToString();
            return this;
        }

        public ConfigFormJavascriptTextBoxLine TextChanged(Action<string> action)
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
