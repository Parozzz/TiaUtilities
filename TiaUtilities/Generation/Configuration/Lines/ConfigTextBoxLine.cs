using CustomControls.RJControls;
using System.Linq.Expressions;
using System.Reflection;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigTextBoxLine : ConfigLine<ConfigTextBoxLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly RJTextBox textBox;

        private bool numericOnly;
        private Action<string?>? textChangedAction;
        private Action<uint>? uintChangedAction;

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
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration);

            this.ControlText(propertyExpression.Compile().Invoke());
            textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            return this;
        }

        public ConfigTextBoxLine BindUInt(Expression<Func<uint>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup,propertyExpression.Body, out object configuration);

            this.ControlText(propertyExpression.Compile().Invoke());
            uintChangedAction = uintValue => propertyInfo.SetValue(configuration, uintValue);
            return this;
        }
        
        public override Control GetControl()
        {
            return this.textBox;
        }
    }
}
