namespace TiaUtilities.Generation.Alarms.Xml
{
    public class AlarmXmlItem()
    {
        public required string TabName { get; init; }
        public required string AlarmVariableName { get; init; }
        public required string AlarmVariableComment { get; init; }

        public required uint HmiID { get; init; }
        public required string HmiAlarmName { get; init; }
        public required string HmiAlarmText { get; init; }
        public required string HmiAlarmClass { get; init; }
        public required string HmiTriggerTag { get; init; }
        public required uint HmiTriggerBit { get; init; }

        public required string DatabaseQuery { get; init; }

        public required List<AlarmXmlHmiParameter> HmiFields { get; init; }

        public override string ToString()
        {
            return $"{HmiAlarmText}";
        }
    }
}
