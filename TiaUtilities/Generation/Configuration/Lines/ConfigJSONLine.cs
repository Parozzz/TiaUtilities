using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJSONLine : ConfigLine<ConfigJSONLine>
    {
        private readonly FastColoredTextBox control;

        private Action<string>? textChangedAction;

        public ConfigJSONLine()
        {
            control = new FastColoredTextBox
            {
                Language = Language.JSON,
                // == INDENTATION ==
                AutoIndent = true,
                AutoIndentExistingLines = true,
                AutoIndentChars = true,
                TabLength = 4,
                // == LINE NUMBERS ==
                ShowLineNumbers = false,
                LineNumberStartValue = 0,
                // == CARET ==
                CaretVisible = true,
                CaretBlinking = true,
                ShowCaretWhenInactive = true,
                WideCaret = false,

                CharHeight = 16, //Default 14
                LineInterval = 4, //Default 0

                AcceptsTab = true,
                AcceptsReturn = true,
                ShowFoldingLines = true,
            };


            control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = control.Text;
            textChangedAction?.Invoke(text);
        }
        public ConfigJSONLine Readonly()
        {
            control.ReadOnly = true;
            control.BackColor = SystemColors.Control;
            return this;
        }

        public override ConfigJSONLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigJSONLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
