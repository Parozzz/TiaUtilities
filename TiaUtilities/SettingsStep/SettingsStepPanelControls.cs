using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsNew;
using TiaUtilities.SettingsStep.ControlFactory;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep
{
    public class SettingsStepPanelControls
    {

        private const int VALUE_ROW_GAP = 4;
        private const int GROUP_ROW_GAP = 10;

        public SettingsStepDescriptor Descriptor { get; init; }
        public LabelColorizable SectionLabel { get; init; }
        public Action? StepLabelClick { get; set; }

        public SettingsStepSequence Sequence { get; init; }
        public List<SettingsLineControls> Lines { get; init; }

        public bool ListenersRegistered { get; private set; } = false;

        private readonly ObservableConfiguration configuration;
        private readonly List<Predicate<PropertyChangedEventArgs>> propertyChangedPredicates;
        private readonly PropertyChangedEventHandler configurationPropertyChanged;

        private readonly List<RowStyle> tableRows;
        private readonly List<TableLayoutPanelColorizable.CellStyle> tableCellStyles;

        public SettingsStepPanelControls(SettingsStepForm form, SettingsStepSequence sequence, SettingsStepDescriptor descriptor, ObservableConfiguration configuration)
        {
            this.Sequence = sequence;
            this.Descriptor = descriptor;

            this.configuration = configuration;
            this.propertyChangedPredicates = [];
            this.configurationPropertyChanged = (sender, args) => propertyChangedPredicates.ForEach(p => p.Invoke(args));

            (this.SectionLabel, this.Lines, this.tableRows, this.tableCellStyles) = this.BuildControls(form);
        }

        public void RegisterListeners()
        {
            if (this.ListenersRegistered)
            {
                return;
            }

            configuration.PropertyChanged += configurationPropertyChanged;
            this.ListenersRegistered = true;
        }

        private (LabelColorizable, List<SettingsLineControls>, List<RowStyle>, List<TableLayoutPanelColorizable.CellStyle>) BuildControls(SettingsStepForm form)
        {
            const int COLUMN_VALUE_LABEL = 1;
            const int COLUMN_VALUE_CONTROL = 2;
            const int COLUMN_ICON = 3;

            const int COLUMN_START = 1;
            const int COLUMN_END = 3;


            //tablePanel.HandleCreated += (sender, args) => tablePanel.AutoScroll = true;

            var groups = this.Descriptor.GetGroups();

            List<SettingsLineControls> lines = [];
            List<RowStyle> rows = [];
            List<TableLayoutPanelColorizable.CellStyle> cellStyles = [];

            int rowCounter = 0;

            foreach (var group in groups)
            {
                var factories = group.Factories;
                if (factories.Count == 0)
                {
                    continue;
                }

                rows.Add(new(SizeType.AutoSize)); //Group Label row

                var groupLabel = SettingsStepPanelControls.CreateGroupLabel(group.Name);
                lines.Add(new()
                {
                    Name = "GroupLabel",
                    MainControl = new(groupLabel) { Column = COLUMN_START, Row = rowCounter, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
                });
                rowCounter += 1;

                cellStyles.Add(new()
                {
                    Column = COLUMN_START,
                    Row = rowCounter,
                    BackColor = Color.Transparent,
                    BorderColor = Color.DarkGray,
                    BorderWidth = 1,
                    ColumnSpan = COLUMN_END - COLUMN_START + 1,
                    RowSpan = factories.Count * 2,
                    FitToControls = true,
                    Padding = new Padding(6, 6, 8, 6),
                });

                Enumerable.Range(1, factories.Count * 2)
                    .Select(i => i % 2 == 0 ? new RowStyle(SizeType.Absolute, VALUE_ROW_GAP) : new RowStyle(SizeType.AutoSize))
                    .ForEach(rows.Add); //*2 for value and padding.

                foreach (var factory in factories)
                {
                    var (nameLabel, control, predicate) = factory.Create(configuration, new()
                    {
                        PlaceholdersCallback = str => this.Sequence.PlaceholdersCallBack?.Invoke(str) ?? str
                    });

                    if (control == null || predicate == null) //If no predicate is provided, means is not a binded control and will not have a label
                    {
                        if (nameLabel == null)
                        { //If everything is null, i ignore the whole factory. It should never happen though.
                            continue;
                        }

                        lines.Add(new()
                        {
                            Name = factory.Name,
                            MainControl = new(nameLabel) { Column = COLUMN_START, Row = rowCounter, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
                        });
                    }
                    else
                    {
                        this.propertyChangedPredicates.Add(predicate);

                        Button transferToAllButton = new()
                        {
                            Dock = DockStyle.Fill,
                            BackColor = Color.Transparent,
                            BackgroundImage = null,
                            BackgroundImageLayout = ImageLayout.Zoom,
                            Text = "",
                            FlatStyle = FlatStyle.Flat,
                            FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Color.Transparent },
                            Visible = true,
                            AutoSize = false,
                            AutoSizeMode = AutoSizeMode.GrowAndShrink,
                            MinimumSize = new(20, 20),
                            Padding = new(4)
                        };

                        var tooltip = ControlUtils.CreateStandardToolTip();
                        transferToAllButton.MouseHover += (sender, args) =>
                        {
                            var tryGetOK = form.ConfigurationTypeModelDict.TryGetValue(this.configuration.GetType(), out var containers);
                            var count = tryGetOK && containers != null ? containers.Count(c => c != this.Sequence) : 0;

                            var caption = $"{Locale.SETTINGS_FORM_CONTEXT_MENU_SET_TO_OTHERS} ({count})";
                            tooltip.Show(caption, transferToAllButton);
                        };

                        cellStyles.Add(new()
                        {
                            Column = COLUMN_START,
                            ColumnSpan = COLUMN_END - COLUMN_START + 1,
                            Row = rowCounter,
                            BackColor = Color.Transparent, // Color.AntiqueWhite,
                            BorderColor = Color.Transparent,
                            BorderWidth = 0,
                            FitToControls = true,
                            Padding = Padding.Empty,
                            MouseEnterCallback = cellStyle =>
                            {
                                if (transferToAllButton.BackgroundImage == null)
                                {
                                    var tryGetOK = form.ConfigurationTypeModelDict.TryGetValue(this.configuration.GetType(), out var containers);
                                    var count = tryGetOK && containers != null ? containers.Count(c => c != this.Sequence) : 0;

                                    if (count > 0)
                                    {
                                        transferToAllButton.BackgroundImage = ImageResources.TRANSFER;
                                        transferToAllButton.FlatAppearance.MouseOverBackColor = Form.DefaultBackColor;
                                    }
                                }

                                control.BackColor = Color.AntiqueWhite;
                                cellStyle.BackColor = Color.AntiqueWhite;
                            },
                            MouseLeaveCallback = cellStyle =>
                            {
                                if (transferToAllButton.BackgroundImage != null)
                                {
                                    transferToAllButton.BackgroundImage = null;
                                    transferToAllButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
                                }

                                control.BackColor = Form.DefaultBackColor;
                                cellStyle.BackColor = Color.Transparent;
                            },
                        });

                        SettingsLineControls line;
                        if (nameLabel == null)
                        {
                            line = new()
                            {
                                Name = factory.Name,
                                MainControl = new(control) { Column = COLUMN_VALUE_LABEL, Row = rowCounter, ColumnSpan = 3 },
                            };
                        }
                        else
                        {
                            line = new()
                            {
                                Name = factory.Name,
                                Label = new(nameLabel) { Column = COLUMN_VALUE_LABEL, Row = rowCounter },
                                MainControl = new(control) { Column = COLUMN_VALUE_CONTROL, Row = rowCounter },
                                Buttons = {
                                    new(transferToAllButton) { Column = COLUMN_ICON, Row = rowCounter }
                                }
                            };
                        }

                        line.ContextPhrases.AddRange([this.Sequence.FullName, this.Descriptor.Name, group.Name]);
                        lines.Add(line);
                    }

                    rowCounter += 2; //Skip 2 = Value row and padding
                }

                rows.Add(new(SizeType.Absolute, GROUP_ROW_GAP));
                rowCounter += 1; //Skip 1 = Padding row

            }

            var divider = SettingsControls.GetDividerLabel(Color.Transparent);
            lines.Add(new()
            {
                Name = "Divider",
                MainControl = new(divider) { Column = COLUMN_VALUE_LABEL, Row = rowCounter + 1, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
            });


            var sectionLabel = CreateSectionLabel(this.Descriptor.Name, arrow: false);
            sectionLabel.Click += (sender, args) => this.StepLabelClick?.Invoke();

            ControlUtils.CreateToolTip().SetToolTip(sectionLabel, this.Descriptor.Description);

            return (sectionLabel, lines, rows, cellStyles);
        }

        public void ApplyControls(TableLayoutPanelColorizable panel)
        {
            foreach (var row in this.tableRows)
            {
                panel.RowStyles.Add(row);
            }

            this.Lines.ForEach(l => l.AddAllControls(panel));
            this.Lines.ForEach(l => l.SetAllPositionsToPanel(panel));

            foreach (var style in this.tableCellStyles)
            {
                panel.AddCellStyle(style);
            }
        }

        private static Label CreateGroupLabel(string text)
        {
            var emptyText = string.IsNullOrEmpty(text);
            return new LabelColorizable()
            {
                BackColor = Form.DefaultBackColor,
                HoverColor = Form.DefaultBackColor,
                ClickedColor = Form.DefaultBackColor,

                BorderColor = ControlPaint.Dark(Color.DarkGray, 0.2f),
                BorderWidth = emptyText ? 0 : 1,

                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                //Dock = DockStyle.Fill,
                AutoSize = true,
                Font = StyleManager.Fonts.BIG_BOLD,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = emptyText ? Padding.Empty : new(5, 2, 5, 2),
                Margin = emptyText ? Padding.Empty : new(2),
            };
        }

        public static LabelColorizable CreateSectionLabel(string text, bool arrow)
        {
            return new LabelColorizable()
            {
                BackColor = Color.Transparent,

                Cursor = arrow ? Cursors.Default : Cursors.Hand,

                HoverColor = arrow ? Color.Transparent : Color.FromArgb(68, Color.CornflowerBlue),
                ClickedColor = arrow ? Color.Transparent : Color.FromArgb(127, Color.CornflowerBlue),

                BorderWidth = 0,
                BorderColor = Color.FromArgb(100, Color.Black),

                Text = text,
                Font = arrow ? StyleManager.Fonts.NORMAL_SEMIBOLD : StyleManager.Fonts.BIG_SEMIBOLD,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,

                Padding = arrow ? Padding.Empty : new(4),
                Margin = Padding.Empty,
            };
        }

    }
}

/*
 
         private static Button CreateQuestionMarkButton()
        {
            Button button = new()
            {
                BackColor = Color.Transparent,
                ForeColor = Color.SlateGray,
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = Size.Empty,
                MaximumSize = new(12, 18),
                Font = StyleManager.Fonts.SMALL_SEMIBOLD,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent, BorderColor = Form.DefaultBackColor, CheckedBackColor = Color.Transparent },
                TextAlign = ContentAlignment.TopCenter,
                //BackgroundImage = ImageResources.QUESTION_MARK_8412400_007435,
                //BackgroundImageLayout = ImageLayout.Zoom,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Text = "❓",
            };
            button.MouseEnter += (sender, args) => button.ForeColor = Color.MediumSeaGreen;
            button.MouseLeave += (sender, args) => button.ForeColor = Color.SlateGray;
            ControlUtils.SetDoubleBuffered(button);
            return button;
        }

 */