using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration
{
    public class ConfigVerticalGroup : IConfigGroup
    {
        private readonly ConfigForm configForm;
        private readonly List<IConfigObject> configObjectList;

        private int height = 0;
        private int splitterDistance = -1;

        public ConfigVerticalGroup(ConfigForm configForm)
        {
            this.configForm = configForm;
            this.configObjectList = [];
        }

        public ConfigForm GetConfigForm()
        {
            return configForm;
        }

        public C Add<C>(C configObject) where C : IConfigObject
        {
            configObjectList.Add(configObject);
            return configObject;
        }

        public ConfigVerticalGroup Height(int height)
        {
            this.height = height;
            return this;
        }

        public ConfigVerticalGroup SplitterDistance(int splitterDistance)
        {
            this.splitterDistance = splitterDistance;
            return this;
        }

        public Control? GetControl()
        {
            if (configObjectList.Count == 1)
            {
                var control = this.CreateConfigObjectPanel(this.configObjectList[0]);
                if (control != null)
                {
                    return CreateSingleControlPanel(control);
                }
            }
            else if (configObjectList.Count >= 2)
            {
                var panelLeft = this.CreateConfigObjectPanel(this.configObjectList[0], insidedSplitContainer: true);
                var panelRight = this.CreateConfigObjectPanel(this.configObjectList[1], insidedSplitContainer: true);
                if (panelLeft != null && panelRight != null)
                {
                    var splitContainer = new SplitContainer()
                    {
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        SplitterWidth = 15,
                        Height = this.height,
                    };
                    if (this.splitterDistance > 0)
                    {
                        splitContainer.SplitterDistance = this.splitterDistance;
                    }
                    splitContainer.Paint += (sender, args) => ControlPaint.DrawBorder(args.Graphics, Rectangle.Inflate(args.ClipRectangle, -2, -2), Color.DarkGray, ButtonBorderStyle.Solid);

                    splitContainer.Panel1.Controls.Add(panelLeft);
                    splitContainer.Panel2.Controls.Add(panelRight);

                    return splitContainer;
                }
                else if (panelLeft != null)
                {
                    return CreateSingleControlPanel(panelLeft);
                }
                else if (panelRight != null)
                {
                    return CreateSingleControlPanel(panelRight);
                }
            }

            return null;
        }

        private static TableLayoutPanel CreateSingleControlPanel(Control control)
        {
            var mainPanel = new TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                ColumnStyles = { new ColumnStyle(SizeType.Percent, 50f) },
                RowCount = 1,
                RowStyles = { new RowStyle(SizeType.Percent, 50f) },
                Margin = new Padding(0),
                Padding = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            mainPanel.Controls.Add(control);
            return mainPanel;
        }

        private TableLayoutPanel? CreateConfigObjectPanel(IConfigObject configObject, bool insidedSplitContainer = false)
        {
            if (configObject is IConfigLine line)
            {
                var linePanel = new TableLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    RowCount = 1,
                    RowStyles = { new RowStyle(SizeType.Percent, 50f) },
                };

                var labelText = line.GetLabelText().Value;
                if (labelText != null)
                {
                    linePanel.ColumnCount++;
                    linePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                    var label = new Label
                    {
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = labelText,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Padding = new Padding(0),
                        Margin = new Padding(2),
                        Font = line.GetLabelFont() ?? this.configForm.LabelFont,
                    };
                    line.GetLabelText().Changed += (sender, args) => label.Text = args.NewValue;
                    linePanel.Controls.Add(label);

                    //Needs height otherwise will not show! AutoSize won't work alone.
                    var textSize = TextRenderer.MeasureText(label.Text, this.configForm.LabelFont);
                    linePanel.RowStyles[0].SizeType = SizeType.Absolute;
                    linePanel.RowStyles[0].Height = textSize.Height + 4;
                }

                var control = line.GetControl();
                if (control != null)
                {
                    if (line.IsLabelOnTop())
                    {
                        linePanel.RowCount++;
                        linePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); //Autosize is needed otherwise it will take the same space as the row below and occupy useless space.
                    }
                    else
                    {
                        linePanel.ColumnCount++;
                        linePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                    }

                    if (insidedSplitContainer)
                    {
                        control.Width = control.Height = 0;
                    }
                    else
                    {
                        control.Width = this.configForm.ControlWidth;
                        control.Height = line.GetHeight() == 0 ? this.configForm.ControlHeight : line.GetHeight();
                    }

                    control.Dock = DockStyle.Fill;
                    control.Font = this.configForm.ControlFont;
                    linePanel.Controls.Add(control);
                }

                return linePanel;
            }
            else
            {
                var control = configObject.GetControl();
                return control == null ? null : CreateSingleControlPanel(control);
            }
        }

        public Control? GetWrappedControl()
        {
            return this.GetControl();
        }
    }
}

