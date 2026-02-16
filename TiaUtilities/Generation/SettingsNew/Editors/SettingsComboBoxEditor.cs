using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsComboBoxEditor : SettingsEditor
    {
        private readonly RJComboBox comboBox;
        public SettingsComboBoxEditor(SettingsValue value) : base(value)
        {
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
                MinimumSize = new Size(150, 0)
            };

            switch (value.ValueBinding.EditorType)
            {
                case SettingsEditorTypeEnum.ENUM:
                    this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 

                    var enumValues = Enum.GetValues(value.PropertyInfo.PropertyType);

                    this.comboBox.DisplayMember = "Text";
                    this.comboBox.ValueMember = "Value";

                    var dataSourceItems = new List<object>();
                    foreach (Enum enumItem in enumValues)
                    {
                        dataSourceItems.Add(new { Text = enumItem.GetTranslation(), Value = enumItem });
                    }
                    this.comboBox.DataSource = dataSourceItems;

                    this.comboBox.SelectedValue = this.Value.GetConfigurationValue();
                    comboBox.OnSelectedIndexChanged += (sender, args) =>
                    {
                        var selectedValue = this.comboBox.SelectedValue;
                        if(selectedValue != null)
                        {
                            this.Value.SetConfigurationValue(selectedValue);
                        }
                    };

                    break;
                case SettingsEditorTypeEnum.STRING:
                    this.comboBox.Text = "" + this.Value.GetConfigurationValue();
                    this.comboBox.TextChanged += (sender, args) =>
                    {
                        value.SetConfigurationValue(this.comboBox.Text);
                    };
                    break;
                case SettingsEditorTypeEnum.INT:
                    this.comboBox.Text = "" + this.Value.GetConfigurationValue();
                    this.comboBox.KeyPress += SignedKeyPressEventHandler;
                    this.comboBox.TextChanged += (sender, args) =>
                    {
                        var ret = long.TryParse(this.comboBox.Text, out var signedValue);
                        if (ret)
                        {
                            value.SetConfigurationValue(signedValue);
                        }

                    };
                    break;
                case SettingsEditorTypeEnum.UINT:
                    this.comboBox.Text = "" + this.Value.GetConfigurationValue();
                    this.comboBox.KeyPress += UnsignedKeyPressEventHandler;
                    this.comboBox.TextChanged += (sender, args) =>
                    {
                        var ret = ulong.TryParse(this.comboBox.Text, out var signedValue);
                        if (ret)
                        {
                            value.SetConfigurationValue(signedValue);
                        }
                    };
                    break;
            }
            
            SettingsUtils.AddContextualMenu(this.comboBox, value);
        }

        private void StringTextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = comboBox.Text;
            this.Value.SetConfigurationValue(text);
        }

        private void SignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            var isKeyValid = char.IsLetter(args.KeyChar) || args.KeyChar == '+' || args.KeyChar == '-';
            args.Handled = isKeyValid;
        }

        private void UnsignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            var isKeyValid = char.IsLetter(args.KeyChar) || args.KeyChar == '+' || args.KeyChar == '-';
            args.Handled = isKeyValid;
        }

        public override Control GetControl()
        {
            return comboBox;
        }
    }
}
