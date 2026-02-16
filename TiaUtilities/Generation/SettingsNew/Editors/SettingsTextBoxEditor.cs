using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsTextBoxEditor : SettingsEditor
    {
        private const int OPEN_TEXT_AREA_SIZE = 30;
        private const int TEXT_BOX_MIN_WIDTH = 350;

        private readonly TextBox textBox;
        private readonly Button openTextAreaButton;

        public SettingsTextBoxEditor(SettingsValue value) : base(value)
        {
            this.textBox = new()
            {
                Text = "" + value.GetConfigurationValue(),
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = false,
                MinimumSize = new Size(TEXT_BOX_MIN_WIDTH, 0),
                Margin = new Padding(3, 0, 0, 0), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                Font = SettingsConstants.SETTINGS_VALUE_TEXTBOX_FONT,
                BackColor = Form.DefaultBackColor,
            };

            this.openTextAreaButton = new()
            {
                Text = ">",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                MaximumSize = new Size(OPEN_TEXT_AREA_SIZE, OPEN_TEXT_AREA_SIZE),
                Margin = new Padding(3, 0, 0, 0), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                Font = SettingsConstants.SETTINGS_VALUE_TEXTBOX_FONT,
                BackColor = Form.DefaultBackColor,
                FlatStyle = FlatStyle.Flat,
                UseCompatibleTextRendering = true
            };
            this.openTextAreaButton.FlatAppearance.BorderSize = 0;
            this.openTextAreaButton.FlatAppearance.BorderColor = Color.Black;
            this.openTextAreaButton.FlatAppearance.MouseOverBackColor = Color.Transparent;

            this.openTextAreaButton.Click += (sender, args) =>
            {
                var floatingTextBox = new FloatingTextBox(this.textBox.Text);

                var point = this.openTextAreaButton.PointToScreen(Point.Empty);

                var dialogResult = floatingTextBox.ShowDialogAtPosition(point.X + OPEN_TEXT_AREA_SIZE + 2, point.Y - OPEN_TEXT_AREA_SIZE/4);
                if(dialogResult == DialogResult.OK)
                {
                    this.textBox.Text = floatingTextBox.InputText;
                }
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

            SettingsUtils.AddContextualMenu(this.textBox, value);
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
            var panel = new TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                ColumnStyles =
                {
                    new(SizeType.AutoSize),
                    new(SizeType.AutoSize)
                },
                RowStyles =
                {
                    new(SizeType.AutoSize)
                },
            };
            
            panel.Controls.Add(this.textBox, 0, 0);
            //panel.Controls.Add(this.colorNameComboBox, 1, 0);
            panel.Controls.Add(this.openTextAreaButton, 1, 0);

            return panel;
        }
    }
}
