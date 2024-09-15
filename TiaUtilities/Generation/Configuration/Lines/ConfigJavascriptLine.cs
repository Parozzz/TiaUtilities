using FastColoredTextBoxNS;
using System.Linq.Expressions;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJavascriptLine : ConfigLine<ConfigJavascriptLine>
    {
        private readonly IConfigGroup configGroup;

        private readonly JavascriptEditor jsFCTB;
        private readonly FastColoredTextBox control;

        private Action<string>? textChangedAction;

        public ConfigJavascriptLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;

            jsFCTB = new JavascriptEditor();
            jsFCTB.InitControl();

            control = jsFCTB.GetTextBox();
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
        
        public ConfigJavascriptLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration);

            this.ControlText(propertyExpression.Compile().Invoke());
            textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            return this;
        }

        public JavascriptEditor GetJavascriptFCTB()
        {
            return jsFCTB;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
