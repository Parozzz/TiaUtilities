using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Utility.Extensions;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigColorPickerLine : ConfigLine<ConfigColorPickerLine>
    {
        private readonly TableLayoutPanel panel;
        private readonly TextBox colorTextBox;
        private readonly Button colorPickerButton;


        private Action<Color> colorAction;
        private Color lastColor = Color.White;

        public ConfigColorPickerLine()
        {
            panel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(2),
                ColumnCount = 2,
                ColumnStyles = { new ColumnStyle(SizeType.Percent, 30f), new ColumnStyle(SizeType.Percent, 70f) },
                RowCount = 1,
                RowStyles = { new RowStyle(SizeType.AutoSize) }
            };

            colorTextBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                TextAlign = HorizontalAlignment.Center,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            colorTextBox.TextChanged += ColorTextBoxTextChangedEvent;
            panel.Controls.Add(colorTextBox);

            colorPickerButton = new Button()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            colorPickerButton.Click += ColorPickerButtonClickEvent;
            panel.Controls.Add(colorPickerButton);
        }

        private void ColorTextBoxTextChangedEvent(object sender, EventArgs e)
        {
            try
            {
                ApplyColor(ColorTranslator.FromHtml(colorTextBox.Text), overrideText: false);
            }
            catch { }
        }

        private void ColorPickerButtonClickEvent(object sender, EventArgs e)
        {
            try
            {
                var colorDialog = new ColorDialog()
                {
                    Color = lastColor,
                };

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

        public ConfigColorPickerLine ApplyColor(Color color, bool overrideText = true)
        {
            lastColor = color;
            if (overrideText)
            {
                colorTextBox.Text = color.ToHexString();
            }
            colorPickerButton.BackColor = color;
            colorAction?.Invoke(color);
            return this;
        }

        public ConfigColorPickerLine ColorChanged(Action<Color> action)
        {
            colorAction = action;
            return this;
        }

        public override Control GetControl()
        {
            return panel;
        }
    }
}
