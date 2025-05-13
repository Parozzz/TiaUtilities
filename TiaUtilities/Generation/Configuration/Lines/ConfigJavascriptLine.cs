using FastColoredTextBoxNS;
using System.Linq.Expressions;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Editors;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJavascriptLine : ConfigLine<ConfigJavascriptLine>
    {
        private readonly IConfigGroup configGroup;

        private readonly JavascriptEditor editor;
        private FastColoredTextBox Control {  get => editor.GetTextBox(); }

        private Action<string>? textChangedAction;
        private Action? transferToOtherTextAction;

        public ConfigJavascriptLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;

            this.editor = new JavascriptEditor();
            this.editor.InitControl();

            this.Control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = Control.Text;
            textChangedAction?.Invoke(text);
        }

        public override ConfigJavascriptLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            Control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
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
                var str = this.Control.Text;
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

        public ConfigJavascriptLine RegisterErrorThreadWithForm(ErrorReportThread errorThread, Form form)
        {
            this.editor.RegisterErrorReporter(errorThread);
            form.FormClosing += (sender, args) => this.editor.UnregisterErrorReporter(errorThread);
            return this;
        }

        public JavascriptEditor GetEditor()
        {
            return editor;
        }

        public override Control GetControl()
        {
            return Control;
        }
    }
}
