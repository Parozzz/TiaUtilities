using System.Reflection;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.SettingsNew.Editors
{

    public class SettingsColorEditor : SettingsEditor
    {

        private readonly static Dictionary<string, Color> COLOR_DICTIONARY = [];
        private static void ParseColors()
        {
            if(COLOR_DICTIONARY.Count > 0)
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

        private Color lastColor = Color.White;

        public SettingsColorEditor(SettingsValue value) : base(value)
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
            this.colorHexaTextBox.TextChanged += ColorTextBoxTextChangedEvent;

            this.colorPickerButton = new Button()
            {
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                Dock = DockStyle.Fill,
                MinimumSize = new(40, 0),
                MaximumSize = new(40, 20),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = Padding.Empty,
                Margin = new Padding(2),
                FlatStyle = FlatStyle.Flat
            };
            this.colorPickerButton.FlatAppearance.BorderColor = Color.Black;
            this.colorPickerButton.FlatAppearance.BorderSize = 1;

            this.colorPickerButton.Click += ColorPickerButtonClickEvent;

            this.ApplyColor(value.GetConfigurationValue<Color>());

            SettingsUtils.AddContextualMenu(this.colorHexaTextBox, value);
            SettingsUtils.AddContextualMenu(this.colorPickerButton, value);
        }

        private void ColorTextBoxTextChangedEvent(object? sender, EventArgs e)
        {
            try
            {
                ApplyColor(ColorTranslator.FromHtml(colorHexaTextBox.Text), overrideText: false);
            }
            catch { }
        }

        private void ColorPickerButtonClickEvent(object? sender, EventArgs e)
        {
            try
            {
                var colorDialog = new ColorDialog() { Color = lastColor, };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyColor(colorDialog.Color);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        public void ApplyColor(Color color, bool overrideText = true)
        {
            lastColor = color;
            if (overrideText)
            {
                this.colorHexaTextBox.Text = color.ToHexString();
            }
            this.colorPickerButton.BackColor = color;

            this.Value.SetConfigurationValue(color);
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

            panel.Controls.Add(this.colorHexaTextBox, 0, 0);
            //panel.Controls.Add(this.colorNameComboBox, 1, 0);
            panel.Controls.Add(this.colorPickerButton, 1, 0);
            
            return panel;
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