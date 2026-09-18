using System.Data;
using TiaUtilities.Configuration;
using TiaUtilities.Generation;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using static TiaUtilities.SettingsStep.SettingsLineControls;

namespace TiaUtilities.SettingsNew
{
    public partial class SettingsStepForm : Form
    {
        private class ComboBoxSourceItem
        {
            public required string Text { get; init; }
            public required SettingsStepSequence Sequence { get; init; }

            public override string ToString()
            {
                return this.Text;
            }
        }


        private const int STEP_PANEL_ROW = 2;
        private const int CONTROLS_PANEL_ROW = 3;

        private const string ARROW_CHAR = "↦";
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

        internal Dictionary<Type, List<SettingsStepSequence>> ConfigurationTypeModelDict { get; init; }

        private readonly ObservableObject<SettingsStepSequence?> selectedSequence;
        private readonly ObservableObject<SettingsStepPanelControls?> selectedPanelControls;
        private readonly ObservableObject<bool> searchMode;

        //private readonly TableLayoutPanelNoScrollbarsColorizable controlsPanel;
        private readonly List<SettingsStepSequence> sequences;

        private readonly SettingsSearchPanelControls searchPanelControls;

        public SettingsStepForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            ControlUtils.SetDoubleBuffered(this.mainTable);
            ControlUtils.SetDoubleBuffered(this.stepFlowPanel);
            ControlUtils.SetDoubleBuffered(this.bottomPanel);

            this.sequences = [];
            this.ConfigurationTypeModelDict = [];

            this.selectedPanelControls = new(null);
            this.selectedSequence = new(null);
            this.searchMode = new(false);

            this.searchPanelControls = new(this.searchPanel, this.sequences);

            this.InitControls();

        }

        private void InitControls()
        {
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

            #region SELECTED_CONFIGURATION_AS_DEFAULT_BUTTON

            ImageList imageList = new()
            {
                Images = { ImageResources.EFFECT },
                ImageSize = new(16, 16),
            };

            Button selectedAsDefaultButton = new()
            {
                Anchor = AnchorStyles.None,
                ImageList = imageList,
                ImageIndex = 0,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                MinimumSize = new(24, 24),
                MaximumSize = new(24, 24),
                Text = "",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = Color.FromArgb(127, Color.LightSkyBlue),
                    MouseDownBackColor = Color.FromArgb(127, Color.LightGreen)
                },
                Padding = Padding.Empty,
                Margin = Padding.Empty,
            };

            var selectedAsDefaultTooltip = ControlUtils.CreateToolTip(quick: true);
            selectedAsDefaultButton.MouseHover += (sender, args) =>
            {
                var selectedSequence = this.selectedSequence.Value;
                if (selectedSequence != null)
                {
                    selectedAsDefaultTooltip.Show($"Save as default configuration ({selectedSequence.Name} => {selectedSequence.Configuration.GetType().Name})", selectedAsDefaultButton);
                }
            };

            selectedAsDefaultButton.Click += (sender, args) =>
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
            };

            this.selectedConfigurationPanel.Controls.Add(selectedAsDefaultButton);
            #endregion

            #region CONTROLS_PANEL
            this.controlsPanel.RowStyles.Clear();
            this.controlsPanel.ColumnStyles.Clear();

            this.controlsPanel.ColumnCount = 5;

            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Name Label
            this.controlsPanel.ColumnStyles.Add(new(SizeType.AutoSize)); //Value Control
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Absolute, 25f));
            this.controlsPanel.ColumnStyles.Add(new(SizeType.Percent, 50f)); //For centering the table in the panel
            #endregion

            #region SELECTED_CONFIGURATION
            this.selectedConfigurationNameLabel.Font = StyleManager.Fonts.BIG_BOLD;
            this.selectedConfigurationNameLabel.Text = "";

            this.selectedSequence.Changed += (sender, args) =>
            {
                var oldSequence = args.OldValue;
                var newSequence = args.NewValue;

                var oldPanelControls = this.selectedPanelControls.Value;

                this.stepFlowPanel.SuspendLayout();
                this.stepFlowPanel.Controls.Clear();

                this.selectedConfigurationNameLabel.Text = "No Selection";

                this.selectedPanelControls.Value = null;
                
                if (oldSequence != null)
                {
                    foreach (var panelControls in oldSequence.PanelControls)
                    {
                        panelControls.StepLabelClick = null;
                        panelControls.SectionLabel.BorderWidth = 0;
                    }
                }

                if (newSequence != null)
                {
                    this.selectedConfigurationNameLabel.Text = newSequence.FullName;

                    List<Control> sectionsControls = [];
                    foreach (var panelControls in newSequence.PanelControls)
                    {
                        panelControls.StepLabelClick = () => this.selectedPanelControls.Value = panelControls;
                        sectionsControls.Add(panelControls.SectionLabel);
                    }

                    var l = sectionsControls.SelectMany((x, index) =>
                        index < sectionsControls.Count - 1 ?
                        new[] { x, SettingsStepPanelControls.CreateSectionLabel(ARROW_CHAR, arrow: true) } :
                        new[] { x }
                    ).ToArray();
                    this.stepFlowPanel.Controls.AddRange(l);

                    if (oldPanelControls != null)
                    {
                        var activePanelControls = newSequence.PanelControls.FirstOrDefault(p => p.SectionLabel.Text.Contains(oldPanelControls.SectionLabel.Text, StringComparison.OrdinalIgnoreCase));
                        if (activePanelControls != null)
                        {
                            this.selectedPanelControls.Value = activePanelControls;
                        }
                    }
                }

                this.stepFlowPanel.ResumeLayout(performLayout: true);
            };
            #endregion

            #region SELECTED_PANEL_CONTROLS
            this.selectedPanelControls.Changed += (sender, args) =>
            {
                var oldPanelControls = args.OldValue;
                var newPanelControls = args.NewValue;

                this.controlsPanel.SuspendLayout();

                this.controlsPanel.RowStyles.Clear();
                this.controlsPanel.Controls.Clear();
                this.controlsPanel.ClearCellStyles();

                if (oldPanelControls != null)
                {
                    oldPanelControls.SectionLabel.BorderWidth = 0;
                }

                if (newPanelControls != null)
                {
                    Utility.Validate.IsTrue(newPanelControls.Sequence == this.selectedSequence.Value);

                    newPanelControls.SectionLabel.BorderWidth = 1;
                    newPanelControls.ApplyControls(this.controlsPanel);
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
            this.searchTextBox.TextChanged += (sender, args) =>
            {
                this.searchPanelControls.UpdateSearchText(this.searchTextBox.Text);
            };
            #endregion

            #region SEARCH_MODE

            void UpdateVisibilityForSearchMode(bool searchModeActive)
            {
                if (searchModeActive)
                {
                    this.selectConfigurationComboBox.Visible = false;
                    this.selectConfigurationLabel.Visible = false;
                    this.stepFlowPanel.Visible = false;
                    this.controlsPanel.Visible = false;
                    this.controlsPanel.Controls.Clear();

                    this.searchLabel.Visible = true;
                    this.searchTextBox.Visible = true;
                    this.searchPanel.Visible = true;
                    this.searchPanelControls.InitPanel();
                }
                else
                {
                    this.selectConfigurationComboBox.Visible = true;
                    this.selectConfigurationLabel.Visible = true;
                    this.stepFlowPanel.Visible = true;
                    this.controlsPanel.Visible = true;

                    this.searchLabel.Visible = false;
                    this.searchTextBox.Visible = false;
                    this.searchPanel.Visible = false;
                    this.searchPanelControls.Clear();
                }
            }

            UpdateVisibilityForSearchMode(this.searchMode.Value);
            this.searchMode.Changed += (sender, args) => UpdateVisibilityForSearchMode(args.NewValue);
            #endregion
        }

        public void SetSequences(IEnumerable<SettingsStepSequence> sequences)
        {
            this.sequences.Clear();
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

        private static bool CalculateMatchCustom(string testoOggetto, string testoCercato)
        {
            // 1. Logica Multi-Parola: "mil acme" trova "Acme Corporation (Milano)"
            string[] paroleCercate = testoCercato.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool matchTutteLeParole = paroleCercate.All(parola => testoOggetto.Contains(parola, StringComparison.OrdinalIgnoreCase));

            if (matchTutteLeParole)
            {
                return true;
            }

            // 2. Logica Acronimo: "ACM" trova "Azienda Costruzioni Molisana"
            string acronimo = new(
                testoOggetto
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 0 && char.IsLetterOrDigit(w[0]))
                    .Select(w => w[0])
                    .ToArray()
            );

            return acronimo.StartsWith(testoCercato, StringComparison.OrdinalIgnoreCase);
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
            if(next)
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
            int step = -1;
            if (keyData >= (Keys.D1 | Keys.Control) && keyData <= (Keys.D9 | Keys.Control))
            {
                step = keyData - (Keys.D1 | Keys.Control);

            }
            else if (keyData == Keys.PageUp || keyData == (Keys.Right | Keys.Control))
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
         private void AddTestMappings()
        {
            AlarmMainConfiguration mainCfg = new()
            {
                AlarmCommentTemplate = "COMMENT!",
                FCBlockNumber = 123,
            };

            AlarmTabConfiguration tabCfg = new();


            var step1Context = new SettingsStepDescriptor("STEP1", "Descrizione dello step1")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC).Add(x => x.FCBlockName, Locale.GENERICS_NAME)
                                                     .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                                                     .Add(x => x.EnableCustomVariable, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR_DESCR)
                                                     .Add(x => x.EnableTimer, Locale.ALARM_SETTINGS_ENABLE_TIMER, Locale.ALARM_SETTINGS_ENABLE_TIMER_DESCR)
                .StartGroup("HMI").Add(x => x.HmiNameTemplate, Locale.ALARM_SETTINGS_HMI_NAME, options: new() { PlaceholdersCallback = (str) => str })
                                  .Add(x => x.HmiTextTemplate, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT_DESCR)
                                  .Add(x => x.HmiTriggerTagTemplate, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG_DESCR)
                                  .AddDivider()
                                  .AddText("TITLE!!", "Description??", new() { TextAlign = ContentAlignment.MiddleLeft })
                                  .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .End();

            var step2Context = new SettingsStepDescriptor("STEP2", "Descrizione dello step1")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC).Add(x => x.FCBlockName, Locale.GENERICS_NAME)
                                                     .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .End();

            var mainContexts = AlarmGenUtils.CreateGlobalSettingsStepDescriptors();
            var tabContexts = AlarmGenUtils.CreateTabSettingsStepDescriptors();

            SettingsStep.SettingsStepSequence container = new(mainCfg, "GLOBAL", "Configuration");
            container.AddRange(mainContexts);

            SettingsStep.SettingsStepSequence container2 = new(tabCfg, "TAB", "Hot tab 1");
            container2.AddRange(tabContexts);

            this.SetSequences([container, container2]);
        }

 
 */