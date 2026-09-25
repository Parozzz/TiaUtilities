using TiaUtilities.Editors.Javascript;
using TiaUtilities.Editors.Json;
using TiaUtilities.Editors.T_SQL;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory
{
    internal static class SettingsControls
    {
        private const int TEXT_BOX_MIN_WIDTH = 300;

        public static TextBox GetTextBox(string sampleText, string startText, SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            var font = StyleManager.Fonts.NORMAL;

            var sampleSize = TextRenderer.MeasureText(sampleText, font, Size.Empty, TextFormatFlags.TextBoxControl);
            var startSize = TextRenderer.MeasureText(startText, font, Size.Empty, TextFormatFlags.TextBoxControl);
            TextBox textBox = new()
            {
                Font = font,
                Anchor = AnchorStyles.Left | AnchorStyles.Right, //This allows centering if no label is present!
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,

                ReadOnly = false,
                MinimumSize = new Size(Math.Max(startSize.Width, TEXT_BOX_MIN_WIDTH), sampleSize.Height + 0),
                BackColor = Form.DefaultBackColor,
                ForeColor = Form.DefaultForeColor,
                Margin = new Padding(8, 4, 8, 0),

                Text = startText,
            };
            ControlUtils.SetDoubleBuffered(textBox);

            if (generalOptions.MinWidth > 0)
            {
                var minSize = textBox.MinimumSize;
                textBox.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return textBox;
        }

        public static ComboBox GetComboBox(IEnumerable<string> items, SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            var font = StyleManager.Fonts.NORMAL;

            var maxWidth = items.Max(i => TextRenderer.MeasureText(i, font, Size.Empty, TextFormatFlags.TextBoxControl).Width);
            maxWidth += 20;

            ComboBoxFilterable comboBox = new()
            {
                Font = font,
                Anchor = AnchorStyles.Left,
                FlatStyle = FlatStyle.System,

                MinimumSize = new(maxWidth, 0),
                Size = new(maxWidth, 0),

                BackColor = Form.DefaultBackColor,
                ForeColor = Form.DefaultForeColor,

                Margin = new(8, 0, 8, 0),
            };

            comboBox.DropDownClosed += (sender, args) =>
            {//Remove focus on ComboBox after closing drop down to avoid having it selected (Annoying).
                var form = comboBox.FindForm();
                form?.BeginInvoke(() => form.ActiveControl = null);
            };

            if (generalOptions.MinWidth > 0)
            {
                var minSize = comboBox.MinimumSize;
                comboBox.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return comboBox;
        }

        public static JavascriptEditor GetJavascriptEditor(SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            JavascriptEditor editor = new();
            editor.InitControl();

            var control = editor.GetControl();
            control.MinimumSize = new(550, 480);

            if (generalOptions.MinWidth > 0)
            {
                var minSize = control.MinimumSize;
                control.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return editor;
        }

        public static JsonEditor GetJSONEditor(SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            JsonEditor editor = new();
            editor.InitControl();

            var control = editor.GetControl();
            control.MinimumSize = new(550, 480);

            if (generalOptions.MinWidth > 0)
            {
                var minSize = control.MinimumSize;
                control.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return editor;
        }

        public static TSQLEditor GetTSQLEditor(SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            TSQLEditor editor = new();
            editor.InitControl();

            var control = editor.GetControl();
            control.MinimumSize = new(550, 480);

            if (generalOptions.MinWidth > 0)
            {
                var minSize = control.MinimumSize;
                control.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return editor;
        }

        public static CheckBox GetCheckBox(SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            CheckBox checkBox = new()
            {
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                CheckAlign = ContentAlignment.MiddleCenter,
                Text = "",
                AutoSize = false,
                FlatStyle = FlatStyle.Standard,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
            };

            if (generalOptions.MinWidth > 0)
            {
                var minSize = checkBox.MinimumSize;
                checkBox.MinimumSize = new(generalOptions.MinWidth, minSize.Height);
            }

            return checkBox;
        }

        public static Label GetText(string text, SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions)
        {
            Label label = new()
            {
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Right | AnchorStyles.Left,
                AutoSize = true,
                Font = StyleManager.Fonts.BIG_BOLD,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            if (generalOptions.TextAlign != null)
            {
                label.TextAlign = (ContentAlignment)generalOptions.TextAlign;
            }

            return label;
        }

        public static Label GetNameLabel(string text, string description, SettingsFactoryGeneralOptions generalOptions, SettingsFactoryCreateOptions createOptions, Func<string>? controlTextCallback = null)
        {
            var label = new LabelWithTooltip()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Font = StyleManager.Fonts.BIG_SEMIBOLD,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                SymbolsPosition = LabelWithTooltip.ItemsPosition.Right,
                SymbolSpacing = 2,
                Padding = Padding.Empty,
                Margin = new(5, 0, 0, 0),
            };

            if (!string.IsNullOrEmpty(description))
            {
                LabelWithTooltip.LabelItem descriptionItem = new()
                {
                    TooltipText = description,
                    Symbol = "ⓘ",
                    Color = Form.DefaultForeColor,
                    HoverColor = Color.MediumSeaGreen,
                };
                label.Symbols.Add(descriptionItem);
            }

            if (controlTextCallback != null && generalOptions.SupportPlaceholders && createOptions.PlaceholdersCallback != null)
            {
                LabelWithTooltip.LabelItem placeholderItem = new()
                {
                    TooltipText = "",
                    Symbol = "🔎",
                    Color = Form.DefaultForeColor,
                    HoverColor = Color.MediumSeaGreen,
                    OnHover = item =>
                    {
                        var controlText = controlTextCallback();

                        var parsedText = createOptions.PlaceholdersCallback(controlText);
                        item.TooltipText = parsedText;
                    }
                };

                label.Symbols.Add(placeholderItem);
            }


            return label;
        }

        public static Label GetDividerLabel(Color backColor)
        {
            return new()
            {
                BackColor = backColor,
                Dock = DockStyle.Fill,
                Text = "",
                Size = new(0, 1),
                MinimumSize = new(0, 1),
                MaximumSize = new(0, 1),
            };
        }
    }
}
