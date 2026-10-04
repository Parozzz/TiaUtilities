using System.Data;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Settings;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Settings
{
    public partial class SettingsControl : UserControl, IMessageFilter
    {
        private class ComboBoxSourceItem
        {
            public required string Text { get; init; }
            public required SettingsSequence Sequence { get; init; }

            public override string ToString()
            {
                return this.Text;
            }
        }

        private class SequencePanelMetadata
        {
            public required SettingsSequencePanel Panel { get; init; }
            public required ObservableObjectChangedEventHandler<bool> PanelEnabledChanged { get; init; }

            public required LabelColorizable SelectControl { get; init; }
            public required EventHandler SelectClick { get; init; }


            public void DisposeSingleUse()
            {
                this.Panel.Enabled.Changed -= PanelEnabledChanged;

                this.SelectControl.Click -= this.SelectClick;
                this.SelectControl.Dispose();

            }
        }


        private const int STEP_PANEL_ROW = 1;
        private const int CONTROLS_PANEL_ROW = 2;

        private const string ARROW_RIGHT = "↦";
        private const string HEAVY_VERTICAL_BAR = "❚";
        private const string RIGHT_TRIANGLE = "▶";
        private const string MIDDLE_DOT = "•";
        private const string CHEVRON_SINGLE_RIGHT = "›";
        private const string CHEVRON_DOUBLE_RIGHT = "»";
        private const string STAR_FILLED = "★";
        private const string STAR_EMPTY = "☆";

        private const string PREVIOUS_ARROW = "⇦";
        private const string NEXT_ARROW = "⇨";

        private const int WS_EX_COMPOSITED = 0x02000000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Attiva lo stile WS_EX_COMPOSITED (0x02000000)
                // Costringe Windows a ridisegnare tutti i controlli figli bottom-up in un unico buffer
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        internal Dictionary<Type, List<SettingsSequence>> ConfigurationTypeSequenceDict { get; init; }

        private readonly ObservableObject<SettingsSequence?> selectedSequence;
        private readonly ObservableObject<SettingsSequencePanel?> selectedPanel;
        private readonly ObservableObject<bool> searchMode;

        private readonly List<SettingsSequence> sequences;

        private readonly SettingsSearchPanel searchPanel;
        private readonly List<SequencePanelMetadata> sequencePanelMetadataList;

        private bool _initialized = false;

        public SettingsControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            ControlUtils.SetDoubleBuffered(this.mainTable);
            ControlUtils.SetDoubleBuffered(this.bottomPanel);
            ControlUtils.SetDoubleBuffered(this.sequenceButtonsPanel);

            this.sequences = [];
            this.ConfigurationTypeSequenceDict = [];

            this.selectedPanel = new(null);
            this.selectedSequence = new(null);
            this.searchMode = new(false);

            this.searchPanel = new(this.searchPanelControl);
            this.searchPanel.InitPanel();

            this.sequencePanelMetadataList = [];
        }


        public void InitControls()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            #region MAIN_TABLE_CELL_STYLES
            this.mainTable.AddCellStyle(new()
            {
                Column = 0,
                Row = STEP_PANEL_ROW,
                BorderWidth = 2,
                BorderRadius = new(5),
                BorderColor = Color.FromArgb(64, Color.LightSkyBlue),
                Padding = new(-1)
            });

            this.mainTable.AddCellStyle(new()
            {
                Column = 0,
                Row = STEP_PANEL_ROW,
                RowSpan = 2,
                BorderWidth = 2,
                BorderRadius = new(5),
                BorderColor = Color.FromArgb(127, Color.LightSkyBlue),
                Padding = new(-1),
            });
            #endregion

            #region TOGGLE_MODE_BUTTON
            ImageList toggleImageList = new()
            {
                Images = { ImageResources.DROPDOWN, ImageResources.SEARCH },
                ImageSize = new(22, 22),
            };

            this.toggleModeButton.Text = "";

            this.toggleModeButton.ImageList = toggleImageList;
            this.toggleModeButton.ImageIndex = 0;
            this.toggleModeButton.ImageAlign = ContentAlignment.MiddleCenter;

            this.toggleModeButton.Click += (sender, args) =>
            {
                this.searchMode.Value = !this.searchMode.Value;
                this.toggleModeButton.ImageIndex = this.searchMode.Value ? 1 : 0;
            };
            #endregion

            #region CONTROLS_PANEL_COLUMNS
            this.controlsPanel.RowStyles.Clear();
            this.controlsPanel.ColumnStyles.Clear();

            this.controlsPanel.ColumnCount = 5;

            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Name Label
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Control
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Absolute, 25f)); //Button
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            #endregion

            #region SELECTED_SEQUENCE_CHANGED
            this.selectedSequence.Changed += (sender, args) =>
            {
                var oldSequence = args.OldValue;
                var newSequence = args.NewValue;

                this.sequenceButtonsPanel.SuspendLayout();

                this.sequenceButtonsPanel.Controls.Cast<Control>().Where(c => String.Equals(c.Tag, "Symbol")).ForEach(c => c.Dispose());
                this.sequencePanelMetadataList.ForEach(m => m.DisposeSingleUse()); //Avoid Memory leak

                this.sequenceButtonsPanel.Controls.Clear();
                this.sequencePanelMetadataList.Clear();

                var oldPanel = this.selectedPanel.Value;
                var oldPanelName = oldPanel?.Descriptor.Name;
                this.selectedPanel.Value = null;

                if (newSequence != null)
                {
                    List<Control> selectControls = [];
                    foreach (var panel in newSequence.Panels)
                    {
                        void click(object? sender, EventArgs args) => this.selectedPanel.Value = panel;

                        var selectControl = panel.CreateSelectControl();
                        selectControl.Enabled = panel.Enabled.Value;
                        selectControl.Click += click;

                        void enabledChanged(object? sender, ObservableObjectChangedEventArgs<bool> args) => selectControl.Enabled = args.NewValue;
                        panel.Enabled.Changed += enabledChanged;

                        SequencePanelMetadata metadata = new()
                        {
                            Panel = panel,
                            PanelEnabledChanged = enabledChanged,

                            SelectControl = selectControl,
                            SelectClick = click,
                        };
                        this.sequencePanelMetadataList.Add(metadata);

                        selectControls.Add(selectControl);
                    }

                    var sequenceNameLabel = new LabelWithSymbols()
                    {//Selected sequence name label with a star icon to save as default configuration
                        BackColor = Color.Transparent,

                        Text = newSequence.FullName,
                        Font = StyleManager.Fonts.BIG_SEMIBOLD,
                        TextAlign = ContentAlignment.MiddleCenter,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        BorderStyle = BorderStyle.None,

                        Padding = new(4, 4, 0, 4), //Fo
                        Margin = Padding.Empty,

                        SymbolSizePercent = 0.82f,
                        Items =
                        {
                            new()
                            {
                                Symbol = STAR_FILLED,
                                TooltipTextCallback = () =>
                                {
                                    var selectedSequence = this.selectedSequence.Value;
                                    return Locale.SETTINGS_CONTROL_SAVE_AS_DEFAULT
                                                .Replace("{name}", selectedSequence?.Name)
                                                .Replace("{type}", selectedSequence?.Configuration.GetType().Name);;
                                },
                                Color = Color.Gray,
                                HoverColor = Color.Green,
                                ClickedColor = Color.DarkGoldenrod,
                                ClickedColorDuration = 500,
                                OnClick = item => SaveSelectedSequenceConfigurationAsDefault()
                            }
                        }
                    };
                    var nameSpacerSymbolLabel = SettingsSequencePanel.CreateSelectLabelColorizable(CHEVRON_DOUBLE_RIGHT, symbol: true);

                    IEnumerable<Control> l = selectControls.SelectMany((x, index) =>
                        index < selectControls.Count - 1 ?
                        new[] { x, SettingsSequencePanel.CreateSelectLabelColorizable(MIDDLE_DOT, symbol: true) } :
                        [x]
                    );
                    this.sequenceButtonsPanel.Controls.AddRange([sequenceNameLabel, nameSpacerSymbolLabel, .. l]);

                    SettingsSequencePanel? activePanel = null;
                    if (!string.IsNullOrEmpty(oldPanelName))
                    {
                        activePanel = newSequence.Panels.FirstOrDefault(p => p.Descriptor.Name.Contains(oldPanelName, StringComparison.OrdinalIgnoreCase));
                    }
                    this.selectedPanel.Value = activePanel ?? newSequence.Panels.FirstOrDefault();
                }

                this.sequenceButtonsPanel.ResumeLayout();
            };
            #endregion

            #region SELECTED_PANEL_CHANGED
            this.selectedPanel.Changed += (sender, args) =>
            {
                var oldPanel = args.OldValue;
                var newPanel = args.NewValue;

                this.controlsPanel.SuspendLayout();

                this.controlsPanel.RowStyles.Clear();
                this.controlsPanel.Controls.Clear();
                this.controlsPanel.ClearCellStyles();

                foreach (var metadata in this.sequencePanelMetadataList)
                {
                    if (metadata.Panel == oldPanel)
                    {
                        metadata.SelectControl.BorderWidth = 0;
                    }
                    else if (metadata.Panel == newPanel)
                    {
                        metadata.SelectControl.BorderWidth = 1;
                        newPanel.ApplyControls(this.controlsPanel);
                    }
                }

                this.controlsPanel.ResumeLayout();
            };
            #endregion

            #region SELECT_SEQUENCE_COMBOBOX
            this.selectSequenceComboBox.BackColor = Form.DefaultBackColor;
            this.selectSequenceComboBox.Font = StyleManager.Fonts.NORMAL_SEMIBOLD;

            this.selectSequenceComboBox.AutoWidthFromItems = true;
            this.selectSequenceComboBox.AutoWidthRightPadding = 20;

            this.selectSequenceComboBox.DisplayMember = nameof(ComboBoxSourceItem.Text);
            this.selectSequenceComboBox.ValueMember = nameof(ComboBoxSourceItem.Sequence);
            this.selectSequenceComboBox.FilterPredicate = (obj, text) =>
            {
                return obj is ComboBoxSourceItem item && SettingsControl.CalculateMatchCustom(item.Text, text);
            };
            this.selectSequenceComboBox.SelectionChangeCommitted += (sender, args) => UpdateSelectedSequenceFromComboBox();

            this.selectSequenceComboBox.DropDownClosed += (sender, args) =>
            {//Remove focus on ComboBox after closing drop down to avoid having it selected (Annoying).
                this.BeginInvoke(() => this.ActiveControl = null);
            };

            ControlUtils.CreateToolTip(quick: true).SetToolTip(this.selectSequenceComboBox, "CTRL|PAG-UP/DOWN");
            #endregion

            #region SEARCH_TEXT_BOX
            this.searchTextBox.BackColor = Form.DefaultBackColor;
            this.searchTextBox.LostFocus += (sender, args) =>
            {
                this.searchPanel.UpdateSearchText(this.sequences, this.searchTextBox.Text);
            };

            this.searchTextBox.KeyDown += (sender, args) =>
            {
                if (args.KeyData == Keys.Enter || args.KeyData == Keys.Tab)
                {
                    this.searchPanel.UpdateSearchText(this.sequences, this.searchTextBox.Text);
                }
            };
            #endregion

            #region SEARCH_MODE
            void UpdateVisibilityForSearchMode(bool searchModeActive)
            {
                this.SuspendLayout();

                if (searchModeActive)
                {
                    this.selectSequenceComboBox.Visible = false;
                    this.selectConfigurationLabel.Visible = false;
                    this.sequenceButtonsPanel.Visible = false;
                    this.controlsPanel.Visible = false;

                    this.selectedSequence.Value = null;
                    this.selectedPanel.Value = null;

                    this.searchLabel.Visible = true;
                    this.searchTextBox.Visible = true;
                    this.searchPanelControl.Visible = true;
                }
                else
                {
                    this.selectSequenceComboBox.Visible = true;
                    this.selectConfigurationLabel.Visible = true;
                    this.sequenceButtonsPanel.Visible = true;
                    this.controlsPanel.Visible = true;

                    if (this.selectSequenceComboBox.SelectedItem is ComboBoxSourceItem item)
                    {
                        this.selectedSequence.Value = item.Sequence;
                    }

                    this.searchLabel.Visible = false;
                    this.searchTextBox.Visible = false;
                    this.searchPanelControl.Visible = false;
                    this.searchPanel.Clear();
                }

                this.ResumeLayout();
            }

            UpdateVisibilityForSearchMode(this.searchMode.Value);
            this.searchMode.Changed += (sender, args) => UpdateVisibilityForSearchMode(args.NewValue);
            #endregion

            this.Translate();
        }

        private void Translate() 
        {
            this.selectConfigurationLabel.Text = Locale.SETTINGS_CONTROL_LABEL_SELECT_CONFIGURATION;
            this.searchLabel.Text = Locale.SETTINGS_CONTROL_LABEL_SEARCH;
        }

        public void SelectSequenceFromName(string name)
        {
            ComboBoxSourceItem? found = null;
            foreach (ComboBoxSourceItem item in this.selectSequenceComboBox.Items)
            {
                if (item.Text.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    found = item;
                    break;
                }
            }

            if (found != null)
            {
                this.selectSequenceComboBox.SelectedItem = found;
                this.UpdateSelectedSequenceFromComboBox();
            }
        }

        private void UpdateSelectedSequenceFromComboBox()
        {
            if (this.selectSequenceComboBox.SelectedItem is ComboBoxSourceItem item)
            {
                this.selectedSequence.Value = item.Sequence;
            }
        }

        public void SetSequences(IEnumerable<SettingsSequence> sequences)
        {
            this.searchMode.Value = false;

            this.sequences.ForEach(s => s.DisposeAll());
            this.sequences.Clear();

            this.searchPanel.DisposeControls();
            this.searchPanel.Clear();

            this.ConfigurationTypeSequenceDict.Clear();

            if (!sequences.Any())
            {
                this.selectedSequence.Value = null;
                return;
            }

            foreach (var sequence in sequences)
            {
                sequence.Init(this);

                var cfgType = sequence.Configuration.GetType();

                var tryGetOK = this.ConfigurationTypeSequenceDict.TryGetValue(cfgType, out var configurationSequences);
                if (!tryGetOK)
                {
                    configurationSequences = [];
                    this.ConfigurationTypeSequenceDict.Add(cfgType, configurationSequences);
                }

                configurationSequences?.Add(sequence);
            }

            var items = sequences.Select(s => new ComboBoxSourceItem() { Text = s.FullName, Sequence = s });
            this.selectSequenceComboBox.DataSource = items;

            this.sequences.AddRange(sequences);
            this.selectedSequence.Value = sequences.FirstOrDefault();
        }

        private static bool CalculateMatchCustom(string text, string searchText)
        {
            string[] searchWords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool matchAll = searchWords.All(parola => text.Contains(parola, StringComparison.OrdinalIgnoreCase));

            if (matchAll)
            {
                return true;
            }

            string acronym = new(
                text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 0 && char.IsLetterOrDigit(w[0]))
                    .Select(w => w[0])
                    .ToArray()
            );

            return acronym.StartsWith(searchText, StringComparison.OrdinalIgnoreCase);
        }

        private void SaveSelectedSequenceConfigurationAsDefault()
        {
            var selectedSequence = this.selectedSequence.Value;
            if (selectedSequence != null)
            {
                var configuration = selectedSequence.Configuration;

                var presetConfiguration = MainForm.Settings.GetPresetConfiguration(configuration.GetType());
                if (presetConfiguration != null)
                {
                    GenUtils.CopySamePublicFieldsAndProperties(configuration, presetConfiguration);
                }
            }
        }

        private void SelectNextPrevious(bool next)
        {
            var sequence = this.selectedSequence.Value;
            var panelControls = this.selectedPanel.Value;
            if (sequence == null || panelControls == null)
            {
                return;
            }

            var count = sequence.Panels.Count;

            var indexOf = sequence.Panels.IndexOf(panelControls);

            int newIndex = 0;
            if (next)
            {
                newIndex = indexOf >= count - 1 ? 0 : indexOf + 1;
            }
            else
            {
                newIndex = indexOf <= 0 ? count - 1 : indexOf - 1;
            }
            this.selectedPanel.Value = sequence.Panels[newIndex];

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!this.DesignMode)
            {
                Application.AddMessageFilter(this);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (!this.DesignMode)
            {
                Application.RemoveMessageFilter(this);
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != DllImports.WM_KEYDOWN || !this.Visible)
            {
                return false;
            }

            var point = this.PointToClient(Cursor.Position);
            var enable = this.DisplayRectangle.Contains(point) || this.ContainsFocus;
            if (!enable)
            {
                return false;
            }

            Keys keyData = (Keys)(int)m.WParam | Control.ModifierKeys;
            if (keyData == Keys.PageUp || keyData == (Keys.Right | Keys.Control))
            {
                SelectNextPrevious(true);
                return true;
            }
            else if (keyData == Keys.PageDown || keyData == (Keys.Left | Keys.Control))
            {
                SelectNextPrevious(false);
                return true;
            }
            else if (keyData == (Keys.PageUp | Keys.Control))
            {
                var comboBox = this.selectSequenceComboBox;
                comboBox.SelectedIndex = Math.Min(comboBox.Items.Count - 1, comboBox.SelectedIndex + 1);
                this.UpdateSelectedSequenceFromComboBox();

                return true;
            }
            else if (keyData == (Keys.PageDown | Keys.Control))
            {
                var comboBox = this.selectSequenceComboBox;
                comboBox.SelectedIndex = Math.Max(0, comboBox.SelectedIndex - 1);
                this.UpdateSelectedSequenceFromComboBox();

                return true;
            }

            return false;
        }
    }
}