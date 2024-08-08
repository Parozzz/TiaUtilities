using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTabConfigHandler(AlarmTabConfiguration tabConfig, DeviceAlarmTabControl tabControl)
    {
        public void Init()
        {
            {
                var comboBox = tabControl.partitionTypeComboBox;
                comboBox.SelectedValue = tabConfig.PartitionType;
                comboBox.SelectionChangeCommitted += (sender, args) => tabConfig.PartitionType = (AlarmPartitionType)(comboBox.SelectedValue ?? default(AlarmPartitionType));
            }

            {
                var comboBox = tabControl.groupingTypeComboBox;
                comboBox.SelectedValue = tabConfig.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => tabConfig.GroupingType = (AlarmGroupingType)(comboBox.SelectedValue ?? default(AlarmGroupingType));
            }

            {
                var button = tabControl.generationConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 300 };

                    var mainGroup = configForm.Init().ControlWidth(150);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_START_NUM")
                         .ControlText(tabConfig.StartingAlarmNum)
                         .UIntChanged(v => tabConfig.StartingAlarmNum = v);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_FORMAT")
                         .ControlText(tabConfig.AlarmNumFormat)
                         .TextChanged(v => tabConfig.AlarmNumFormat = v);
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_SKIP")
                         .ControlText(tabConfig.SkipNumberAfterGroup)
                         .UIntChanged(v => tabConfig.SkipNumberAfterGroup = v);
                    mainGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_COIL1_TYPE")
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(tabConfig.Coil1Type)
                        .SelectedValueChanged<AlarmCoilType>(type => tabConfig.Coil1Type = type);
                    mainGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_COIL2_TYPE")
                        .DisableEdit()
                        .TranslatableEnumItems<AlarmCoilType>()
                        .SelectedValue(tabConfig.Coil2Type)
                        .SelectedValueChanged<AlarmCoilType>(type => tabConfig.Coil2Type = type);

                    mainGroup.AddSeparator().Height(15);

                    var antiSlipGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    antiSlipGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP");
                    antiSlipGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP_AMOUNT")
                         .ControlText(tabConfig.AntiSlipNumber)
                         .UIntChanged(v => tabConfig.AntiSlipNumber = v);
                    antiSlipGroup.AddCheckBox().LocalizedLabel("ALARM_CONFIG_GENERATION_ANTI_SLIP_GEN_EMPTY")
                        .ControlNoAdapt()
                        .Value(tabConfig.GenerateEmptyAlarmAntiSlip)
                        .CheckedChanged(v => tabConfig.GenerateEmptyAlarmAntiSlip = v);

                    mainGroup.AddSeparator().Height(15);

                    var emptyAlarmGroup = mainGroup.AddGroup().ControlWidth(175).NoAdapt();
                    emptyAlarmGroup.AddLabel().Label("Empty Alarms");
                    emptyAlarmGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_NUM")
                         .ControlText(tabConfig.EmptyAlarmAtEnd)
                         .UIntChanged(v => tabConfig.EmptyAlarmAtEnd = v);
                    emptyAlarmGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_ALARM_ADDRESS")
                         .ControlText(tabConfig.EmptyAlarmContactAddress)
                         .TextChanged(v => tabConfig.EmptyAlarmContactAddress = v);

                    var timerGroup = emptyAlarmGroup.AddGroup().ControlWidth(175);
                    timerGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER");
                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_ADDRESS")
                         .ControlText(tabConfig.EmptyAlarmTimerAddress)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerAddress = v);
                    timerGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_TYPE")
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(tabConfig.EmptyAlarmTimerType)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerType = v);
                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_GENERATION_EMPTY_TIMER_VALUE")
                         .ControlText(tabConfig.EmptyAlarmTimerValue)
                         .TextChanged(v => tabConfig.EmptyAlarmTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = tabControl.defaultValuesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_COIL")
                         .ControlText(tabConfig.DefaultCoil1Address)
                         .TextChanged(v => tabConfig.DefaultCoil1Address = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_SET_COIL")
                         .ControlText(tabConfig.DefaultCoil2Address)
                         .TextChanged(v => tabConfig.DefaultCoil2Address = v);

                    var timerGroup = mainGroup.AddGroup().ControlWidth(225).NoAdapt();

                    timerGroup.AddLabel().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER");

                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_ADDRESS")
                         .ControlText(tabConfig.DefaultTimerAddress)
                         .TextChanged(v => tabConfig.DefaultTimerAddress = v);

                    timerGroup.AddComboBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_TYPE")
                         .Items(["TON", "TOF"]).DisableEdit()
                         .ControlText(tabConfig.DefaultTimerType)
                         .TextChanged(v => tabConfig.DefaultTimerType = v);

                    timerGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_DEFAULTS_TIMER_VALUE")
                         .ControlText(tabConfig.DefaultTimerValue)
                         .TextChanged(v => tabConfig.DefaultTimerValue = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = tabControl.valuesPrefixesConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_ALARM_ADDRESS")
                         .ControlText(tabConfig.AlarmAddressPrefix)
                         .TextChanged(v => tabConfig.AlarmAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_COIL_ADDRESS")
                         .ControlText(tabConfig.CoilAddressPrefix)
                         .TextChanged(v => tabConfig.CoilAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_SET_COIL_ADDRESS")
                         .ControlText(tabConfig.SetCoilAddressPrefix)
                         .TextChanged(v => tabConfig.SetCoilAddressPrefix = v);

                    mainGroup.AddTextBox().LocalizedLabel("ALARM_CONFIG_PREFIX_TIMER_ADDRESS")
                         .ControlText(tabConfig.TimerAddressPrefix)
                         .TextChanged(v => tabConfig.TimerAddressPrefix = v);

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
