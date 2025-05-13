using System.Linq.Expressions;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration.Utility;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigTextBoxLine : ConfigLine<ConfigTextBoxLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly RJTextBox textBox;

        private bool numericOnly;
        private Action<string?>? textChangedAction;
        private Action<uint>? uintChangedAction;
        private Action? transferToOtherTextAction;
        private Action? transferToOtherUIntAction;

        public ConfigTextBoxLine(IConfigGroup group)
        {
            this.configGroup = group;
            this.textBox = new RJTextBox()
            {
                Margin = new Padding(0),
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                BorderColor = ConfigStyle.DETAIL_COLOR_DARK,
                BorderFocusColor = ConfigStyle.DETAIL_COLOR_DARKDARK,
                BorderStyle = BorderStyle.None,
                Underlined = true,
                UnderlineColor = ConfigStyle.UNDERLINE_COLOR,
                TextLeftPadding = 3,
            };
            this.textBox.TextChanged += TextChangedEventHandler;
            this.textBox.KeyPress += KeyPressEventHandler;
        }

        private void KeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if (numericOnly)
            {
                args.Handled = char.IsLetter(args.KeyChar);
            }
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = this.textBox.Text;
            textChangedAction?.Invoke(text);

            if (uintChangedAction != null && uint.TryParse(text, out uint result))
            {
                uintChangedAction.Invoke(result);
            }
        }

        public ConfigTextBoxLine Readonly()
        {
            this.textBox.Underlined = false;
            this.textBox.ReadOnly = true;
            return this;
        }

        public ConfigTextBoxLine Multiline()
        {
            this.textBox.Multiline = true;
            this.textBox.ScrollBars = ScrollBars.Both;
            return this;
        }

        public ConfigTextBoxLine TextChanged(Action<string?> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigTextBoxLine TextChangedNotNull(Action<string> action)
        {
            textChangedAction = str => action.Invoke(str ?? "");
            return this;
        }

        public ConfigTextBoxLine UIntChanged(Action<uint> action)
        {
            numericOnly = true;
            uintChangedAction = action;
            return this;
        }

        public ConfigTextBoxLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            this.transferToOtherTextAction = () =>
            {
                var str = this.textBox.Text;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, str);
                }
            };
            return this;
        }

        public ConfigTextBoxLine BindUInt(Expression<Func<uint>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.uintChangedAction = uintValue => propertyInfo.SetValue(configuration, uintValue);
            this.transferToOtherUIntAction = () =>
            {
                var str = this.textBox.Text;
                if (uintChangedAction != null && uint.TryParse(str, out uint result))
                {
                    foreach (var otherConfig in otherConfigurations)
                    {
                        propertyInfo.SetValue(otherConfig, result);
                    }
                }
            };
            return this;
        }

        public override void TrasferToAllConfigurations()
        {
            transferToOtherTextAction?.Invoke();
            transferToOtherUIntAction?.Invoke();
        }


        public override Control GetControl()
        {
            return this.textBox;
        }
    }
}
