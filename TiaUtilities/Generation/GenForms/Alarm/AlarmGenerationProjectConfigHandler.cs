using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.Alarm;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public class AlarmGenerationProjectConfigHandler(AlarmConfigControl configControl, AlarmConfiguration config)
    {
        public void Init()
        {
            {
                var comboBox = configControl.partitionTypeComboBox;
                comboBox.SelectedValue = config.PartitionType;
                comboBox.SelectionChangeCommitted += (sender, args) => config.PartitionType = (AlarmPartitionType)(comboBox.SelectedValue ?? default(AlarmPartitionType));
            }

            {
                var comboBox = configControl.groupingTypeComboBox;
                comboBox.SelectedValue = config.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => config.GroupingType = (AlarmGroupingType)(comboBox.SelectedValue ?? default(AlarmGroupingType));
            }

            {
                var button = configControl.fcConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init().ControlWidth(185);
                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                        .ControlText(config.FCBlockName)
                        .TextChanged(v => config.FCBlockName = v);

                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NUMBER")
                        .ControlText(config.FCBlockNumber)
                        .UIntChanged(v => config.FCBlockNumber = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = configControl.alarmGenerationConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 300 };

                    var mainGroup = configForm.Init().ControlWidth(150);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_START_NUM")
                         .ControlText(config.StartingAlarmNum)
                         .UIntChanged(v => config.StartingAlarmNum = v);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_FORMAT")
                         .ControlText(config.AlarmNumFormat)
                         .TextChanged(v => config.AlarmNumFormat = v);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_SKIP")
                         .ControlText(config.SkipNumberAfterGroup)
                         .UIntChanged(v => config.SkipNumberAfterGroup = v);
                    mainGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_COIL1_TYPE")
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(config.Coil1Type)
                        .SelectedValueChanged<AlarmCoilType>(type => config.Coil1Type = type);
                    mainGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_COIL2_TYPE")
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(config.Coil2Type)
                        .SelectedValueChanged<AlarmCoilType>(type => config.Coil2Type = type);

                    mainGroup.AddSeparator().Height(15);

                    var antiSlipGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    antiSlipGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP");
                    antiSlipGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP_AMOUNT")
                         .ControlText(config.AntiSlipNumber)
                         .UIntChanged(v => config.AntiSlipNumber = v);
                    antiSlipGroup.AddCheckBox().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP_GEN_EMPTY")
                        .ControlNoAdapt()
                        .Value(config.GenerateEmptyAlarmAntiSlip)
                        .CheckedChanged(v => config.GenerateEmptyAlarmAntiSlip = v);

                    mainGroup.AddSeparator().Height(15);

                    var emptyAlarmGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    emptyAlarmGroup.AddLabel().Label("Empty Alarms");
                    emptyAlarmGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_NUM")
                         .ControlText(config.EmptyAlarmAtEnd)
                         .UIntChanged(v => config.EmptyAlarmAtEnd = v);
                    emptyAlarmGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_ALARM_ADDRESS")
                         .ControlText(config.EmptyAlarmContactAddress)
                         .TextChanged(v => config.EmptyAlarmContactAddress = v);

                    var timerGroup = emptyAlarmGroup.AddGroup().ControlWidth(175);
                    timerGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER");
                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_ADDRESS")
                         .ControlText(config.EmptyAlarmTimerAddress)
                         .TextChanged(v => config.EmptyAlarmTimerAddress = v);
                    timerGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_TYPE")
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(config.EmptyAlarmTimerType)
                         .TextChanged(v => config.EmptyAlarmTimerType = v);
                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_VALUE")
                         .ControlText(config.EmptyAlarmTimerValue)
                         .TextChanged(v => config.EmptyAlarmTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = configControl.fieldDefaultValueConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_COIL")
                         .ControlText(config.DefaultCoil1Address)
                         .TextChanged(v => config.DefaultCoil1Address = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_SET_COIL")
                         .ControlText(config.DefaultCoil2Address)
                         .TextChanged(v => config.DefaultCoil2Address = v);

                    var timerGroup = mainGroup.AddGroup().ControlWidth(225).NoAdapt();

                    timerGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER");

                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_ADDRESS")
                         .ControlText(config.DefaultTimerAddress)
                         .TextChanged(v => config.DefaultTimerAddress = v);

                    timerGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_TYPE")
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(config.DefaultTimerType)
                         .TextChanged(v => config.DefaultTimerType = v);

                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_VALUE")
                         .ControlText(config.DefaultTimerValue)
                         .TextChanged(v => config.DefaultTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = configControl.fieldPrefixConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_ALARM_ADDRESS")
                         .ControlText(config.AlarmAddressPrefix)
                         .TextChanged(v => config.AlarmAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_COIL_ADDRESS")
                         .ControlText(config.CoilAddressPrefix)
                         .TextChanged(v => config.CoilAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_SET_COIL_ADDRESS")
                         .ControlText(config.SetCoilAddressPrefix)
                         .TextChanged(v => config.SetCoilAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_TIMER_ADDRESS")
                         .ControlText(config.TimerAddressPrefix)
                         .TextChanged(v => config.TimerAddressPrefix = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = configControl.segmentNameConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 500 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_SEGMENT_NAME_ONE_EACH")
                         .ControlText(config.OneEachSegmentName)
                         .TextChanged(v => config.OneEachSegmentName = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_SEGMENT_NAME_ONE_EACH_EMPTY")
                         .ControlText(config.OneEachEmptyAlarmSegmentName)
                         .TextChanged(v => config.OneEachEmptyAlarmSegmentName = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH")
                         .ControlText(config.GroupSegmentName)
                         .TextChanged(v => config.GroupSegmentName = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH_EMPTY")
                         .ControlText(config.GroupEmptyAlarmSegmentName)
                         .TextChanged(v => config.GroupEmptyAlarmSegmentName = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = configControl.textListConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(Localization.Get("ALARM_CONFIG_TEXT_LIST"));

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_TEXT_LIST_FULL")
                         .ControlText(config.AlarmTextInList)
                         .TextChanged(v => config.AlarmTextInList = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_TEXT_LIST_EMPTY")
                         .ControlText(config.EmptyAlarmTextInList)
                         .TextChanged(v => config.EmptyAlarmTextInList = v);

                    SetupConfigForm(button, configForm);
                };
            }
        }

        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
