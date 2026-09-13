using System.Data;
using System.Diagnostics;
using TiaUtilities.Configuration;
using TiaUtilities.Generation;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Languages;
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
            public required SettingsStepContainer Container { get; init; }

            public override string ToString()
            {
                return this.Text;
            }
        }

        private const int MAIN_PANEL_ROW = 2;

        private const string ARROW_CHAR = "↦";
        private const string PREVIOUS_ARROW = "⇦";
        private const string NEXT_ARROW = "⇨";


        internal Dictionary<Type, List<SettingsStepContainer>> ConfigurationTypeContainerDict { get; init; }

        private readonly List<SettingsStepContainer> containers;

        private readonly ObservableObject<SettingsStepContainer?> selectedContainer;
        private readonly ObservableObject<int> selectedStep;

        private int currentStepCount = 0;

        public SettingsStepForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            ControlUtils.SetDoubleBuffered(this.mainTable);
            ControlUtils.SetDoubleBuffered(this.stepFlowPanel);
            ControlUtils.SetDoubleBuffered(this.bottomPanel);

            this.containers = [];
            this.ConfigurationTypeContainerDict = [];

            this.selectedStep = new(-1);
            this.selectedContainer = new(null);

            this.InitControls();

            //this.AddTestMappings();
        }

        private void InitControls()
        {
            this.selectedContainer.Changed += (sender, args) =>
            {
                var oldContainer = args.OldValue;
                var newContainer = args.NewValue;

                this.stepFlowPanel.SuspendLayout();

                var oldStep = this.selectedStep.Value;

                this.selectedStep.Value = -1;
                this.currentStepCount = 0;

                this.stepFlowPanel.Controls.Clear();

                if (oldContainer != null)
                {
                    var mappings = oldContainer.GetMappings(this);
                    foreach (var mapping in mappings)
                    {
                        mapping.StepLabel.BorderWidth = 0;
                        mapping.StepLabel.BorderColor = Color.Transparent;
                    }
                }

                if (newContainer != null)
                {
                    var mappings = newContainer.GetMappings(this);
                    this.currentStepCount = mappings.Count;

                    List<Label> stepLabels = [];
                    for (int x = 0; x < mappings.Count; x++)
                    {
                        var mapping = mappings[x];

                        if (!mapping.ListenersRegistered)
                        {
                            mapping.RegisterListeners();

                            var step = x;
                            mapping.StepLabel.Click += (sender, args) => this.selectedStep.Value = step;
                        }

                        stepLabels.Add(mapping.StepLabel);
                    }

                    var l = stepLabels.SelectMany((x, index) =>
                        index < stepLabels.Count - 1 ?
                        new[] { x, SettingsStepMapper.CreateStepLabel(ARROW_CHAR, arrow: true) } :
                        new[] { x }
                    ).ToArray();
                    this.stepFlowPanel.Controls.AddRange(l);

                    if (mappings.InRange(oldStep))
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

                this.mainTable.SuspendLayout();

                var centralControl = this.mainTable.GetControlFromPosition(0, MAIN_PANEL_ROW);
                if (centralControl != null)
                {
                    this.mainTable.Controls.Remove(centralControl);
                }

                var container = this.selectedContainer.Value;
                if (container != null)
                {
                    var mappings = container.GetMappings(this);

                    if (mappings.TryGet(oldValue, out var oldMapping))
                    {
                        //oldMapping.StepLabel.Margin = new(2);
                        oldMapping.StepLabel.BorderWidth = 0;
                        oldMapping.StepLabel.BorderColor = Color.Transparent;

                        this.mainTable.Controls.Remove(oldMapping.Control);
                    }

                    if (mappings.TryGet(newValue, out var newMapping))
                    {
                        //newMapping.StepLabel.Margin = Padding.Empty;
                        newMapping.StepLabel.BorderWidth = 2;
                        newMapping.StepLabel.BorderColor = Color.FromArgb(127, Color.Black);

                        this.mainTable.Controls.Add(newMapping.Control, 0, MAIN_PANEL_ROW);
                    }
                    else
                    {
                        this.selectedStep.Value = -1;
                    }
                }

                this.mainTable.ResumeLayout(performLayout: true);
            };

            this.buttonNext.Click += (sender, args) =>
            {
                var step = this.selectedStep.Value;
                this.selectedStep.Value = Math.Min(step + 1, this.currentStepCount - 1);
            };

            this.buttonPrevious.Click += (sender, args) =>
            {
                var step = this.selectedStep.Value;
                this.selectedStep.Value = Math.Max(step - 1, 0);
            };

            this.selectContainerComboBox.BackColor = Form.DefaultBackColor;
            this.selectContainerComboBox.Font = StyleManager.Fonts.NORMAL_SEMIBOLD;
            this.selectContainerComboBox.DropDownClosed += (sender, args) =>
            {//Remove focus on ComboBox after closing drop down to avoid having it selected (Annoying).
                this.BeginInvoke(() => this.ActiveControl = null);
            };
            this.selectContainerComboBox.DisplayMember = nameof(ComboBoxSourceItem.Text);
            this.selectContainerComboBox.ValueMember = nameof(ComboBoxSourceItem.Container);
            this.selectContainerComboBox.FilterPredicate = (item, text) =>
            {
                var i = (ComboBoxSourceItem)item;
                return CalcolaMatchCustom(i.Text, text);
            };
            this.selectContainerComboBox.SelectedValueChanged += (sender, args) =>
            {
                if (this.selectContainerComboBox.SelectedItem is ComboBoxSourceItem item)
                {
                    this.selectedContainer.Value = item.Container;
                }
            };

            ContextMenuStrip contextMenu = new();

            ToolStripLabel actualConfigurationNameLabel = new() { AutoSize = true };

            ToolStripMenuItem saveToPresetItem = new() { Image = ImageResources.EFFECT };
            saveToPresetItem.Click += (sender, args) =>
            {
                var selectedItem = this.selectContainerComboBox.SelectedItem;
                if (selectedItem is ComboBoxSourceItem sourceItem)
                {
                    var configuration = sourceItem.Container.Configuration;

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
                    if (this.selectContainerComboBox.SelectedItem is ComboBoxSourceItem item)
                    {
                        saveToPresetItem.Text = $"Save as default configuration ({item.Container.Configuration.GetType().Name})";

                        actualConfigurationNameLabel.Text = item.Text;
                        contextMenu.Refresh();
                        contextMenu.PerformLayout();
                    }
                });
            };


            contextMenu.Items.AddRange([actualConfigurationNameLabel, new ToolStripSeparator(), saveToPresetItem]);
            this.selectContainerComboBox.ContextMenuStrip = contextMenu;
        }

        public void SetContainers(IEnumerable<SettingsStepContainer> containers)
        {
            this.containers.Clear();
            this.ConfigurationTypeContainerDict.Clear();
            if (!containers.Any())
            {
                this.selectedContainer.Value = null;
                return;
            }

            foreach (var container in containers)
            {
                var cfgType = container.Configuration.GetType();

                var tryGetOK = this.ConfigurationTypeContainerDict.TryGetValue(cfgType, out var typeContainers);
                if (!tryGetOK)
                {
                    typeContainers = [];
                    this.ConfigurationTypeContainerDict.Add(cfgType, typeContainers);
                }

                typeContainers?.Add(container);
            }

            var items = containers.Select(c => new ComboBoxSourceItem() { Text = $"{c.GroupName} - {c.Name}", Container = c });

            var maxWidth = items.Max(i => TextRenderer.MeasureText(i.Text, this.selectContainerComboBox.Font, Size.Empty, TextFormatFlags.TextBoxControl).Width);
            this.selectContainerComboBox.Width = maxWidth + (int)(maxWidth * 0.15);
            this.selectContainerComboBox.SetFilterableSource(items);

            this.containers.AddRange(containers);
            this.selectedContainer.Value = containers.First();
        }

        /// <summary>
        /// Logica di Matching Personalizzata
        /// </summary>
        private static bool CalcolaMatchCustom(string testoOggetto, string testoCercato)
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

        private void AddTestMappings()
        {
            AlarmMainConfiguration mainCfg = new()
            {
                AlarmCommentTemplate = "COMMENT!",
                FCBlockNumber = 123,
            };

            AlarmTabConfiguration tabCfg = new();


            var step1Context = new SettingsStepContext("STEP1", "Descrizione dello step1")
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

            var step2Context = new SettingsStepContext("STEP2", "Descrizione dello step1")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC).Add(x => x.FCBlockName, Locale.GENERICS_NAME)
                                                     .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .End();

            var mainContexts = AlarmGenUtils.CreateSettingsGlobalContext();
            var tabContexts = AlarmGenUtils.CreateSettingsTabContext();

            SettingsStepContainer container = new(mainCfg, "GLOBAL", "Configuration");
            container.AddRange(mainContexts);

            SettingsStepContainer container2 = new(tabCfg, "TAB", "Hot tab 1");
            container2.AddRange(tabContexts);

            this.SetContainers([container, container2]);
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
