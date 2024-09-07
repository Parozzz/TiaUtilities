using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenModules.Alarm;

namespace TiaUtilities.Generation.GenModules.Alarm
{
    public class AlarmGenConfigHandler(AlarmGenConfigTopControl configControl, AlarmMainConfiguration config)
    {
        public void Init()
        {
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
