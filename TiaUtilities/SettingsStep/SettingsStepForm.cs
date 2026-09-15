using System.Data;
using System.Diagnostics;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew
{
    public partial class SettingsStepForm : Form
    {
        private class ComboBoxSourceItem
        {
            public required string Text { get; init; }
            public required SettingsStepSequence Model { get; init; }

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


        internal Dictionary<Type, List<SettingsStepSequence>> ConfigurationTypeModelDict { get; init; }

        private readonly ObservableObject<SettingsStepSequence?> selectedConfigurationData;
        private readonly ObservableObject<int> selectedStep;

        private readonly TableLayoutPanelNoScrollbarsColorizable controlsPanel;
        private readonly List<SettingsStepSequence> sequences;

        private int currentStepCount = 0;

        public SettingsStepForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            ControlUtils.SetDoubleBuffered(this.mainTable);
            ControlUtils.SetDoubleBuffered(this.stepFlowPanel);

            this.sequences = [];
            this.ConfigurationTypeModelDict = [];
            this.selectedStep = new(-1);
            this.selectedConfigurationData = new(null);

            this.controlsPanel = this.InitControlsPanel();
            this.InitControls();

        }

        private TableLayoutPanelNoScrollbarsColorizable InitControlsPanel()
        {
            TableLayoutPanelNoScrollbarsColorizable controlsPanel = new()
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
                //Padding = new(10),
                Margin = new(10),
            };

            Panel middlePanel = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Controls = { controlsPanel },
                Padding = Padding.Empty,
                Margin = new(5),
            };
            ControlUtils.SetDoubleBuffered(middlePanel);

            this.mainTable.Controls.Add(middlePanel, 0, CONTROLS_PANEL_ROW);
            return controlsPanel;

            /*
                ScrollableControl scrollableControl = new()
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    AutoScrollMinSize = new(0, 0),
                    AutoScrollMargin = new(0, 0),
                    Controls = { middlePanel },
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };
                ControlUtils.SetDoubleBuffered(scrollableControl);
            */
        }

        private void InitControls()
        {
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

            this.selectedConfigurationNameLabel.Font = StyleManager.Fonts.BIG_BOLD;
            this.selectedConfigurationNameLabel.Text = "";

            this.selectedConfigurationData.Changed += (sender, args) =>
            {
                var oldContainer = args.OldValue;
                var newContainer = args.NewValue;

                var oldStep = this.selectedStep.Value;

                this.stepFlowPanel.SuspendLayout();
                this.stepFlowPanel.Controls.Clear();

                this.selectedConfigurationNameLabel.Text = "No Selection";


                this.selectedStep.Value = -1;
                this.currentStepCount = 0;

                if (oldContainer != null)
                {
                    var panelControlsList = oldContainer.GetPanelControlsList(this);
                    foreach (var tableLayout in panelControlsList)
                    {
                        tableLayout.StepLabel.BorderWidth = 0;
                        tableLayout.StepLabel.BorderColor = Color.Transparent;
                    }
                }

                if (newContainer != null)
                {
                    this.selectedConfigurationNameLabel.Text = $"{newContainer.GroupName} - {newContainer.Name}";

                    var panelControlsList = newContainer.GetPanelControlsList(this);
                    this.currentStepCount = panelControlsList.Count;

                    List<Label> stepLabels = [];
                    for (int x = 0; x < panelControlsList.Count; x++)
                    {
                        var panelControls = panelControlsList[x];

                        if (!panelControls.ListenersRegistered)
                        {
                            panelControls.RegisterListeners();

                            var step = x;
                            panelControls.StepLabel.Click += (sender, args) => this.selectedStep.Value = step;
                        }

                        stepLabels.Add(panelControls.StepLabel);
                    }

                    var l = stepLabels.SelectMany((x, index) =>
                        index < stepLabels.Count - 1 ?
                        new[] { x, SettingsStepPanelControls.CreateStepLabel(ARROW_CHAR, arrow: true) } :
                        new[] { x }
                    ).ToArray();
                    this.stepFlowPanel.Controls.AddRange(l);

                    if (panelControlsList.InRange(oldStep))
                    {//Keep the same selected step if is in range of the new
                        this.selectedStep.Value = oldStep;
                    }
                }

                this.stepFlowPanel.ResumeLayout(performLayout: true);
            };

            this.selectedStep.Changed += (sender, args) =>
            {
                var oldValue = args.OldValue;
                var newValue = args.NewValue;

                this.controlsPanel.SuspendLayout();

                this.controlsPanel.RowStyles.Clear();
                this.controlsPanel.Controls.Clear();
                this.controlsPanel.ClearCellStyles();

                var container = this.selectedConfigurationData.Value;
                if (container != null)
                {
                    var panelControlsList = container.GetPanelControlsList(this);

                    if (panelControlsList.TryGet(oldValue, out var oldPanelControls))
                    {
                        oldPanelControls.StepLabel.BorderWidth = 0;
                        oldPanelControls.StepLabel.BorderColor = Color.Transparent;
                    }

                    if (panelControlsList.TryGet(newValue, out var newPanelControls))
                    {
                        newPanelControls.StepLabel.BorderWidth = 2;
                        newPanelControls.StepLabel.BorderColor = Color.FromArgb(127, Color.Black);

                        newPanelControls.ApplyControls(this.controlsPanel);
                    }
                    else
                    {
                        this.selectedStep.Value = -1;
                    }
                }

                this.controlsPanel.ResumeLayout(performLayout: true);
            };

            this.selectConfigurationComboBox.BackColor = Form.DefaultBackColor;
            this.selectConfigurationComboBox.Font = StyleManager.Fonts.NORMAL_SEMIBOLD;
            this.selectConfigurationComboBox.DropDownClosed += (sender, args) =>
            {//Remove focus on ComboBox after closing drop down to avoid having it selected (Annoying).
                this.BeginInvoke(() => this.ActiveControl = null);
            };
            this.selectConfigurationComboBox.DisplayMember = nameof(ComboBoxSourceItem.Text);
            this.selectConfigurationComboBox.ValueMember = nameof(ComboBoxSourceItem.Model);
            this.selectConfigurationComboBox.FilterPredicate = (item, text) =>
            {
                var i = (ComboBoxSourceItem)item;
                return CalculateMatchCustom(i.Text, text);
            };
            this.selectConfigurationComboBox.SelectedValueChanged += (sender, args) =>
            {
                Debug.WriteLine("SelectedValueChanged");
                if (this.selectConfigurationComboBox.SelectedItem is ComboBoxSourceItem item)
                {
                    this.selectedConfigurationData.Value = item.Model;
                }
            };

            ContextMenuStrip contextMenu = new();

            ToolStripLabel actualConfigurationNameLabel = new() { AutoSize = true };

            ToolStripMenuItem saveToPresetItem = new() { Image = ImageResources.EFFECT };
            saveToPresetItem.Click += (sender, args) =>
            {
                var selectedItem = this.selectConfigurationComboBox.SelectedItem;
                if (selectedItem is ComboBoxSourceItem sourceItem)
                {
                    var configuration = sourceItem.Model.Configuration;

                    var presetConfiguration = MainForm.Settings.GetPresetConfiguration(configuration.GetType());
                    if (presetConfiguration != null)
                    {
                        GenUtils.CopySamePublicFieldsAndProperties(configuration, presetConfiguration);
                    }
                }

            };

            contextMenu.VisibleChanged += (sender, args) =>
            {
                contextMenu.BeginInvoke(() =>
                {
                    if (this.selectConfigurationComboBox.SelectedItem is ComboBoxSourceItem item)
                    {
                        saveToPresetItem.Text = $"Save as default configuration ({item.Model.Configuration.GetType().Name})";

                        actualConfigurationNameLabel.Text = item.Text;
                        contextMenu.Refresh();
                        contextMenu.PerformLayout();
                    }
                });
            };


            contextMenu.Items.AddRange([actualConfigurationNameLabel, new ToolStripSeparator(), saveToPresetItem]);
            this.selectConfigurationComboBox.ContextMenuStrip = contextMenu;
        }

        public void SetSequences(IEnumerable<SettingsStepSequence> configurationDataEnumerable)
        {
            this.sequences.Clear();
            this.ConfigurationTypeModelDict.Clear();
            if (!configurationDataEnumerable.Any())
            {
                this.selectedConfigurationData.Value = null;
                return;
            }

            foreach (var model in configurationDataEnumerable)
            {
                var cfgType = model.Configuration.GetType();

                var tryGetOK = this.ConfigurationTypeModelDict.TryGetValue(cfgType, out var typeContainers);
                if (!tryGetOK)
                {
                    typeContainers = [];
                    this.ConfigurationTypeModelDict.Add(cfgType, typeContainers);
                }

                typeContainers?.Add(model);
            }

            var items = configurationDataEnumerable.Select(c => new ComboBoxSourceItem() { Text = $"{c.GroupName} - {c.Name}", Model = c });

            var maxWidth = items.Max(i => TextRenderer.MeasureText(i.Text, this.selectConfigurationComboBox.Font, Size.Empty, TextFormatFlags.TextBoxControl).Width);
            this.selectConfigurationComboBox.Width = maxWidth + (int)(maxWidth * 0.15);
            this.selectConfigurationComboBox.SetFilterableSource(items);

            this.sequences.AddRange(configurationDataEnumerable);
            this.selectedConfigurationData.Value = configurationDataEnumerable.First();
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int step = -1;
            if (keyData >= (Keys.D1 | Keys.Control) && keyData <= (Keys.D9 | Keys.Control))
            {
                step = keyData - (Keys.D1 | Keys.Control);

            }
            else if (keyData == Keys.PageUp || keyData == (Keys.Right | Keys.Control))
            {
                var currentStep = this.selectedStep.Value;
                step = currentStep >= this.currentStepCount - 1 ? 0 : currentStep + 1;
            }
            else if (keyData == Keys.PageDown || keyData == (Keys.Left | Keys.Control))
            {
                var currentStep = this.selectedStep.Value;
                step = currentStep <= 0 ? this.currentStepCount - 1 : currentStep - 1;
            }

            if (step >= 0 && step < this.currentStepCount)
            {
                if (this.selectedStep.Value != step)
                {
                    this.selectedStep.Value = step;
                }
                else
                {
                    this.selectedStep.Value = -1;
                }

                return true;
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