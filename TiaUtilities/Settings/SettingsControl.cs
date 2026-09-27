using System.Data;
using TiaUtilities.Configuration;
using TiaUtilities.Generation;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TiaUtilities.SettingsStep
{
    public partial class SettingsControl : UserControl
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

        internal Dictionary<Type, List<SettingsSequence>> ConfigurationTypeModelDict { get; init; }

        private readonly ObservableObject<SettingsSequence?> selectedSequence;
        private readonly ObservableObject<SettingsSequencePanel?> selectedPanelControls;
        private readonly ObservableObject<bool> searchMode;

        //private readonly TableLayoutPanelNoScrollbarsColorizable controlsPanel;
        private readonly List<SettingsSequence> sequences;

        private readonly SettingsSearchPanel searchPanel;
        private bool _initialized = false;

        public SettingsControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            ControlUtils.SetDoubleBuffered(this.mainTable);
            ControlUtils.SetDoubleBuffered(this.bottomPanel);
            ControlUtils.SetDoubleBuffered(this.sequenceButtonsPanel);

            this.sequences = [];
            this.ConfigurationTypeModelDict = [];

            this.selectedPanelControls = new(null);
            this.selectedSequence = new(null);
            this.searchMode = new(false);

            this.searchPanel = new(this.searchPanelControl, this.sequences);
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

            #region CONTROLS_PANEL
            this.controlsPanel.RowStyles.Clear();
            this.controlsPanel.ColumnStyles.Clear();

            this.controlsPanel.ColumnCount = 5;

            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Name Label
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Control
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Absolute, 25f)); //Button
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            #endregion

            #region SELECTED_CONFIGURATION
            this.selectedSequence.Changed += (sender, args) =>
            {
                var oldSequence = args.OldValue;
                var newSequence = args.NewValue;

                var oldPanelControls = this.selectedPanelControls.Value;

                this.sequenceButtonsPanel.SuspendLayout();
                this.sequenceButtonsPanel.Controls.Clear();

                this.selectedPanelControls.Value = null;

                if (oldSequence != null)
                {
                    foreach (var panelControls in oldSequence.PanelControls)
                    {
                        panelControls.LabelClick = null;
                        panelControls.Label.BorderWidth = 0;
                    }
                }

                if (newSequence != null)
                {
                    List<Control> sectionsControls = [];
                    foreach (var panelControls in newSequence.PanelControls)
                    {
                        panelControls.LabelClick = () => this.selectedPanelControls.Value = panelControls;
                        sectionsControls.Add(panelControls.Label);
                    }

                    var sequenceNameLabel = new LabelWithSymbols()
                    {
                        BackColor = Color.Transparent,

                        Text = newSequence.FullName,
                        Font = StyleManager.Fonts.BIG_SEMIBOLD,
                        TextAlign = ContentAlignment.MiddleCenter,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        BorderStyle = BorderStyle.None,

                        Padding = new(4, 4, 0, 4),
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
                                    return $"Save as default configuration ({selectedSequence?.Name} => {selectedSequence?.Configuration.GetType().Name})";
                                },
                                Color = Color.Gray,
                                HoverColor = Color.Green,
                                ClickedColor = Color.DarkGoldenrod,
                                ClickedColorDuration = 500,
                                OnClick = item => SaveSelectedSequenceConfigurationAsDefault()
                            }
                        }
                    };
                    var symbolLabel = SettingsSequencePanel.CreateSectionLabel(CHEVRON_DOUBLE_RIGHT, symbol: true);

                    this.sequenceButtonsPanel.Controls.AddRange([sequenceNameLabel/*, selectAsDefaultControl*/, symbolLabel]);

                    var l = sectionsControls.SelectMany((x, index) =>
                        index < sectionsControls.Count - 1 ?
                        new[] { x, SettingsSequencePanel.CreateSectionLabel(MIDDLE_DOT, symbol: true) } :
                        new[] { x }
                    ).ToArray();
                    this.sequenceButtonsPanel.Controls.AddRange(l);

                    if (oldPanelControls != null)
                    {
                        var activePanelControls = newSequence.PanelControls.FirstOrDefault(p => p.Label.Text.Contains(oldPanelControls.Label.Text, StringComparison.OrdinalIgnoreCase));
                        if (activePanelControls != null)
                        {
                            this.selectedPanelControls.Value = activePanelControls;
                        }
                    }
                }

                this.sequenceButtonsPanel.ResumeLayout(performLayout: true);
            };
            #endregion

            #region SELECTED_PANEL_CONTROLS
            this.selectedPanelControls.Changed += (sender, args) =>
            {
                var oldPanel = args.OldValue;
                var newPanel = args.NewValue;

                this.controlsPanel.SuspendLayout();

                this.controlsPanel.RowStyles.Clear();
                this.controlsPanel.Controls.Clear();
                this.controlsPanel.ClearCellStyles();

                if (oldPanel != null)
                {
                    oldPanel.Label.BorderWidth = 0;
                }

                if (newPanel != null)
                {
                    Utility.Validate.IsTrue(newPanel.Sequence == this.selectedSequence.Value);

                    newPanel.Label.BorderWidth = 1;
                    newPanel.ApplyControls(this.controlsPanel);
                }

                this.controlsPanel.ResumeLayout(performLayout: true);
            };
            #endregion

            #region SELECT_CONFIGURATION_COMBOBOX
            this.selectConfigurationComboBox.BackColor = Form.DefaultBackColor;
            this.selectConfigurationComboBox.Font = StyleManager.Fonts.NORMAL_SEMIBOLD;
            this.selectConfigurationComboBox.DropDownClosed += (sender, args) =>
            {//Remove focus on ComboBox after closing drop down to avoid having it selected (Annoying).
                this.BeginInvoke(() => this.ActiveControl = null);
            };
            this.selectConfigurationComboBox.DisplayMember = nameof(ComboBoxSourceItem.Text);
            this.selectConfigurationComboBox.ValueMember = nameof(ComboBoxSourceItem.Sequence);
            this.selectConfigurationComboBox.FilterPredicate = (item, text) =>
            {
                var i = (ComboBoxSourceItem)item;
                return CalculateMatchCustom(i.Text, text);
            };
            this.selectConfigurationComboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if (this.selectConfigurationComboBox.SelectedItem is ComboBoxSourceItem item)
                {
                    this.selectedSequence.Value = item.Sequence;
                }
            };
            #endregion

            #region SEARCH_CONTROLS
            this.searchTextBox.BackColor = Form.DefaultBackColor;
            this.searchTextBox.LostFocus += (sender, args) =>
            {
                this.searchPanel.UpdateSearchText(this.searchTextBox.Text);
            };

            this.searchTextBox.KeyDown += (sender, args) =>
            {
                if (args.KeyData == Keys.Enter || args.KeyData == Keys.Tab)
                {
                    this.searchPanel.UpdateSearchText(this.searchTextBox.Text);
                }
            };
            #endregion

            #region SEARCH_MODE

            void UpdateVisibilityForSearchMode(bool searchModeActive)
            {
                if (searchModeActive)
                {
                    this.selectConfigurationComboBox.Visible = false;
                    this.selectConfigurationLabel.Visible = false;
                    this.sequenceButtonsPanel.Visible = false;
                    this.controlsPanel.Visible = false;
                    this.controlsPanel.Controls.Clear();

                    this.searchLabel.Visible = true;
                    this.searchTextBox.Visible = true;
                    this.searchPanelControl.Visible = true;
                    this.searchPanel.InitPanel();
                }
                else
                {
                    this.selectConfigurationComboBox.Visible = true;
                    this.selectConfigurationLabel.Visible = true;
                    this.sequenceButtonsPanel.Visible = true;
                    this.controlsPanel.Visible = true;

                    this.searchLabel.Visible = false;
                    this.searchTextBox.Visible = false;
                    this.searchPanelControl.Visible = false;
                    this.searchPanel.Clear();
                }
            }

            UpdateVisibilityForSearchMode(this.searchMode.Value);
            this.searchMode.Changed += (sender, args) => UpdateVisibilityForSearchMode(args.NewValue);
            #endregion
        }

        public void SetSequences(IEnumerable<SettingsSequence> sequences)
        {
            this.searchMode.Value = false;

            this.sequences.ForEach(s => s.DisposeControls());
            this.sequences.Clear();

            this.searchPanel.DisposeControls();
            this.searchPanel.Clear();

            this.ConfigurationTypeModelDict.Clear();

            if (!sequences.Any())
            {
                this.selectedSequence.Value = null;
                return;
            }

            foreach (var sequence in sequences)
            {
                sequence.Init(this);

                var cfgType = sequence.Configuration.GetType();

                var tryGetOK = this.ConfigurationTypeModelDict.TryGetValue(cfgType, out var configurationSequences);
                if (!tryGetOK)
                {
                    configurationSequences = [];
                    this.ConfigurationTypeModelDict.Add(cfgType, configurationSequences);
                }

                configurationSequences?.Add(sequence);
            }

            var items = sequences.Select(s => new ComboBoxSourceItem() { Text = s.FullName, Sequence = s });

            var maxWidth = items.Max(i => TextRenderer.MeasureText(i.Text, this.selectConfigurationComboBox.Font, Size.Empty, TextFormatFlags.TextBoxControl).Width);
            this.selectConfigurationComboBox.Width = maxWidth + (int)(maxWidth * 0.15);
            this.selectConfigurationComboBox.SetFilterableSource(items);

            this.sequences.AddRange(sequences);
            this.selectedSequence.Value = sequences.First();
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
            var panelControls = this.selectedPanelControls.Value;
            if (sequence == null || panelControls == null)
            {
                return;
            }

            var count = sequence.PanelControls.Count;

            var indexOf = sequence.PanelControls.IndexOf(panelControls);

            int newIndex = 0;
            if (next)
            {
                newIndex = indexOf >= count - 1 ? 0 : indexOf + 1;
            }
            else
            {
                newIndex = indexOf <= 0 ? count - 1 : indexOf - 1;
            }
            this.selectedPanelControls.Value = sequence.PanelControls[newIndex];

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.PageUp || keyData == (Keys.Right | Keys.Control))
            {
                SelectNextPrevious(true);
            }
            else if (keyData == Keys.PageDown || keyData == (Keys.Left | Keys.Control))
            {
                SelectNextPrevious(false);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}


/*
private Control CreateSelectAsDefaultControl()
{

    ImageList imageList = new()
    {
        Images = { ImageResources.EFFECT },
        ImageSize = new(16, 16),
    };

    Button selectedAsDefaultButton = new()
    {
        Dock = DockStyle.Fill,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,

        //ImageList = imageList,
        //ImageIndex = 0,
        //ImageAlign = ContentAlignment.MiddleCenter,
        //TextImageRelation = TextImageRelation.Overlay,
        //MinimumSize = new(24, 24),
        //MaximumSize = new(24, 24),
        Text = "★",
        FlatStyle = FlatStyle.Flat,
        FlatAppearance = {
            BorderSize = 0,
            MouseOverBackColor = Color.FromArgb(127, Color.LightSkyBlue),
            MouseDownBackColor = Color.FromArgb(127, Color.LightGreen)
        },
        Padding = Padding.Empty,
        Margin = new(0, 0, 4, 0),
    };

    LabelColorizable control = new()
    {
        Dock = DockStyle.Fill,
        AutoSize = true,

        Text = STAR_FILLED,
        TextAlign = ContentAlignment.MiddleCenter,
        Font = StyleManager.Fonts.NORMAL_SEMIBOLD,

        BorderWidth = 0,

        BackColor = Color.Transparent,
        MouseDownBackColor = Color.Transparent,
        MouseHoverBackColor = Color.Transparent,

        MouseHoverForeColor = Color.FromArgb(127, Color.DarkGreen),
        MouseDownForeColor = Color.FromArgb(127, Color.DarkBlue),


        Margin = new(0, 0, 4, 0),
    };

    var tooltip = ControlUtils.CreateToolTip(quick: true);
    control.MouseHover += (sender, args) =>
    {
        var selectedSequence = this.selectedSequence.Value;
        if (selectedSequence != null)
        {
            var text = $"Save as default configuration ({selectedSequence.Name} => {selectedSequence.Configuration.GetType().Name})";
            tooltip.Show(text, control);
        }
    };

    control.Click += (sender, args) => SaveSelectedSequenceConfigurationAsDefault();
    return control;
}*/