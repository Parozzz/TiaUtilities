using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJavascriptLine : ConfigLine<ConfigJavascriptLine>
    {

        private readonly JavascriptFCTB jsFCTB;
        private readonly FastColoredTextBox control;

        private Action<string>? textChangedAction;

        public ConfigJavascriptLine()
        {
            jsFCTB = new JavascriptFCTB();
            jsFCTB.InitControl();

            control = jsFCTB.GetFCTB();
            control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = control.Text;
            textChangedAction?.Invoke(text);
        }

        public override ConfigJavascriptLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigJavascriptLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public JavascriptFCTB GetJavascriptFCTB()
        {
            return jsFCTB;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
