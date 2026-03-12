using System.Reflection;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.SettingsNew.FormHelpers;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew.Editors
{

    public class SettingsColorEditor : SettingsEditor
    {
        private enum LastColorChangeCauseEnum
        {
            NONE,
            LOAD,
            TEXT_BOX,
            COLOR_PICKER,
        };

        private record LastColorChange(Color Color, LastColorChangeCauseEnum Cause);

        private readonly static Dictionary<string, Color> COLOR_DICTIONARY = [];
        private static void ParseColors()
        {
            if (COLOR_DICTIONARY.Count > 0)
            {
                return;
            }

            foreach (KnownColor knownColorItem in Enum.GetValues(typeof(KnownColor)))
            {
                var color = Color.FromKnownColor(knownColorItem);
                COLOR_DICTIONARY.Add(color.Name, color);
            }
        }

        private readonly TextBox colorHexaTextBox;
        private readonly Button colorPickerButton;

        private LastColorChange lastColorChange = new(Color.White, LastColorChangeCauseEnum.NONE);

        private bool loading = false;

        public SettingsColorEditor(SettingsFormValueImpl value) : base(value)
        {
            this.colorHexaTextBox = new()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = false,
                MinimumSize = new Size(80, 0),
                MaximumSize = new Size(80, 0),
                Margin = new Padding(3, 0, 0, 0), //This is to align to the label since a the padding is automatically set to the left.
                Padding = Padding.Empty,
                Font = SettingsConstants.VALUE_CONTROL_FONT,
                BackColor = Form.DefaultBackColor,
            };
            Utils.SetDoubleBuffered(this.colorHexaTextBox);
            this.colorHexaTextBox.TextChanged += ColorTextBoxTextChangedEvent;

            this.colorPickerButton = new Button()
            {
                Dock = DockStyle.Fill,
                MinimumSize = new(40, 20),
                MaximumSize = new(40, 20),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = Padding.Empty,
                Margin = new Padding(2),
                FlatStyle = FlatStyle.Flat
            };
            Utils.SetDoubleBuffered(this.colorPickerButton);
            this.colorPickerButton.FlatAppearance.BorderColor = Color.Black;
            this.colorPickerButton.FlatAppearance.BorderSize = 1;

            this.colorPickerButton.Click += ColorPickerButtonClickEvent;

            var _ = SettingsUtils.AddContextualMenu(this.colorHexaTextBox, value);
            _ = SettingsUtils.AddContextualMenu(this.colorPickerButton, value);

            base.RegisterPropertyChanged(this.colorHexaTextBox);
            this.LoadFromConfiguration();
        }

        private void ColorTextBoxTextChangedEvent(object? sender, EventArgs e)
        {
            if(loading)
            {
                return;
            }

            try
            {
                var color = ColorTranslator.FromHtml(colorHexaTextBox.Text);
                this.lastColorChange = new(color, LastColorChangeCauseEnum.TEXT_BOX);

                this.colorPickerButton.BackColor = color;

                this.SaveToConfiguration();
            }
            catch { }
        }

        private void ColorPickerButtonClickEvent(object? sender, EventArgs e)
        {
            try
            {
                var colorDialog = new ColorDialog() { Color = lastColorChange.Color };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    this.lastColorChange = new(colorDialog.Color, LastColorChangeCauseEnum.COLOR_PICKER);

                    this.colorHexaTextBox.Text = colorDialog.Color.ToHexString();
                    this.colorPickerButton.BackColor = colorDialog.Color;

                    this.SaveToConfiguration();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        public override Control GetControl()
        {
            var panel = new TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                //Anchor = AnchorStyles.Left | AnchorStyles.Right,
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
            Utils.SetDoubleBuffered(panel);

            panel.Controls.Add(this.colorHexaTextBox, 0, 0);
            //panel.Controls.Add(this.colorNameComboBox, 1, 0);
            panel.Controls.Add(this.colorPickerButton, 1, 0);

            return panel;
        }

        public override void LoadFromConfiguration()
        {
            loading = true;

            var color = this.Value.GetConfigurationValue<Color>();
            lastColorChange = new(color, LastColorChangeCauseEnum.LOAD);

            this.colorHexaTextBox.Text = color.ToHexString();
            this.colorPickerButton.BackColor = color;

            loading = false;
        }

        public override void SaveToConfiguration()
        {
            var color = this.colorPickerButton.BackColor;
            this.Value.SetConfigurationValue(color);
        }
    }
}

/*

private readonly ComboBox colorNameComboBox;
this.colorNameComboBox = new()
{
    Dock = DockStyle.Fill,
    MinimumSize = new Size(150, 0),
    MaximumSize = new Size(150, 0),
    FlatStyle = FlatStyle.Flat,
    DropDownStyle = ComboBoxStyle.DropDown,
    Font = SettingsConstants.SETTINGS_VALUE_TEXTBOX_FONT,
    BackColor = Form.DefaultBackColor,
    DisplayMember = "Text",
    ValueMember = "Value"
};

SettingsColorEditor.ParseColors();

var dataSourceItems = new List<object>();
foreach (var pair in COLOR_DICTIONARY)
{
    dataSourceItems.Add(new { Text = pair.Key, Value = pair.Value });
}
this.colorNameComboBox.DataSource = dataSourceItems;
this.colorNameComboBox.SelectedIndexChanged += (sender, args) =>
{
    if(this.colorNameComboBox.SelectedItem is Color selectedColor)
    {
        this.ApplyColor(selectedColor);
    }
};

this.colorNameComboBox.SelectedValue = color;


        private string? GetColorName(Color color)
        {
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                var derivedColor = Color.FromKnownColor(knownColor);

                if (derivedColor.ToHexaString() == color.ToHexaString())
                {
                    return Enum.GetName(knownColor);
                }
            }

            return null;
        }
*/