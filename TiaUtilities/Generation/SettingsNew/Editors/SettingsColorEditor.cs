using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Lines;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.SettingsNew.Editors
{

    public class SettingsColorEditor : SettingsEditor
    {
        private readonly TextBox colorTextBox;
        private readonly Button colorPickerButton;

        private Color lastColor = Color.White;

        public SettingsColorEditor(SettingsValue value) : base(value)
        {
            this.colorTextBox = new()
            {
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
            this.colorTextBox.TextChanged += ColorTextBoxTextChangedEvent;

            this.colorPickerButton = new Button()
            {
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                Dock = DockStyle.Fill,
                MaximumSize = new Size(50, 0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = Padding.Empty,
                Margin = new Padding(2),
                FlatStyle = FlatStyle.Flat
            };
            this.colorPickerButton.FlatAppearance.BorderColor = Color.Black;
            this.colorPickerButton.FlatAppearance.BorderSize = 1;

            this.colorPickerButton.Click += ColorPickerButtonClickEvent;

            this.ApplyColor((Color)value.GetConfigurationValue());
        }
        private void ColorTextBoxTextChangedEvent(object? sender, EventArgs e)
        {
            try
            {
                ApplyColor(ColorTranslator.FromHtml(colorTextBox.Text), overrideText: false);
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
                colorTextBox.Text = color.ToHexString();
            }
            colorPickerButton.BackColor = color;

            this.Value.SetConfigurationValue(color);
        }

        public override Control GetControl()
        {
            return new TableLayoutPanel()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                ColumnCount = 2,
                ColumnStyles = { new ColumnStyle(SizeType.Percent, 30f), new ColumnStyle(SizeType.Percent, 70f) },
                RowCount = 1,
                RowStyles = { new RowStyle(SizeType.AutoSize) },
                Controls = { this.colorTextBox, this.colorPickerButton }
            };
        }
    }
}
