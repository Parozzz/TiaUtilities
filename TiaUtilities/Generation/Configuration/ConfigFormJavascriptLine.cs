using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Javascript;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormJavascriptLine : ConfigFormLine<ConfigFormJavascriptLine>
    {

        private readonly JavascriptFCTB jsFCTB;
        private readonly FastColoredTextBox control;

        private Action<string> textChangedAction;

        public ConfigFormJavascriptLine()
        {
            this.jsFCTB = new JavascriptFCTB();
            jsFCTB.InitControl();

            this.control = jsFCTB.GetFCTB();
            this.control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object sender, EventArgs args)
        {
            var text = this.control.Text;
            textChangedAction?.Invoke(text);
        }

        public ConfigFormJavascriptLine ControlText(IConvertible value)
        {
            this.control.Text = (value ?? "").ToString();
            this.control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigFormJavascriptLine TextChanged(Action<string> action)
        {
            this.textChangedAction = action;
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
