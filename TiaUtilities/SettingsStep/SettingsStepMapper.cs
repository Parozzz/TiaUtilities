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
    public class SettingsStepMapper
    {
        private const int VALUE_ROW_GAP = 4;
        private const int GROUP_ROW_GAP = 10;

        public SettingsStepContext Context { get; init; }

        public LabelColorizable StepLabel { get; init; }
        public Control Control { get; init; }
        public bool ListenersRegistered { get; private set; } = false;

        private readonly SettingsStepContainer container;
        private readonly ObservableConfiguration configuration;
        private readonly List<Predicate<PropertyChangedEventArgs>> propertyChangedPredicates;
        private readonly PropertyChangedEventHandler configurationPropertyChanged;


        public SettingsStepMapper(SettingsStepForm form, SettingsStepContainer container, SettingsStepContext context, ObservableConfiguration configuration)
        {
            this.container = container;
            this.Context = context;

            this.configuration = configuration;
            this.propertyChangedPredicates = [];
            this.configurationPropertyChanged = (sender, args) => propertyChangedPredicates.ForEach(p => p.Invoke(args));

            (this.StepLabel, this.Control) = this.BuildControls(form);
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

        private (LabelColorizable, Control) BuildControls(SettingsStepForm form)
        {
            const int COLUMN_VALUE_LABEL = 1;
            const int COLUMN_VALUE_CONTROL = 2;
            const int COLUMN_ICON = 3;

            const int COLUMN_START = 1;
            const int COLUMN_END = 3;

            TableLayoutPanelNoScrollbarsColorizable tablePanel = new()
            {
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 5,
                ColumnStyles = {
                    new(SizeType.Percent, 50f), //For centering the table in the panel
                    new(SizeType.AutoSize), //Value Name Label
                    new(SizeType.AutoSize), //Value Control
                    new(SizeType.Absolute, 25f),
                    new(SizeType.Percent, 50f)  //For centering the table in the panel
                },
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new(10),
            };
            //tablePanel.HandleCreated += (sender, args) => tablePanel.AutoScroll = true;

            var groups = this.Context.GetGroups();

            Dictionary<Control, ControlPosition> positionDict = [];

            int rowCounter = 0;

            foreach (var group in groups)
            {
                var factories = group.Factories;
                if (factories.Count == 0)
                {
                    continue;
                }

                tablePanel.RowStyles.Add(new(SizeType.AutoSize)); //Group Label row

                Enumerable.Range(1, factories.Count * 2)
                    .Select(i => i % 2 == 0 ? new RowStyle(SizeType.Absolute, VALUE_ROW_GAP) : new RowStyle(SizeType.AutoSize))
                    .ForEach(r => tablePanel.RowStyles.Add(r)); //*2 for value and padding.

                tablePanel.RowStyles.Add(new(SizeType.Absolute, GROUP_ROW_GAP));

                var groupLabel = SettingsStepMapper.CreateGroupLabel(group.Name);
                positionDict.Add(groupLabel, new()
                {
                    Column = COLUMN_START,
                    Row = rowCounter,
                    ColumnSpan = COLUMN_END - COLUMN_START + 1
                });
                rowCounter += 1;

                tablePanel.AddCellStyle(new()
                {
                    Column = COLUMN_START,
                    Row = rowCounter,
                    BackColor = Color.Transparent,
                    BorderColor = Color.DarkBlue,
                    BorderWidth = 1,
                    ColumnSpan = COLUMN_END - COLUMN_START + 1,
                    RowSpan = factories.Count * 2,
                    FitToControls = true,
                    FitToControlsPadding = new Padding(6, 6, 8, 6),
                });

                foreach (var factory in factories)
                {
                    var (nameLabel, control, predicate) = factory.Create(configuration);

                    if (control == null || predicate == null) //If no predicate is provided, means is not a binded control and will not have a label
                    {
                        if (nameLabel == null)
                        { //If everything is null, i ignore the whole factory. It should never happen though.
                            continue;
                        }

                        positionDict.Add(nameLabel, new()
                        {
                            Column = COLUMN_START,
                            Row = rowCounter,
                            ColumnSpan = COLUMN_END - COLUMN_START + 1,
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
                            var tryGetOK = form.ConfigurationTypeContainerDict.TryGetValue(this.configuration.GetType(), out var containers);
                            var count = tryGetOK && containers != null ? containers.Count(c => c != this.container) : 0;

                            var caption = $"{Locale.SETTINGS_FORM_CONTEXT_MENU_SET_TO_OTHERS} ({count})";
                            tooltip.Show(caption, transferToAllButton);
                        };


                        void MouseEnter(TableLayoutPanelNoScrollbarsColorizable.CellStyle cellStyle)
                        {
                            if (transferToAllButton.BackgroundImage == null)
                            {
                                var tryGetOK = form.ConfigurationTypeContainerDict.TryGetValue(this.configuration.GetType(), out var containers);
                                var count = tryGetOK && containers != null ? containers.Count(c => c != this.container) : 0;

                                if (count > 0)
                                {
                                    transferToAllButton.BackgroundImage = ImageResources.TRANSFER;
                                    transferToAllButton.FlatAppearance.MouseOverBackColor = Form.DefaultBackColor;
                                }
                            }

                            control.BackColor = Color.AntiqueWhite;
                            cellStyle.BackColor = Color.AntiqueWhite;
                        }

                        void MouseLeave(TableLayoutPanelNoScrollbarsColorizable.CellStyle cellStyle)
                        {
                            if (transferToAllButton.BackgroundImage != null)
                            {
                                transferToAllButton.BackgroundImage = null;
                                transferToAllButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
                            }

                            control.BackColor = Form.DefaultBackColor;
                            cellStyle.BackColor = Color.Transparent;
                        }

                        TableLayoutPanelNoScrollbarsColorizable.CellStyle backColorCellStyle = new()
                        {
                            Column = COLUMN_START,
                            ColumnSpan = COLUMN_END - COLUMN_START + 1,
                            Row = rowCounter,
                            BackColor = Color.Transparent, // Color.AntiqueWhite,
                            BorderColor = Color.Transparent,
                            BorderWidth = 0,
                            FitToControls = true,
                            FitToControlsPadding = Padding.Empty,
                            MouseEnterCallback = MouseEnter,
                            MouseLeaveCallback = MouseLeave,
                        };
                        tablePanel.AddCellStyle(backColorCellStyle);

                        if (nameLabel == null)
                        {
                            positionDict.Add(control, new() { Column = COLUMN_VALUE_LABEL, ColumnSpan = 3, Row = rowCounter });
                        }
                        else
                        {
                            positionDict.Add(nameLabel, new() { Column = COLUMN_VALUE_LABEL, Row = rowCounter });
                            positionDict.Add(control, new() { Column = COLUMN_VALUE_CONTROL, Row = rowCounter });
                            positionDict.Add(transferToAllButton, new() { Column = COLUMN_ICON, Row = rowCounter });
                        }
                    }

                    rowCounter += 2; //Skip 2 = Value row and padding
                }

                rowCounter += 1; //Skip 1 = Padding row

            }

            positionDict.Add(SettingsControls.GetDividerLabel(Color.Transparent),
                new()
                {
                    Column = COLUMN_VALUE_LABEL,
                    Row = rowCounter + 1,
                    ColumnSpan = tablePanel.ColumnCount
                }
            );

            tablePanel.Controls.AddRange([.. positionDict.Keys]);

            foreach (var (control, pos) in positionDict)
            {
                tablePanel.SetCellPosition(control, new(pos.Column, pos.Row));
                if (pos.ColumnSpan > 0)
                {
                    tablePanel.SetColumnSpan(control, pos.ColumnSpan);
                }

                if (pos.RowSpan > 0)
                {
                    tablePanel.SetRowSpan(control, pos.RowSpan);
                }
            }

            var stepLabel = CreateStepLabel(this.Context.Name, arrow: false);
            ControlUtils.CreateStandardToolTip().SetToolTip(stepLabel, this.Context.Description);

            Panel middlePanel = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Controls = { tablePanel },
            };
            ControlUtils.SetDoubleBuffered(middlePanel);

            ScrollableControl scrollableControl = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new(0, 0),
                AutoScrollMargin = new(0, 0),
                Controls = { middlePanel },
            };
            ControlUtils.SetDoubleBuffered(scrollableControl);

            return (stepLabel, scrollableControl);
        }

        private static Label CreateGroupLabel(string text)
        {
            return new LabelColorizable()
            {
                BackColor = Form.DefaultBackColor,
                HoverColor = Form.DefaultBackColor,
                ClickedColor = Form.DefaultBackColor,

                BorderColor = ControlPaint.LightLight(Color.DarkBlue),
                BorderWidth = 1,

                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                //Dock = DockStyle.Fill,
                AutoSize = true,
                Font = StyleManager.Fonts.BIG_BOLD,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = Padding.Empty,
                Margin = new(1),
            };
        }

        public static LabelColorizable CreateStepLabel(string text, bool arrow)
        {
            return new LabelColorizable()
            {
                BackColor = Color.Transparent,

                HoverColor = arrow ? Color.Transparent : Color.FromArgb(68, Color.CornflowerBlue),
                ClickedColor = arrow ? Color.Transparent : Color.FromArgb(127, Color.CornflowerBlue),

                BorderWidth = 0,
                BorderColor = Color.Transparent,

                Text = text,
                Font = arrow ? StyleManager.Fonts.NORMAL_BOLD : StyleManager.Fonts.BIG_BOLD,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Padding = arrow ? Padding.Empty : new(4),
                Margin = Padding.Empty,
            };
        }

        public class ControlPosition
        {
            public required int Column { get; init; }
            public required int Row { get; init; }
            public int ColumnSpan { get; set; } = 0;
            public int RowSpan { get; set; } = 0;

            public override string ToString() => $"C-{Column}, CS-{ColumnSpan}, R-{Row}, RS-{RowSpan}";
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