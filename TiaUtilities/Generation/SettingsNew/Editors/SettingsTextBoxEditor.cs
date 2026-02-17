using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsTextBoxEditor : SettingsEditor
    {
        private const int TEXT_BOX_MIN_WIDTH = 350;

        private readonly TextBox textBox;

        public SettingsTextBoxEditor(SettingsValue value) : base(value)
        {
            var size = TextRenderer.MeasureText("g", SettingsConstants.VALUE_CONTROL_FONT, Size.Empty, TextFormatFlags.TextBoxControl);
            this.textBox = new()
            {
                Text = "" + value.GetConfigurationValue(),
                Font = SettingsConstants.VALUE_CONTROL_FONT,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = false,
                MinimumSize = new Size(TEXT_BOX_MIN_WIDTH, size.Height + 8), //Padding does not work :'(
                Margin = new Padding(3, 0, 0, 0), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                BackColor = Form.DefaultBackColor,
            };

            var type = value.ValueBinding.EditorType;
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

            var _ = SettingsUtils.AddContextualMenu(this.textBox, value);
        }

        private void SignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            var isKeyValid = char.IsLetter(args.KeyChar) && args.KeyChar != '+' && args.KeyChar != '-';
            args.Handled = isKeyValid;
        }

        private void UnsignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            var isKeyValid = char.IsLetter(args.KeyChar);
            args.Handled = isKeyValid;
        }

        public override Control GetControl()
        {
            return this.textBox;
        }
    }
}
