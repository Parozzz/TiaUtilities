using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormBindingControlLoader(SettingsBindings bindings, SettingsForm form)
    {
        private record ControlPosition(Control? Control, int Column, int Row, SizeType RowSizeType = SizeType.AutoSize, float RowSize = 0f, int ColumnSpan = 0, int RowSpan = 0);

        private static void ApplyControlPositions(TableLayoutPanel panel, List<ControlPosition> controlPositionList)
        {
            var controlList = controlPositionList.Select(controlPosition => controlPosition.Control).Where(c => c is not null).Cast<Control>();
            panel.Controls.AddRange([.. controlList]);

            int lastRow = -1;

            var orderedCollection = controlPositionList.OrderBy((controlPosition) => controlPosition.Row);
            foreach (var controlPosition in orderedCollection)
            {
                if (lastRow != controlPosition.Row)
                {
                    panel.RowStyles.Add(new RowStyle(controlPosition.RowSizeType, controlPosition.RowSize));
                    lastRow = controlPosition.Row;
                }

                var control = controlPosition.Control;
                if (control != null)
                {
                    panel.SetCellPosition(control, new(controlPosition.Column, controlPosition.Row));
                    if (controlPosition.ColumnSpan > 0)
                    {
                        panel.SetColumnSpan(control, controlPosition.ColumnSpan);
                    }

                    if (controlPosition.RowSpan > 0)
                    {
                        panel.SetRowSpan(control, controlPosition.RowSpan);
                    }
                }
            }
        }

        const int SECTION_NAME_COLUMN = 0;
        const int SECTION_BORDER_COLUMN = 1;
        const int SECTION_VALUES_COLUMN = 2;

        public SettingsBindings Bindings { get; init; } = bindings;
        private readonly SettingsForm settingsForm = form;

        private readonly List<SettingsFormMacroSection> macroSectionList = [];
        private readonly List<ControlPosition> createdControlPositionList = [];

        public void UpdateValues()
        {
            foreach (var macroSection in this.macroSectionList)
            {
                macroSection.ListItem.Text = macroSection.Name;

                var label = macroSection.Label;
                if (label != null)
                {
                    label.Text = macroSection.Name;
                }

                var newConfigurationObject = macroSection.Binding.GetConfigurationObject() ?? throw new InvalidOperationException($"Cannot RequestUpdate with a null ConfigurationObject inside a SettingsMacroSectionBinding. Name: {macroSection.Name}");
                foreach (var formValue in macroSection.Sections.SelectMany(section => section.FormValueList))
                {
                    formValue.UpdateConfigurationObject(newConfigurationObject);
                    formValue.Editor?.LoadFromConfiguration();
                }
            }
        }

        public void UpdateVisiblePercentages(int topScrollPosition, int bottomScrollPosition)
        {
            foreach (var section in this.macroSectionList.SelectMany(formMacroSection => formMacroSection.Sections))
            {
                var panel = section.Panel;
                if (panel == null)
                {
                    continue;
                }

                var top = panel.Top;
                var bottom = panel.Bottom;

                var height = (float)panel.Height;
                if (bottom <= topScrollPosition || top >= bottomScrollPosition)
                {
                    section.VisiblePercentage = 0;
                }
                else
                {
                    var minBottom = Math.Min(bottom, bottomScrollPosition);
                    var maxTop = Math.Max(topScrollPosition, top);

                    section.VisiblePercentage = (minBottom - maxTop) / height;
                }

                var listItem = section.ListItem;
                if (listItem != null && listItem.Tag is SettingsFormSectionListView.ItemSectionTag sectionTag)
                {
                    var oldVisible = sectionTag.IsSectionVisible;
                    sectionTag.IsSectionVisible = (section.VisiblePercentage > 0.3f);

                    if (oldVisible != sectionTag.IsSectionVisible)
                    {
                        listItem.Text = listItem.Text; //This force a redraw of the item!
                    }
                }
            }
        }

        public void Load()
        {
            this.macroSectionList.Clear();
            foreach (var macroSectionBinding in this.Bindings.MacroSectionList)
            {
                var configurationObject = macroSectionBinding.GetConfigurationObject();
                if (!macroSectionBinding.Visible || configurationObject == null)
                {
                    continue;
                }

                SettingsFormMacroSection formMacroSection = new(macroSectionBinding);
                this.macroSectionList.Add(formMacroSection);

                foreach (var sectionBinding in macroSectionBinding.SectionsList)
                {
                    var formValuesArray = sectionBinding.ValueList.Select(bindings => SettingsFormValue.FromBinding(bindings, configurationObject, settingsForm)).ToArray();

                    SettingsFormSection formSection = new(sectionBinding, formMacroSection);
                    formSection.FormValueList.AddRange(formValuesArray);

                    formMacroSection.Sections.Add(formSection);
                }
            }

            this.CreateControls();
        }

        private void CreateControls()
        {
            //I've tried many things and this seems to be the more efficient!
            this.createdControlPositionList.Clear();

            int rowCount = 0;
            foreach (var formMacroSection in macroSectionList)
            {
                var lastOpenInformation = SettingsFormLastOpenInformation.GetFromGuid(formMacroSection.Guid);

                Label macroSectioNameLabel = new()
                {
                    Text = formMacroSection.Name,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Form.DefaultBackColor,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                    Font = SettingsFormConstants.MACROSECTION_NAME_LABEL_FONT,
                };
                formMacroSection.Label = macroSectioNameLabel;

                this.createdControlPositionList.Add(new(macroSectioNameLabel, 0, rowCount, ColumnSpan: 3));
                rowCount++;

                var macroSectionButtonToolbar = this.CreateButtonToolbar(formMacroSection, lastOpenInformation);
                this.createdControlPositionList.Add(new(macroSectionButtonToolbar, 0, rowCount, ColumnSpan: 3));
                rowCount++;

                foreach (var formSection in formMacroSection.Sections)
                {
                    var sectionStartRowCount = rowCount;
                    Label sectionNameLabel = new()
                    {
                        Text = formSection.Name,
                        Dock = DockStyle.Fill,
                        //Anchor = AnchorStyles.Top,
                        BorderStyle = BorderStyle.None,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleRight,
                        BackColor = Form.DefaultBackColor,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 0, 4, 0),
                        Font = SettingsFormConstants.SECTION_NAME_LABEL_FONT,
                    };
                    ControlUtils.SetDoubleBuffered(sectionNameLabel);
                    ControlUtils.CreateStandardToolTip().SetToolTip(sectionNameLabel, formSection.ToolTip);

                    TableLayoutPanel valuesPanel = new()
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        //Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                        Padding = Padding.Empty,
                        Margin = Padding.Empty,
                    };
                    ControlUtils.SetDoubleBuffered(valuesPanel);

                    List<ControlPosition> valuesControlPositionList = [];

                    int valuesRowCount = 0;
                    foreach (var formValue in formSection.FormValueList)
                    {
                        bool hasName = !string.IsNullOrEmpty(formValue.Name);
                        bool hasDescription = !string.IsNullOrEmpty(formValue.Description);

                        if (hasName)
                        {
                            SettingsValueNameLabel valueNameLabel = new()
                            {
                                Text = formValue.Name,
                                AutoSize = true,
                                Dock = DockStyle.Fill,
                                BorderStyle = BorderStyle.None,
                                TextAlign = ContentAlignment.MiddleLeft,
                                BackColor = Color.Transparent,
                                Margin = hasDescription ? Padding.Empty : new(0, 0, 0, 3),
                                Padding = Padding.Empty,
                                Font = SettingsFormConstants.VALUE_NAME_LABEL_FONT,
                            };
                            if(formValue.Binding.HasPlaceholderSupportDotMark)
                            {
                                valueNameLabel.DotList.Add(new() 
                                { 
                                    Color = SettingsFormConstants.MARKER_DOT_HAS_PLACEHOLDER_COLOR, 
                                    ToolTipText = Locale.SETTINGS_FORM_SECTION_HAS_PLACEHOLDER_TOOLTIP
                                });
                            }

                            valuesControlPositionList.Add(new(valueNameLabel, 0, valuesRowCount));
                            valuesRowCount++;
                        }

                        if (hasDescription)
                        {
                            Label valueDescriptionLabel = new()
                            {
                                Text = formValue.Description,
                                AutoSize = true,
                                Dock = DockStyle.Fill,
                                BorderStyle = BorderStyle.None,
                                TextAlign = ContentAlignment.MiddleLeft,
                                BackColor = Color.Transparent,
                                Margin = new Padding(0, 0, 0, 3),
                                Padding = Padding.Empty,
                                Font = SettingsFormConstants.DESCRIPTION_LABEL_FONT,
                                MaximumSize = new Size(SettingsFormConstants.VALUE_DESCRIPTION_MAX_SIZE, 0), //Since last columns is to fill the all control, set a maximun size to allow wrapping of text
                                Visible = lastOpenInformation.CommentVisibility,
                            };
                            ControlUtils.SetDoubleBuffered(valueDescriptionLabel);
                            formMacroSection.ValueDescriptionLabelList.Add(valueDescriptionLabel);

                            valuesControlPositionList.Add(new(valueDescriptionLabel, 0, valuesRowCount));
                            valuesRowCount++;
                        }

                        var editorControl = formValue.Editor?.GetControl();
                        if (editorControl != null)
                        {
                            valuesControlPositionList.Add(new(editorControl, 0, valuesRowCount));
                            valuesRowCount++;
                        }

                        valuesControlPositionList.Add(new(null, 0, valuesRowCount, SizeType.Absolute, SettingsFormConstants.VALUES_SEPERATION));
                        valuesRowCount++;
                    }

                    SettingsFormBindingControlLoader.ApplyControlPositions(valuesPanel, valuesControlPositionList);

                    //This is just to view better the division in sections
                    var sectionBorderLabel = new Label()
                    {
                        Text = "",
                        Dock = DockStyle.Fill,
                        BackColor = SettingsFormConstants.SECTION_BORDER_ENABLED_COLOR,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 6, (int)(SettingsFormConstants.SECTIONS_BORDER_COLUMN_SIZE / 2f), 6),
                    };
                    ControlUtils.SetDoubleBuffered(sectionBorderLabel);

                    this.createdControlPositionList.Add(new(sectionNameLabel, SECTION_NAME_COLUMN, rowCount));
                    this.createdControlPositionList.Add(new(sectionBorderLabel, SECTION_BORDER_COLUMN, rowCount));
                    this.createdControlPositionList.Add(new(valuesPanel, SECTION_VALUES_COLUMN, rowCount));
                    rowCount++;

                    this.createdControlPositionList.Add(new(null, 0, rowCount, SizeType.Absolute, SettingsFormConstants.SECTIONS_SEPARATION, RowSpan: 3));
                    rowCount++;

                    formSection.Panel = valuesPanel;

                    UpdateStateOfControlPositionList(formSection, sectionNameLabel, valuesControlPositionList, sectionBorderLabel);
                    this.Bindings.UpdateEvent +=
                        (sender, args) => UpdateStateOfControlPositionList(formSection, sectionNameLabel, valuesControlPositionList, sectionBorderLabel);
                }
            }
        }

        private static void UpdateStateOfControlPositionList(SettingsFormSection formSection, 
            Label sectionNameLabel, 
            List<ControlPosition> controlPositionList, 
            Label sectionBorderLabel)
        {
            var sectionEnabled = formSection.Binding.EnabledFunc();

            sectionNameLabel.Enabled = sectionEnabled;
            foreach (var valueControlPosition in controlPositionList)
            {
                var control = valueControlPosition.Control;
                if (control != null)
                {
                    control.Enabled = sectionEnabled;
                }
            }

            sectionBorderLabel.BackColor = sectionEnabled ? SettingsFormConstants.SECTION_BORDER_ENABLED_COLOR : SettingsFormConstants.SECTION_BORDER_DISABLED_COLOR;
        }

        private FlowLayoutPanel CreateButtonToolbar(SettingsFormMacroSection formMacroSection, SettingsFormLastOpenInformation lastOpenInformation)
        {
            Button toggleCommentsVisibilityButton = new()
            {
                AutoSize = true,
                BackgroundImage = Image.FromFile("Resources/Images/subtitle-8187463.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                MaximumSize = new(SettingsFormConstants.BUTTONS_SIZE, SettingsFormConstants.BUTTONS_SIZE),
                MinimumSize = new(SettingsFormConstants.BUTTONS_SIZE, SettingsFormConstants.BUTTONS_SIZE),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.Black, MouseDownBackColor = Color.LightGray },
                BackColor = Color.Transparent,
            };
            toggleCommentsVisibilityButton.Click += (sender, args) =>
            {
                this.settingsForm.SuspendAll();

                lastOpenInformation.CommentVisibility = !lastOpenInformation.CommentVisibility;
                formMacroSection.SetDescriptionLabelVisibility(lastOpenInformation.CommentVisibility);

                this.settingsForm.ResumeAll();
            };

            Button saveDefaultButton = new()
            {
                AutoSize = true,
                BackgroundImage = Image.FromFile("Resources/Images/favorite-8250509.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                MaximumSize = new(SettingsFormConstants.BUTTONS_SIZE, SettingsFormConstants.BUTTONS_SIZE),
                MinimumSize = new(SettingsFormConstants.BUTTONS_SIZE, SettingsFormConstants.BUTTONS_SIZE),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.Black, MouseDownBackColor = Color.LightGray },
                BackColor = Color.Transparent,
            };
            saveDefaultButton.Click += (sender, args) => formMacroSection.Binding.SaveToPresetConfiguration();
            ControlUtils.CreateStandardToolTip().SetToolTip(saveDefaultButton, Locale.CONFIG_LINE_SAVE_DEFAULT_TOOLTIP);


            FlowLayoutPanel buttonFlowPanel = new()
            {
                AutoSize = true,
                Anchor = AnchorStyles.None,
                BorderStyle = BorderStyle.None,
                Padding = Padding.Empty,
                Margin = new Padding(0, 0, 0, 10),
                Controls = { saveDefaultButton, toggleCommentsVisibilityButton }
            };
            return buttonFlowPanel;
        }

        public void Add(ListView listView, TableLayoutPanel panel)
        {
            foreach (var formMacroSection in this.macroSectionList)
            {
                listView.Items.Add(formMacroSection.ListItem);
                foreach (var formSection in formMacroSection.Sections)
                {
                    listView.Items.Add(formSection.ListItem);
                }
            }

            SettingsFormBindingControlLoader.ApplyControlPositions(panel, this.createdControlPositionList);
        }

        public void ClearAllLastOpenInformation()
        {
            foreach (var formMacroSection in this.macroSectionList)
            {
                SettingsFormLastOpenInformation.RemoveGuid(formMacroSection.Guid);
            }
        }
    }
}
