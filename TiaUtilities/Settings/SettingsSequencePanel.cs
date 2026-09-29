using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls.tableColorizable;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep.ControlFactory;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;
using static TiaUtilities.SettingsStep.SettingsSequencePanel;

namespace TiaUtilities.SettingsStep
{
    public class SettingsSequencePanel
    {
        public class TrasferToAllButton
        {
            private readonly Button control;
            private readonly ToolTip ToolTip = ControlUtils.CreateToolTip(quick: true);
            private EventHandler? click;

            public TrasferToAllButton()
            {
                Size maxSize = new(18, 18);

                ImageList imageList = new()
                {
                    Images = { ImageResources.TRANSFER },
                    ImageSize = new Size(maxSize.Width - 2, maxSize.Height - 2),
                };

                this.control = new()
                {
                    Anchor = AnchorStyles.None,

                    BackColor = Color.Transparent,
                    ImageList = imageList,
                    ImageIndex = 0,
                    TextImageRelation = TextImageRelation.ImageBeforeText,

                    Text = "",

                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Form.DefaultBackColor },

                    Visible = true,

                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,

                    MaximumSize = maxSize,

                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };
            }

            public void ShowAt(TableLayoutPanel panel, int column, int row,
                SettingsControl settingsControl, SettingsSequence sequence,
                ObservableConfiguration configuration, SettingsConfigurationProperty property)
            {
                this.Remove();

                var count = 0;
                if (settingsControl.ConfigurationTypeSequenceDict.TryGetValue(configuration.GetType(), out var sequences))
                {
                    count = sequences.Count(c => c != sequence);
                }

                if (count <= 0)
                {
                    return;
                }

                panel.Controls.Add(this.control, column, row);

                var caption = $"{Locale.SETTINGS_FORM_CONTEXT_MENU_SET_TO_OTHERS} ({count})";
                this.ToolTip.SetToolTip(this.control, caption);

                this.click = (sender, args) =>
                {
                    if (settingsControl.ConfigurationTypeSequenceDict.TryGetValue(configuration.GetType(), out var sequences))
                    {
                        var value = property.GetFrom(configuration);
                        if (value != null)
                        {
                            sequences.Where(s => sequence != s).ForEach(s => property.SetTo(s.Configuration, value));
                        }
                    }
                };
                this.control.Click += this.click;
            }

            public void Remove()
            {
                this.control.Parent?.Controls.Remove(this.control);

                if (this.click != null)
                {
                    this.control.Click -= this.click;
                    this.click = null;
                }

                this.ToolTip.Hide(this.control);
                this.ToolTip.RemoveAll();
            }
        }


        private readonly static TrasferToAllButton TRANSFER_TO_ALL_BUTTON = new();

        private const int VALUE_ROW_GAP = 4;
        private const int GROUP_ROW_GAP = 10;

        public SettingsSequence Sequence { get; init; }
        public SettingsSequencePanelDescriptor Descriptor { get; init; }

        public List<SettingsSequencePanelLine> Lines { get; init; }

        public bool ListenersRegistered { get; private set; } = false;

        private readonly ObservableConfiguration configuration;
        private readonly List<Predicate<PropertyChangedEventArgs>> propertyChangedPredicates;
        private readonly PropertyChangedEventHandler configurationPropertyChanged;

        private readonly List<RowStyle> tableRows;
        private readonly List<TableCellStyle> tableCellStyles;

        public SettingsSequencePanel(SettingsControl settingsControl, SettingsSequence sequence, SettingsSequencePanelDescriptor descriptor, ObservableConfiguration configuration)
        {
            this.Sequence = sequence;
            this.Descriptor = descriptor;

            this.configuration = configuration;
            this.propertyChangedPredicates = [];
            this.configurationPropertyChanged = (sender, args) => propertyChangedPredicates.ForEach(p => p.Invoke(args));

            (this.Lines, this.tableRows, this.tableCellStyles) = this.BuildControls(settingsControl);
        }

        public void RegisterListeners()
        {
            if (this.ListenersRegistered)
            {
                return;
            }

            this.configuration.PropertyChanged += configurationPropertyChanged;
            this.ListenersRegistered = true;
        }

        public void UnregisterListeners()
        {
            if (!this.ListenersRegistered)
            {
                return;
            }

            this.configuration.PropertyChanged -= configurationPropertyChanged;
            this.ListenersRegistered = false;
        }

        private (List<SettingsSequencePanelLine>, List<RowStyle>, List<TableCellStyle>) BuildControls(SettingsControl settingsControl)
        {
            const int COLUMN_VALUE_LABEL = 1;
            const int COLUMN_VALUE_CONTROL = 2;
            const int COLUMN_ICON = 3;

            const int COLUMN_START = 1;
            const int COLUMN_END = 3;

            List<SettingsSequencePanelLine> lines = [];
            List<RowStyle> rows = [];
            List<TableCellStyle> cellStyles = [];

            int rowCounter = 0;
            foreach (var group in this.Descriptor.GetGroups())
            {
                var factories = group.Factories;
                if (factories.Count == 0)
                {
                    continue;
                }

                rows.Add(new(SizeType.AutoSize)); //Group Label row

                var groupLabel = SettingsSequencePanel.CreateGroupLabel(group.Name);
                lines.Add(new()
                {
                    Name = "GroupLabel",
                    Main = new(groupLabel) { Column = COLUMN_START, Row = rowCounter, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
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
                    FitToControls = false,
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
                            Main = new(nameLabel) { Column = COLUMN_START, Row = rowCounter, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
                        });
                    }
                    else
                    {
                        this.propertyChangedPredicates.Add(predicate);

                        var savedRow = rowCounter;
                        cellStyles.Add(new()
                        {
                            Column = COLUMN_START,
                            ColumnSpan = COLUMN_END - COLUMN_START + 1,
                            Row = rowCounter,
                            BackColor = Color.Transparent, // Color.AntiqueWhite,
                            BorderColor = Color.Transparent,
                            BorderWidth = 0,
                            FitToControls = false,
                            Padding = Padding.Empty,
                            MouseEnterCallback = cellStyle =>
                            {
                                var panel = cellStyle.TableLayoutPanel;
                                if (panel != null && factory.ConfigurationProperty != null)
                                {
                                    SettingsSequencePanel.TRANSFER_TO_ALL_BUTTON.ShowAt(
                                        panel,
                                        COLUMN_ICON, savedRow,
                                        settingsControl,
                                        this.Sequence,
                                        this.configuration, 
                                        factory.ConfigurationProperty
                                    );
                                }

                                control.BackColor = Color.AntiqueWhite;
                                cellStyle.BackColor = Color.AntiqueWhite;
                            },
                            MouseLeaveCallback = cellStyle =>
                            {
                                SettingsSequencePanel.TRANSFER_TO_ALL_BUTTON.Remove();

                                control.BackColor = Form.DefaultBackColor;
                                cellStyle.BackColor = Color.Transparent;
                            },
                        });

                        SettingsSequencePanelLine line;
                        if (nameLabel == null)
                        {
                            line = new()
                            {
                                Name = factory.Name,
                                Main = new(control) { Column = COLUMN_VALUE_LABEL, Row = rowCounter, ColumnSpan = 3 },
                            };
                        }
                        else
                        {
                            line = new()
                            {
                                Name = factory.Name,
                                Label = new(nameLabel) { Column = COLUMN_VALUE_LABEL, Row = rowCounter },
                                Main = new(control) { Column = COLUMN_VALUE_CONTROL, Row = rowCounter },
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

            var divider = SettingsControls.GetDividerPanel(Color.Transparent);
            lines.Add(new()
            {
                Name = "Divider",
                Main = new(divider) { Column = COLUMN_VALUE_LABEL, Row = rowCounter + 1, ColumnSpan = COLUMN_END - COLUMN_START + 1 }
            });

            return (lines, rows, cellStyles);
        }

        public LabelColorizable CreateSelectControl()
        {
            var selectControl = CreateSelectLabelColorizable(this.Descriptor.Name, symbol: false);
            ControlUtils.CreateToolTip().SetToolTip(selectControl, this.Descriptor.Description);

            return selectControl;
        }

        public void ApplyControls(TableLayoutPanelColorizable panel)
        {
            foreach (var row in this.tableRows)
            {
                panel.RowStyles.Add(row);
            }

            this.Lines.ForEach(l => l.AddToPanel(panel));

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
                MouseHoverBackColor = Form.DefaultBackColor,
                MouseDownBackColor = Form.DefaultBackColor,

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

        public static LabelColorizable CreateSelectLabelColorizable(string text, bool symbol)
        {
            return new LabelColorizable()
            {
                BackColor = Color.Transparent,

                Cursor = symbol ? Cursors.Default : Cursors.Hand,

                MouseHoverBackColor = symbol ? Color.Transparent : Color.FromArgb(68, Color.CornflowerBlue),
                MouseDownBackColor = symbol ? Color.Transparent : Color.FromArgb(127, Color.CornflowerBlue),

                BorderWidth = 0,
                BorderColor = Color.FromArgb(100, Color.Black),

                Text = text,
                Font = symbol ? StyleManager.Fonts.BIG_BOLD : StyleManager.Fonts.NORMAL_SEMIBOLD,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,

                Padding = symbol ? Padding.Empty : new(4),
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