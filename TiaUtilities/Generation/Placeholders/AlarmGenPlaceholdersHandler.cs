using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Placeholders.Data;

namespace TiaUtilities.Generation.Placeholders
{
    public class AlarmGenPlaceholdersHandler(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig) : GenPlaceholderHandler
    {
        public DeviceData DeviceData
        {
            set
            {
                AddOrReplace(GenPlaceholders.Alarms.DEVICE_NAME, new StringGenPlaceholderData() { Value = value.Name });
                AddOrReplace(GenPlaceholders.Alarms.DEVICE_DESCRIPTION, new StringGenPlaceholderData() { Value = value.Description });
            }
        }

        public AlarmData AlarmData
        {
            set
            {
                AddOrReplace(GenPlaceholders.Alarms.ALARM_DESCRIPTION, new StringGenPlaceholderData() { Value = value.Description });
            }
        }

        public void SetAlarmNum(uint alarmNum, string alarmNumFormat)
        {
            AddOrReplace(GenPlaceholders.Alarms.ALARM_NUM, new UIntGenPlaceholderData()
            {
                Value = alarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public void SetStartEndAlarmNum(uint startAlarmNum, uint endAlarmNum, string alarmNumFormat)
        {
            AddOrReplace(GenPlaceholders.Alarms.ALARM_NUM_START, new UIntGenPlaceholderData()
            {
                Value = startAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });

            AddOrReplace(GenPlaceholders.Alarms.ALARM_NUM_END, new UIntGenPlaceholderData()
            {
                Value = endAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public void AddGenericPlaceholder(int num, string value)
        {
            var placeholder = GenPlaceholders.Alarms.DEVICE_PLACEHOLDERS_GENERIC.Replace("x", num.ToString());
            AddOrReplace(placeholder, new StringGenPlaceholderData() { Value = value });
        }
    }
}
