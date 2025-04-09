using System.Linq.Expressions;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigComboBoxLine : ConfigLine<ConfigComboBoxLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly RJComboBox comboBox;

        private bool numericOnly;
        private Action<string>? textChangedAction;
        private Action<uint>? uintChangedAction;
        private Action<object?>? selectedValueChangedAction;
        private Action? transferToOtherTextAction;
        private Action? transferToOtherUIntAction;
        private Action? transferToOtherObjectAction;

        public ConfigComboBoxLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;
            this.comboBox = new RJComboBox()
            {
                Margin = Padding.Empty,
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                IconBackColor = ConfigStyle.DETAIL_COLOR_DARK,
                IconColor = ConfigStyle.DETAIL_COLOR_DARKDARK,
                BorderStyle = BorderStyle.None,
                Underlined = true,
                UnderlineColor = ConfigStyle.UNDERLINE_COLOR,
            };

            comboBox.OnSelectedIndexChanged += OnSelectedIndexChanged;
            comboBox.TextChanged += TextChangedEventHandler;
            comboBox.KeyPress += KeyPressEventHandler;
        }

        private void OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            selectedValueChangedAction?.Invoke(comboBox.SelectedValue);
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
            var text = comboBox.Text;
            textChangedAction?.Invoke(text);

            if (uintChangedAction != null && uint.TryParse(text, out uint result))
            {
                uintChangedAction.Invoke(result);
            }
        }

        public ConfigComboBoxLine DisableEdit()
        {
            this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            return this;
        }

        public ConfigComboBoxLine Items(object[] items)
        {
            comboBox.Items.AddRange(items);
            return this;
        }

        public ConfigComboBoxLine TranslatableEnumItems<T>() where T : Enum
        {
            this.comboBox.DisplayMember = "Text";
            this.comboBox.ValueMember = "Value";

            var dataSourceItems = new List<object>();
            foreach (Enum enumItem in Enum.GetValues(typeof(T)))
            {
                dataSourceItems.Add(new { Text = enumItem.GetTranslation(), Value = enumItem });
            }
            this.comboBox.DataSource = dataSourceItems;

            return this;
        }

        public ConfigComboBoxLine SelectedValue(object item)
        {
            comboBox.SelectedValue = item;
            return this;
        }

        public ConfigComboBoxLine SelectedValueChanged(Action<object?> action)
        {
            this.selectedValueChangedAction = action;
            return this;
        }
        
        public ConfigComboBoxLine SelectedValueChanged<T>(Action<T> action)
        {
            this.selectedValueChangedAction = obj =>
            {
                if (obj is T t)
                {
                    action(t);
                }
            };
            return this;
        }
        
        public ConfigComboBoxLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigComboBoxLine UIntChanged(Action<uint> action)
        {
            uintChangedAction = action;
            return this;
        }
        
        public ConfigComboBoxLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            this.transferToOtherTextAction = () =>
            {
                var str = this.comboBox.Text;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, str);
                }
            };
            return this;
        }

        public ConfigComboBoxLine BindUInt(Expression<Func<uint>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.uintChangedAction = uintValue => propertyInfo.SetValue(configuration, uintValue);
            this.transferToOtherUIntAction = () =>
            {
                var str = this.comboBox.Text;
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

        public ConfigComboBoxLine BindValue<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.comboBox.SelectedValue = propertyExpression.Compile().Invoke();
            this.selectedValueChangedAction = obj =>
            {
                if (obj is T tValue)
                {
                    propertyInfo.SetValue(configuration, tValue);
                }
            };
            this.transferToOtherTextAction = () =>
            {
                var objectValue = this.comboBox.SelectedValue;
                if(objectValue is T tValue)
                {
                    foreach (var otherConfig in otherConfigurations)
                    {
                        propertyInfo.SetValue(otherConfig, tValue);
                    }
                }
            };
            return this;
        }
        public override void TrasferToAllConfigurations()
        {
            transferToOtherTextAction?.Invoke();
            transferToOtherUIntAction?.Invoke();
            transferToOtherTextAction?.Invoke();
        }

        public ConfigComboBoxLine NumericOnly()
        {
            numericOnly = true;
            return this;
        }

        public override Control GetControl()
        {
            return comboBox;
        }
    }
}
