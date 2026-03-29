namespace TiaUtilities.Generation.Alarms.Xml
{
    public class AlarmXmlItem(string tabName, string alarmVariableName, string alarmVariableComment, uint hmiID, string hmiAlarmName, string hmiAlarmText, string hmiAlarmClass, string hmiTriggerTag, uint hmiTriggerBit, List<AlarmXmlHmiParameter> hmiFields)
    {
        public string TabName { get; init; } = tabName;
        public string AlarmVariableName { get; init; } = alarmVariableName;
        public string AlarmVariableComment { get; init; } = alarmVariableComment;

        public uint HmiID { get; init; } = hmiID;
        public string HmiAlarmName { get; init; } = hmiAlarmName;
        public string HmiAlarmText { get; init; } = hmiAlarmText;
        public string HmiAlarmClass { get; init; } = hmiAlarmClass;
        public string HmiTriggerTag { get; init; } = hmiTriggerTag;
        public uint HmiTriggerBit { get; init; } = hmiTriggerBit;

        public List<AlarmXmlHmiParameter> HmiFields { get; init; } = hmiFields;

        public override string ToString()
        {
            return $"{HmiAlarmText}";
        }
    }
}
