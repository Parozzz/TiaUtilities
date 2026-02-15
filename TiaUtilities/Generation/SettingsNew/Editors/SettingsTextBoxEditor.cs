namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsTextBoxEditor : SettingsEditor
    {
        private readonly TextBox textBox;
        public SettingsTextBoxEditor(SettingsValue value) : base(value)
        {
            this.textBox = new()
            {
                Text = "" + value.GetConfigurationValue(),
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = false,
                MaximumSize = new Size(150, 0),
                Margin = new Padding(3, 0, 0, 0), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                Font = SettingsConstants.SETTINGS_VALUE_TEXTBOX_FONT,
                BackColor = Form.DefaultBackColor,
            };

            var type = value.Binding.EditorType;
            switch (type)
            {
                case SettingsEditorTypeEnum.STRING:
                    this.textBox.TextChanged += (sender, args) =>
                    {
                        value.SetConfigurationValue(this.textBox.Text);
                    };
                    break;
                case SettingsEditorTypeEnum.INT:
                    this.textBox.KeyPress += SignedKeyPressEventHandler;
                    this.textBox.TextChanged += (sender, args) =>
                    {
                        var ret = long.TryParse(this.textBox.Text, out var signedValue);
                        if (ret)
                        {
                            value.SetConfigurationValue(signedValue);
                        }

                    };
                    break;
                case SettingsEditorTypeEnum.UINT:
                    this.textBox.KeyPress += UnsignedKeyPressEventHandler;
                    this.textBox.TextChanged += (sender, args) =>
                    {
                        var ret = ulong.TryParse(this.textBox.Text, out var signedValue);
                        if (ret)
                        {
                            value.SetConfigurationValue(signedValue);
                        }
                    };
                    break;
            }
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
            return textBox;
        }
    }
}
