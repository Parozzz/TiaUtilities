using System.ComponentModel;
using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsTextBoxEditor : SettingsEditor
    {
        private const int TEXT_BOX_MIN_WIDTH = 350;

        private readonly TextBox textBox;

        public SettingsTextBoxEditor(SettingsValue value) : base(value)
        {
            var size = TextRenderer.MeasureText("AaGg", SettingsConstants.VALUE_CONTROL_FONT, Size.Empty, TextFormatFlags.TextBoxControl);
            this.textBox = new()
            {
                Font = SettingsConstants.VALUE_CONTROL_FONT,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = false,
                MinimumSize = new Size(TEXT_BOX_MIN_WIDTH, size.Height + 8), //Padding does not work :'(
                Margin = new Padding(3, 0, 0, 3), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                BackColor = Form.DefaultBackColor,
            };

            var type = value.ValueBinding.EditorType;
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

            var _ = SettingsUtils.AddContextualMenu(this.textBox, value);

            base.RegisterPropertyChanged(this.textBox);
            this.LoadFromConfiguration();
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

        public override void LoadFromConfiguration()
        {
            this.textBox.Text = "" + this.Value.GetConfigurationValue();
        }

        public override void SaveToConfiguration()
        {
            switch (this.Value.ValueBinding.EditorType)
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
