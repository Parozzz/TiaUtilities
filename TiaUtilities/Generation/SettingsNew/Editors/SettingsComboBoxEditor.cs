using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.SettingsNew.Bindings;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsComboBoxEditor : SettingsEditor
    {
        private readonly RJComboBox comboBox;
        public SettingsComboBoxEditor(SettingsFormValueImpl value) : base(value)
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
                MinimumSize = new Size(150, 0),
                Anchor = AnchorStyles.Left,  //This allows centering if no label is present!
            };

            switch (value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.ENUM:
                    if(value.PropertyInfo.PropertyType.IsAssignableFrom(typeof(Enum)))
                    {
                        throw new InvalidCastException($"Using SettingsEditorTypeEnum ENUM with a PropertyInfo that is not an Enumeration for {value.PropertyInfo.Name}");
                    }

                    this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
                    this.comboBox.DisplayMember = "Text";
                    this.comboBox.ValueMember = "Value";

                    var enumDataSourceItems = new List<object>();
                    foreach (Enum enumItem in Enum.GetValues(value.PropertyInfo.PropertyType))
                    {
                        enumDataSourceItems.Add(new { Text = enumItem.GetTranslation(), Value = enumItem });
                    }
                    this.comboBox.DataSource = enumDataSourceItems;

                    this.comboBox.OnSelectedIndexChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.LIST:
                    if(value.Binding.Tag is not SettingsValueListTag listTag)
                    {
                        throw new InvalidCastException($"Using SettingsEditorTypeEnum LIST without a Tag that is not SettingsValueListTag for {value.PropertyInfo.Name}");
                    }

                    this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
                    this.comboBox.DisplayMember = "Text";
                    this.comboBox.ValueMember = "Value";

                    var listDataSourceItems = new List<object>();
                    foreach (var listValue in listTag.List)
                    {
                        listDataSourceItems.Add(new { Text = listValue, Value = listValue });
                    }
                    this.comboBox.DataSource = listDataSourceItems;

                    this.comboBox.OnSelectedIndexChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.STRING:
                    this.comboBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.INT:
                    this.comboBox.KeyPress += SignedKeyPressEventHandler;
                    this.comboBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.UINT:
                    this.comboBox.KeyPress += UnsignedKeyPressEventHandler;
                    this.comboBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
            }

            var _ = SettingsUtils.AddContextualMenu(this.comboBox, value);

            base.RegisterPropertyChanged(this.comboBox);
            this.LoadFromConfiguration();
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

        public override void LoadFromConfiguration()
        {
            switch (base.Value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.ENUM:
                    this.comboBox.SelectedValue = this.Value.GetConfigurationValue();
                    break;
                case SettingsEditorTypeEnum.STRING:
                case SettingsEditorTypeEnum.LIST:
                case SettingsEditorTypeEnum.INT:
                case SettingsEditorTypeEnum.UINT:
                    this.comboBox.Text = "" + this.Value.GetConfigurationValue();
                    break;
            }
        }

        public override void SaveToConfiguration()
        {
            switch (this.Value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.ENUM:
                case SettingsEditorTypeEnum.LIST:
                    var selectedValue = this.comboBox.SelectedValue;
                    if (selectedValue != null)
                    {
                        this.Value.SetConfigurationValue(selectedValue);
                    }
                    break;
                case SettingsEditorTypeEnum.STRING:
                    this.Value.SetConfigurationValue(this.comboBox.Text);
                    break;
                case SettingsEditorTypeEnum.INT:
                    if (long.TryParse(this.comboBox.Text, out var signedValue))
                    {
                        this.Value.SetConfigurationValue(signedValue);
                    }
                    break;
                case SettingsEditorTypeEnum.UINT:
                    if (ulong.TryParse(this.comboBox.Text, out var unsignedValue))
                    {
                        this.Value.SetConfigurationValue(unsignedValue);
                    }
                    break;
            }
        }
    }
}
