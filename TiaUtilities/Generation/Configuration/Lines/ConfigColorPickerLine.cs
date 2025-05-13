using System.Linq.Expressions;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigColorPickerLine : ConfigLine<ConfigColorPickerLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly RJTextBox colorTextBox;
        private readonly Button colorPickerButton;

        private Action<Color>? colorAction;
        private Action? transferToOtherColorAction;

        private Color lastColor = Color.White;

        public ConfigColorPickerLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;
            this.colorTextBox = new RJTextBox()
            {
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                Margin = Padding.Empty,
                BorderStyle = BorderStyle.None,
                Underlined = true,
                UnderlineColor = ConfigStyle.UNDERLINE_COLOR,
                TextAlign = HorizontalAlignment.Center,
            };
            this.colorTextBox.TextChanged += ColorTextBoxTextChangedEvent;

            this.colorPickerButton = new Button()
            {
                ForeColor = ConfigStyle.FORE_COLOR,
                BackColor = ConfigStyle.BACK_COLOR,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = Padding.Empty,
                Margin = new(2),
            };
            this.colorPickerButton.Click += ColorPickerButtonClickEvent;
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
        
        public ConfigColorPickerLine BindColor(Expression<Func<Color>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ApplyColor(propertyExpression.Compile().Invoke());
            colorAction = colorValue => propertyInfo.SetValue(configuration, colorValue);
            transferToOtherColorAction = () =>
            {
                var colorValue = this.lastColor;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, colorValue);
                }
            };
            return this;
        }

        public override Control GetControl()
        {
            return new TableLayoutPanel()
            {
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
