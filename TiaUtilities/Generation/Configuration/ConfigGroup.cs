﻿using TiaUtilities.Generation.Configuration.Lines;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Configuration
{
    public class ConfigGroup(ConfigForm configForm) : IConfigGroup
    {
        private readonly List<IConfigObject> configObjectList = [];

        private int controlWidth = 0;
        private bool noAdapt = false;

        public bool IsSubGroup { get; set; } = false;

        public ConfigForm GetConfigForm()
        {
            return configForm;
        }

        public ConfigGroup ControlWidth(int controlWidth)
        {
            this.controlWidth = controlWidth;
            return this;
        }

        public ConfigGroup NoAdapt()
        {
            this.noAdapt = true;
            return this;
        }

        public C Add<C>(C configObject) where C : IConfigObject
        {
            this.configObjectList.Add(configObject);
            return configObject;
        }

        public Control GetControl()
        {
            var mainPanel = new TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.None,
                Dock = noAdapt ? DockStyle.None : DockStyle.Fill,
                ColumnCount = 1,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                Margin = new Padding(3, 0, 3, 0),
                Padding = Padding.Empty,
                CellBorderStyle = this.IsSubGroup ? TableLayoutPanelCellBorderStyle.OutsetDouble : TableLayoutPanelCellBorderStyle.None,
            };

            var linePanelList = new List<TableLayoutPanel>();
            int biggestTitleLength = 0;

            foreach (var configObject in configObjectList)
            {
                Control? configObjectControl;
                if (configObject is IConfigLine line)
                {
                    TableLayoutPanel linePanel = new()
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Anchor = AnchorStyles.None,
                        Dock = DockStyle.Fill,
                        Margin = Padding.Empty,
                        Padding = Padding.Empty,
                    };
                    BuildLineTableLayout(line, linePanel, ref biggestTitleLength);
                    linePanelList.Add(linePanel);

                    configObjectControl = linePanel;
                }
                else
                {
                    configObjectControl = configObject.GetControl();
                }

                if (configObjectControl != null)
                {
                    mainPanel.RowCount++;
                    mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    mainPanel.Controls.Add(configObjectControl);
                }
            }

            foreach (var panel in linePanelList)
            {
                if (panel.ColumnCount == 2) //Only if there are both label AND control.
                {
                    panel.ColumnStyles[0].SizeType = SizeType.Absolute;
                    panel.ColumnStyles[0].Width = biggestTitleLength;
                }
            }

            return mainPanel;
        }

        private void BuildLineTableLayout(IConfigLine line, TableLayoutPanel panel, ref int biggestTitleLength)
        {
            Label? label = null;

            var labelText = line.GetLabelText().Value;

            var hasLabel = !string.IsNullOrEmpty(labelText);
            if (hasLabel)
            {
                panel.RowCount++;
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.ColumnCount++;
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                label = new()
                {
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Text = labelText,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                    AutoEllipsis = true,
                    Font = line.GetLabelFont() ?? configForm?.LabelFont ?? ConfigStyle.LABEL_FONT
                };

                panel.Controls.Add(label);

                line.GetLabelText().Changed += (sender, args) => label.Text = args.NewValue;

                if (line is not ConfigLabelLine)
                {//For the biggest title, you need to consider only the lines that have A ACTIVE CONTROL! Lines that only contains the label are not counted!
                    var size = TextRenderer.MeasureText(labelText, label.Font);
                    size.Width += 4; //Padding
                    if (size.Width > biggestTitleLength)
                    {
                        biggestTitleLength = size.Width;
                    }
                }
            }

            var control = line.GetControl();
            if (control != null)
            {
                if (hasLabel && line.IsLabelOnTop())
                {
                    panel.RowCount++;
                    panel.RowStyles.Add(new ColumnStyle(SizeType.AutoSize)); //Autosize is needed otherwise it will take the same space as the row below and occupy useless space.
                }
                else
                {
                    if (!hasLabel)
                    {
                        panel.RowCount++;
                        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
                    }

                    if (label != null)
                    {
                        label.TextAlign = ContentAlignment.MiddleRight; //Set the text to be closer to the control!
                        label.Padding = new(0, 0, 4, 0);
                    }

                    panel.ColumnCount++;
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                }

                var width = this.controlWidth == 0 ? (configForm?.ControlWidth ?? 300) : this.controlWidth;
                var height = line.GetHeight() == 0 ? (configForm?.ControlHeight ?? 30) : line.GetHeight();

                control.Width = width;
                control.Height = height;
                control.Dock = line.IsControlNoAdapt() ? DockStyle.None : DockStyle.Fill;
                control.Font = configForm?.ControlFont ?? ConfigStyle.CONTROL_FONT;

                ToolStripMenuItem item = new(Locale.CONFIG_LINE_TRANSFER_TO_OTHERS) { Image = Image.FromFile("Resources/Images/noun-transfer-7710063.png") };
                item.Click += (sender, args) => line.TrasferToAllConfigurations();
                control.ContextMenuStrip = new() { Items = { item } };

                panel.Controls.Add(control);

            }
        }
    }
}
