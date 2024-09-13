using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenModules.Alarm.Tab
{
    public partial class AlarmGenTabControl : UserControl
    {
        private readonly DataGridView dataGridViewLeft;
        private readonly DataGridView dataGridViewRight;

        public AlarmGenTabControl(DataGridView dataGridViewLeft, DataGridView dataGridViewRight)
        {
            this.dataGridViewLeft = dataGridViewLeft;
            this.dataGridViewRight = dataGridViewRight;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.gridSplitContainer.Panel1.Controls.Add(this.dataGridViewLeft);
            this.gridSplitContainer.Panel2.Controls.Add(this.dataGridViewRight);

            #region PartitionType ComboBox
            this.partitionTypeComboBox.DisplayMember = "Text";
            this.partitionTypeComboBox.ValueMember = "Value";

            var partitionTypeItems = new List<object>();
            foreach (AlarmPartitionType partitionType in Enum.GetValues(typeof(AlarmPartitionType)))
            {
                partitionTypeItems.Add(new { Text = partitionType.GetTranslation(), Value = partitionType });
            }
            this.partitionTypeComboBox.DataSource = partitionTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion
        }

        public void BindConfig(AlarmTabConfiguration tabConfig)
        {
            {
                var comboBox = this.partitionTypeComboBox;
                comboBox.SelectedValue = tabConfig.PartitionType;
                comboBox.SelectionChangeCommitted += (sender, args) => tabConfig.PartitionType = (AlarmPartitionType)(comboBox.SelectedValue ?? default(AlarmPartitionType));
            }

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

                    var mainGroup = configForm.Init().ControlWidth(150);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_START_NUM)
                         .ControlText(tabConfig.StartingAlarmNum)
                         .UIntChanged(v => tabConfig.StartingAlarmNum = v);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_FORMAT)
                         .ControlText(tabConfig.AlarmNumFormat)
                         .TextChanged(v => tabConfig.AlarmNumFormat = v);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_SKIP)
                         .ControlText(tabConfig.SkipNumberAfterGroup)
                         .UIntChanged(v => tabConfig.SkipNumberAfterGroup = v);

                    mainGroup.AddSeparator().Height(15);

                    var antiSlipGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    antiSlipGroup.AddLabel().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP);
                    antiSlipGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_AMOUNT)
                         .ControlText(tabConfig.AntiSlipNumber)
                         .UIntChanged(v => tabConfig.AntiSlipNumber = v);
                    antiSlipGroup.AddCheckBox().Label(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_GEN_EMPTY)
                        .ControlNoAdapt()
                        .Value(tabConfig.GenerateEmptyAlarmAntiSlip)
                        .CheckedChanged(v => tabConfig.GenerateEmptyAlarmAntiSlip = v);

                    mainGroup.AddSeparator().Height(15);

                    var emptyAlarmGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    emptyAlarmGroup.AddLabel().Label("Empty Alarms");
                    emptyAlarmGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_NUM)
                         .ControlText(tabConfig.EmptyAlarmAtEnd)
                         .UIntChanged(v => tabConfig.EmptyAlarmAtEnd = v);
                    emptyAlarmGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_ALARM_ADDRESS)
                         .ControlText(tabConfig.EmptyAlarmContactAddress)
                         .TextChanged(v => tabConfig.EmptyAlarmContactAddress = v);

                    var timerGroup = emptyAlarmGroup.AddGroup().ControlWidth(175);
                    timerGroup.AddLabel().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER);
                    timerGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_ADDRESS)
                         .ControlText(tabConfig.EmptyAlarmTimerAddress)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerAddress = v);
                    timerGroup.AddComboBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_TYPE)
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(tabConfig.EmptyAlarmTimerType)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerType = v);
                    timerGroup.AddTextBox().Label(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_VALUE)
                         .ControlText(tabConfig.EmptyAlarmTimerValue)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.defaultValuesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();

                    var coil1Group = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    coil1Group.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_COIL1);

                    coil1Group.AddTextBox().Label(Locale.GENERICS_ADDRESS)
                         .ControlText(tabConfig.DefaultCoil1Address)
                         .TextChanged(v => tabConfig.DefaultCoil1Address = v);

                    coil1Group.AddComboBox().Label(Locale.GENERICS_TYPE)
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(tabConfig.DefaultCoil1Type)
                        .SelectedValueChanged<AlarmCoilType>(type => tabConfig.DefaultCoil1Type = type);


                    var coil2Group = mainGroup.AddGroup().ControlWidth(225).NoAdapt();
                    coil2Group.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_COIL2);

                    coil2Group.AddTextBox().Label(Locale.GENERICS_ADDRESS)
                         .ControlText(tabConfig.DefaultCoil2Address)
                         .TextChanged(v => tabConfig.DefaultCoil2Address = v);

                    coil2Group.AddComboBox().Label(Locale.GENERICS_TYPE)
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(tabConfig.DefaultCoil2Type)
                        .SelectedValueChanged<AlarmCoilType>(type => tabConfig.DefaultCoil2Type = type);


                    var timerGroup = mainGroup.AddGroup().ControlWidth(225).NoAdapt();

                    timerGroup.AddLabel().Label(Locale.ALARM_CONFIG_DEFAULTS_TIMER);

                    timerGroup.AddTextBox().Label(Locale.GENERICS_ADDRESS)
                         .ControlText(tabConfig.DefaultTimerAddress)
                         .TextChanged(v => tabConfig.DefaultTimerAddress = v);

                    timerGroup.AddComboBox().Label(Locale.GENERICS_TYPE)
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(tabConfig.DefaultTimerType)
                         .TextChanged(v => tabConfig.DefaultTimerType = v);

                    timerGroup.AddTextBox().Label(Locale.GENERICS_VALUE)
                         .ControlText(tabConfig.DefaultTimerValue)
                         .TextChanged(v => tabConfig.DefaultTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.valuesPrefixesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_ALARM)
                         .ControlText(tabConfig.AlarmAddressPrefix)
                         .TextChanged(v => tabConfig.AlarmAddressPrefix = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_COIL1)
                         .ControlText(tabConfig.Coil1AddressPrefix)
                         .TextChanged(v => tabConfig.Coil1AddressPrefix = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_COIL2)
                         .ControlText(tabConfig.Coil2AddressPrefix)
                         .TextChanged(v => tabConfig.Coil2AddressPrefix = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_PREFIX_TIMER)
                         .ControlText(tabConfig.TimerAddressPrefix)
                         .TextChanged(v => tabConfig.TimerAddressPrefix = v);

                    SetupConfigForm(button, configForm);
                };
            }
        }

        public void Translate()
        {
            this.generationConfigButton.Text = Locale.ALARM_CONFIG_GENERATION;
            this.defaultValuesConfigButton.Text = Locale.ALARM_CONFIG_DEFAULTS;
            this.valuesPrefixesConfigButton.Text = Locale.ALARM_CONFIG_PREFIX;
        }

        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
