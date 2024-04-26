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
            // == BRACKETS ==
            this.control.AutoCompleteBrackets = true;
            this.control.AutoCompleteBracketsList = new char[] { '(', ')', '{', '}', '\"', '\"', '\'', '\'' };
            this.control.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            this.control.LeftBracket = '(';
            this.control.RightBracket = ')';
            this.control.LeftBracket2 = '{';
            this.control.RightBracket2 = '}';
            // == INDENTATION ==
            this.control.AutoIndent = true;
            this.control.AutoIndentExistingLines = true;
            this.control.AutoIndentChars = true;
            this.control.TabLength = 4;
            // == LINE NUMBERS ==
            this.control.ShowLineNumbers = true;
            this.control.LineNumberStartValue = 1;

            this.control.AcceptsTab = true;
            this.control.AcceptsReturn = true;
            this.control.ShowFoldingLines = false;

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
            this.control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
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
