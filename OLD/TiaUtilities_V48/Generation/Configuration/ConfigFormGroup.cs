using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormGroup : IConfigGroup
    {
        private readonly ConfigForm configForm;
        private readonly List<IConfigObject> configObjectList;

        public bool IsSubGroup { get; set; } = false;

        public ConfigFormGroup(ConfigForm configForm)
        {
            this.configForm = configForm;
            this.configObjectList = new List<IConfigObject>();
        }

        public ConfigFormGroup AddGroup()
        {
            var group = new ConfigFormGroup(this.configForm) { IsSubGroup  = true };
            this.configObjectList.Add(group);
            return group;
        }

        public C AddLine<C>(ConfigFormLineType<C> type) where C : ConfigFormLine<C>
        {
            IConfigLine line;
            if (type.Equals(ConfigFormLineTypes.LABEL))
            {
                line = new ConfigFormLabelLine();
            }
            else if (type.Equals(ConfigFormLineTypes.TEXT_BOX))
            {
                line = new ConfigFormTextBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.COMBO_BOX))
            {
                line = new ConfigFormComboBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.CHECK_BOX))
            {
                line = new ConfigFormCheckBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.BUTTON_PANEL))
            {
                line = new ConfigFormButtonPanelLine();
            }
            else if (type.Equals(ConfigFormLineTypes.COLOR_PICKER))
            {
                line = new ConfigFormColorPickerLine();
            }
            else if (type.Equals(ConfigFormLineTypes.JAVASCRIPT))
            {
                line = new ConfigFormJavascriptLine();
            }
            else if (type.Equals(ConfigFormLineTypes.JSON))
            {
                line = new ConfigFormJSONLine();
            }
            else
            {
                throw new Exception("Invalid ConfigForm.AddLine ConfigFormLineType for" + type);
            }

            configObjectList.Add(line);
            return (C)line;
        }

        public Control GetControl()
        {
            var mainPanel = new TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                Margin = new Padding(0),
                Padding = new Padding(0),
                CellBorderStyle = this.IsSubGroup ? TableLayoutPanelCellBorderStyle.Single : TableLayoutPanelCellBorderStyle.None
            };
            
            var linePanelList = new List<TableLayoutPanel>();
            var biggestTitleLength = 0;

            foreach (var configObject in configObjectList)
            {
                var objectPanel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Margin = new Padding(2),
                    Padding = new Padding(0),
                    RowCount = 1,
                    RowStyles = { new RowStyle(SizeType.Percent, 50f) },
                };
                linePanelList.Add(objectPanel);

                if(configObject is IConfigLine line)
                {
                    var labelText = line.GetLabelText();
                    if (labelText != null)
                    {
                        objectPanel.ColumnCount++;
                        objectPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                        var label = new Label
                        {
                            Font = this.configForm.LabelFont,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Dock = DockStyle.Fill,
                            Text = line.GetLabelText(),
                            AutoSize = true,
                            Padding = new Padding(0),
                            Margin = new Padding(0)
                        };
                        label.Font = line.GetLabelFont() ?? this.configForm.LabelFont;
                        objectPanel.Controls.Add(label);

                        var size = TextRenderer.MeasureText(label.Text, this.configForm.LabelFont);
                        size.Width += 4; //Padding
                        if (size.Width > biggestTitleLength)
                        {
                            biggestTitleLength = size.Width;
                        }
                    }

                    var control = line.GetControl();
                    if (control != null)
                    {
                        if (line.IsLabelOnTop())
                        {
                            objectPanel.RowCount++;
                            objectPanel.RowStyles.Add(new ColumnStyle(SizeType.AutoSize)); //Autosize is needed otherwise it will take the same space as the row below and occupy useless space.
                        }
                        else
                        {
                            objectPanel.ColumnCount++;
                            objectPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                        }

                        control.Width = this.configForm.ControlWidth;
                        control.Height = line.GetHeight() == 0 ? this.configForm.ControlHeight : line.GetHeight();
                        control.Dock = DockStyle.Fill;
                        control.Font = this.configForm.ControlFont;
                        objectPanel.Controls.Add(control);
                    }

                    mainPanel.RowCount++;
                    mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    mainPanel.Controls.Add(objectPanel);
                }
                else
                {
                    var control = configObject.GetControl();
                    if(control != null)
                    {
                        mainPanel.RowCount++;
                        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                        mainPanel.Controls.Add(control);
                    }
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
