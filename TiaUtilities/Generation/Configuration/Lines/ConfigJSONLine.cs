using System.Linq.Expressions;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Editors.Json;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJSONLine : ConfigLine<ConfigJSONLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly JsonEditor editor;

        private Action<string>? textChangedAction;
        private Action? transferToOtherTextAction;

        public ConfigJSONLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;

            this.editor = new JsonEditor();
            this.editor.InitControl();

            var control = editor.GetControl();
            control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            textChangedAction?.Invoke(this.editor.Text);
        }

        public ConfigJSONLine Readonly()
        {
            var control = this.editor.GetControl();
            control.ReadOnly = true;
            control.BackColor = SystemColors.Control;
            return this;
        }

        public override ConfigJSONLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            this.editor.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigJSONLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigJSONLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            this.transferToOtherTextAction = () =>
            {
                var str = this.editor.Text;
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

        public ConfigJSONLine RegisterErrorThreadWithForm(ErrorReportThread errorThread, Form form)
        {
            return this;
        }

        public override Control GetControl()
        {
            return this.editor.GetControl();
        }
    }
}
