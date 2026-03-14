using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsTextBoxEditor : SettingsEditor
    {
        private const int TEXT_BOX_MIN_WIDTH = 350;

        private readonly TextBox textBox;

        public SettingsTextBoxEditor(SettingsFormValueImpl value) : base(value)
        {
            var size = TextRenderer.MeasureText("AaGg", SettingsFormConstants.VALUE_CONTROL_FONT, Size.Empty, TextFormatFlags.TextBoxControl);
            this.textBox = new()
            {
                Font = SettingsFormConstants.VALUE_CONTROL_FONT,
                Anchor = AnchorStyles.Left | AnchorStyles.Right, //This allows centering if no label is present!
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = false,
                MinimumSize = new Size(TEXT_BOX_MIN_WIDTH, size.Height + 8), //Padding does not work :'(
                Margin = new Padding(3), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                BackColor = Form.DefaultBackColor,
            };

            var type = value.Binding.EditorType;
            switch (type)
            {
                case SettingsEditorTypeEnum.STRING:
                    this.textBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.INT:
                    this.textBox.KeyPress += SignedKeyPressEventHandler;
                    this.textBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
                case SettingsEditorTypeEnum.UINT:
                    this.textBox.KeyPress += UnsignedKeyPressEventHandler;
                    this.textBox.TextChanged += (sender, args) => this.SaveToConfiguration();
                    break;
            }

            var _ = SettingsFormUtils.AddContextualMenu(this.textBox, value);
        }

        private void SignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if(args.KeyChar == (char) Keys.Cancel ||  args.KeyChar == (char)Keys.Enter || args.KeyChar == (char)Keys.Back)
            {
                return;
            }

            var isKeyValid = char.IsNumber(args.KeyChar) || args.KeyChar == '+' || args.KeyChar == '-';
            args.Handled = !isKeyValid;
        }

        private void UnsignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if (args.KeyChar == (char)Keys.Cancel || args.KeyChar == (char)Keys.Enter || args.KeyChar == (char)Keys.Back)
            {
                return;
            }

            var isKeyInvalid = char.IsLetter(args.KeyChar);
            args.Handled = isKeyInvalid;
        }

        public override Control GetControl()
        {
            return this.textBox;
        }

        protected override Control GetControlForEvents()
        {
            return this.textBox;
        }

        public override void LoadFromConfiguration()
        {
            var configurationValue = this.Value.GetConfigurationValue();
            this.textBox.Text = "" + configurationValue;
        }

        public override void SaveToConfiguration()
        {
            switch (this.Value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.STRING:
                    this.Value.SetConfigurationValue(this.textBox.Text);
                    break;
                case SettingsEditorTypeEnum.INT:
                    if (long.TryParse(this.textBox.Text, out var signedValue))
                    {
                        this.Value.SetConfigurationValue(signedValue);
                    }
                    break;
                case SettingsEditorTypeEnum.UINT:
                    if (ulong.TryParse(this.textBox.Text, out var unsignedValue))
                    {
                        this.Value.SetConfigurationValue(unsignedValue);
                    }
                    break;
            }
        }
    }
}
