using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Languages;
using TiaXmlReader;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenModules.Alarm.Tab
{
    public partial class AlarmGenTabControl : UserControl
    {
        private readonly AlarmGenTemplateHandler templateHandler;
        private readonly DataGridView dataGridView;

        public AlarmGenTabControl(AlarmGenTemplateHandler templateHandler, DataGridView dataGridView)
        {
            this.templateHandler = templateHandler;
            this.dataGridView = dataGridView;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.mainTableLayout.Controls.Add(this.dataGridView);

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            //Seems every time a tab is shown, these have all the text selected. Not sure why. This fixes it.
            this.groupingTypeComboBox.VisibleChanged += (sender, args) => this.groupingTypeComboBox.SelectionLength = 0;
            #endregion
        }

        public void BindConfig(GridBindContainer bindContainer, AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig, IEnumerable<AlarmTabConfiguration> tabConfigs)
        {
            {
                var comboBox = this.groupingTypeComboBox;
                comboBox.SelectedValue = tabConfig.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => tabConfig.GroupingType = (AlarmGroupingType)(comboBox.SelectedValue ?? default(AlarmGroupingType));
            }

            {
                var button = this.generationConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 300 };
                    configForm.SetConfiguration(tabConfig, MainForm.Settings.PresetAlarmTabConfiguration, tabConfigs);

                    var mainGroup = configForm.Init().ControlWidth(150);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_TOTAL_NUM).BindUInt(() => tabConfig.TotalAlarmNum);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_START_NUM).BindUInt(() => tabConfig.StartingAlarmNum);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_SKIP).BindUInt(() => tabConfig.SkipNumberAfterGroup);

                    mainGroup.AddSeparator().Height(15);

                    var hmiGroup = mainGroup.AddGroup().ControlWidth(150);
                    hmiGroup.AddLabel().Label(Locale.GENERICS_HMI);
                    hmiGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_HMI_START_ID).BindUInt(() => tabConfig.HmiStartID);
                    hmiGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_HMI_DEFAULT_ALARM_CLASS).BindText(() => tabConfig.DefaultHmiAlarmClass);

                    mainGroup.AddSeparator().Height(15);

                    var antiSlipGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    antiSlipGroup.AddLabel().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP);
                    antiSlipGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_AMOUNT) .BindUInt(() => tabConfig.AntiSlipNumber);
                    antiSlipGroup.AddCheckBox().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_GEN_EMPTY).BindChecked(() => tabConfig.GenerateEmptyAlarmAntiSlip).ControlNoAdapt();

                    mainGroup.AddSeparator().Height(15);

                    var emptyAlarmGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    emptyAlarmGroup.AddLabel().Label("Empty Alarms");
                    emptyAlarmGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_NUM).BindUInt(() => tabConfig.EmptyAlarmAtEnd);
                    emptyAlarmGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_ALARM_ADDRESS).BindText(() => tabConfig.EmptyAlarmContactAddress);

                    var timerGroup = emptyAlarmGroup.AddGroup().ControlWidth(175);
                    timerGroup.AddLabel().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER);
                    timerGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_ADDRESS).BindText(() => tabConfig.EmptyAlarmTimerAddress);
                    timerGroup.AddComboBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_TYPE).BindText(() => tabConfig.EmptyAlarmTimerType)
                         .Items(["TON", "TOF"]).DisableEdit();
                    timerGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_VALUE).BindText(() => tabConfig.EmptyAlarmTimerValue);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.defaultValuesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);
                    configForm.SetConfiguration(tabConfig, MainForm.Settings.PresetAlarmTabConfiguration, tabConfigs);

                    var mainGroup = configForm.Init();
                    
                    var customVarGroup = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    customVarGroup.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_CUSTOM_VAR);
                    customVarGroup.AddTextBox().Label(Locale.GENERICS_ADDRESS).BindText(() => tabConfig.DefaultCustomVarAddress);
                    customVarGroup.AddTextBox().Label(Locale.GENERICS_VALUE).BindText(() => tabConfig.DefaultCustomVarValue);
                    
                    var coil1Group = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    coil1Group.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_COIL1);
                    coil1Group.AddTextBox().Label(Locale.GENERICS_ADDRESS).BindText(() => tabConfig.DefaultCoil1Address);
                    coil1Group.AddComboBox().Label(Locale.GENERICS_TYPE)
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .BindValue(() => tabConfig.DefaultCoil1Type); //After the Items func above

                    var coil2Group = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    coil2Group.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_COIL2);
                    coil2Group.AddTextBox().Label(Locale.GENERICS_ADDRESS).BindText(() => tabConfig.DefaultCoil2Address);
                    coil2Group.AddComboBox().Label(Locale.GENERICS_TYPE)
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .BindValue(() => tabConfig.DefaultCoil2Type); //After the Items func above

                    var timerGroup = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    timerGroup.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_TIMER);
                    timerGroup.AddTextBox().Label(Locale.GENERICS_ADDRESS).BindText(() => tabConfig.DefaultTimerAddress);
                    timerGroup.AddComboBox().Label(Locale.GENERICS_TYPE).BindText(() => tabConfig.DefaultTimerType).Items(["TON", "TOF"]).DisableEdit();
                    timerGroup.AddTextBox().Label(Locale.GENERICS_VALUE).BindText(() => tabConfig.DefaultTimerValue);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.valuesPrefixesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);
                    configForm.SetConfiguration(tabConfig, MainForm.Settings.PresetAlarmTabConfiguration, tabConfigs);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_ALARM).BindText(() => tabConfig.AlarmAddressPrefix);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_COIL1).BindText(() => tabConfig.Coil1AddressPrefix);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_COIL2).BindText(() => tabConfig.Coil2AddressPrefix);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_TIMER).BindText(() => tabConfig.TimerAddressPrefix);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.customPlaceholdersButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text)
                    {
                        CloseOnEnter = false, //Avoid closing when entering new line in the text box
                    };
                    configForm.SetConfiguration(tabConfig, MainForm.Settings.PresetAlarmTabConfiguration, tabConfigs);

                    var mainGroup = configForm.Init().ControlWidth(600);
                    mainGroup.AddLabel().Label("Custom Placeholders JSON object");
                    mainGroup.AddJSON().Height(500).BindText(() => tabConfig.CustomPlaceholdersJSON).RegisterLinter(configForm);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.editTemplateConfigButton;
                button.Click += (sender, args) =>
                {
                    var deviceTemplateForm = new AlarmGenTemplateForm(mainConfig, tabConfig, bindContainer, templateHandler);
                    deviceTemplateForm.Init();
                    deviceTemplateForm.ShowDialog(this);
                };
            }
        }

        public void Translate()
        {
            this.generationConfigButton.Text = Locale.ALARM_CONFIG_GENERATION;
            this.defaultValuesConfigButton.Text = Locale.ALARM_CONFIG_DEFAULTS;
            this.valuesPrefixesConfigButton.Text = Locale.ALARM_CONFIG_PREFIX;
            this.editTemplateConfigButton.Text = Locale.ALARM_CONFIG_EDIT_TEMPLATE;
        }

        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
