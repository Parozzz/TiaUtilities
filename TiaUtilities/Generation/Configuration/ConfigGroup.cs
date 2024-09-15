using TiaUtilities.Generation.Configuration;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigGroup(ConfigForm? configForm) : IConfigGroup
    {
        private readonly List<IConfigObject> configObjectList = [];

        private int controlWidth = 0;
        private bool noAdapt = false;

        public bool IsSubGroup { get; set; } = false;

        /*
                 public Font LabelFont { get; set; } = ConfigStyle.LABEL_FONT;
        public Font ControlFont { get; set; } = ConfigStyle.CONTROL_FONT;
        public int ControlWidth { get; set; } = 300;
        public int ControlHeight { get; set; } = 30;
         */

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
            var biggestTitleLength = 0;

            foreach (var configObject in configObjectList)
            {
                Control? configObjectControl;
                if (configObject is IConfigLine line)
                {
                    var linePanel = new TableLayoutPanel
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Anchor = AnchorStyles.None,
                        Dock = DockStyle.Fill,
                        Margin = Padding.Empty,
                        Padding = Padding.Empty,
                    };
                    configObjectControl = linePanel;
                    linePanelList.Add(linePanel);

                    Label? label = null;

                    var labelText = line.GetLabelText();
                    var hasLabel = !string.IsNullOrEmpty(labelText);
                    if (hasLabel)
                    {
                        linePanel.RowCount++;
                        linePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        linePanel.ColumnCount++;
                        linePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                        linePanel.Controls.Add(label = new Label
                        {
                            AutoSize = true,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Dock = DockStyle.Fill,
                            Text = labelText,
                            Padding = Padding.Empty,
                            Margin = Padding.Empty,
                            Font = line.GetLabelFont() ?? configForm?.LabelFont ?? ConfigStyle.LABEL_FONT
                        });

                        var size = TextRenderer.MeasureText(labelText, label.Font);
                        size.Width += 4; //Padding
                        if (size.Width > biggestTitleLength)
                        {
                            biggestTitleLength = size.Width;
                        }
                    }

                    var control = line.GetControl();
                    if (control != null)
                    {
                        if (hasLabel && line.IsLabelOnTop())
                        {
                            linePanel.RowCount++;
                            linePanel.RowStyles.Add(new ColumnStyle(SizeType.AutoSize)); //Autosize is needed otherwise it will take the same space as the row below and occupy useless space.
                        }
                        else
                        {
                            if (!hasLabel)
                            {
                                linePanel.RowCount++;
                                linePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
                            }

                            if (label != null)
                            {
                                label.TextAlign = ContentAlignment.MiddleRight; //Set the text to be closer to the control!
                                label.Padding = new(0, 0, 4, 0);
                            }

                            linePanel.ColumnCount++;
                            linePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                        }

                        control.Width = this.controlWidth == 0 ? (configForm?.ControlWidth ?? 300) : this.controlWidth;
                        control.Height = line.GetHeight() == 0 ? (configForm?.ControlHeight ?? 30) : line.GetHeight();
                        control.Dock = line.IsControlNoAdapt() ? DockStyle.None : DockStyle.Fill;
                        control.Font = configForm?.ControlFont ?? ConfigStyle.CONTROL_FONT;
                        linePanel.Controls.Add(control);
                    }
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
    }
}
