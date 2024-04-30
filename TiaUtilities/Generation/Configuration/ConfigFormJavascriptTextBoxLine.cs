using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Javascript;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormJavascriptTextBoxLine : ConfigFormLine
    {

        private readonly JavascriptFCTB jsFCTB;
        private readonly FastColoredTextBox control;

        private Action<string> textChangedAction;

        public ConfigFormJavascriptTextBoxLine(string labelText, int height = 0) : base(labelText, height)
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

        public JavascriptFCTB GetJSFCTB()
        {
            return jsFCTB;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
