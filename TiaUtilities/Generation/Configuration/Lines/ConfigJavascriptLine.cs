using System.Linq.Expressions;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Editors;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJavascriptLine : ConfigLine<ConfigJavascriptLine>
    {
        private readonly IConfigGroup configGroup;

        private readonly JavascriptEditor jsEditor;

        private Action<string>? textChangedAction;
        private Action? transferToOtherTextAction;

        public ConfigJavascriptLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;

            this.jsEditor = new JavascriptEditor();
            this.jsEditor.InitControl();
            this.jsEditor.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = this.jsEditor.Text;
            textChangedAction?.Invoke(text);
        }

        public override ConfigJavascriptLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            this.jsEditor.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }
        
        public ConfigJavascriptLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }
        
        public ConfigJavascriptLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            this.transferToOtherTextAction = () =>
            {
                var str = this.jsEditor.Text;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, str);
                }
            };
            return this;
        }

        public override void TrasferToAllConfigurations()
        {
            transferToOtherTextAction?.Invoke();
        }

        public JavascriptEditor GetEditor()
        {
            return jsEditor;
        }

        public override Control GetControl() => this.jsEditor.GetControl();
    }
}
