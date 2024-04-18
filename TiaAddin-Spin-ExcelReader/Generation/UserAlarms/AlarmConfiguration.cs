using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TiaXmlReader.Generation.UserAlarms
{

    public enum AlarmPartitionType
    {
        [Display(Description = "PARITITION_TYPE_ALARM_TYPE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        ALARM_TYPE,
        [Display(Description = "PARITITION_TYPE_DEVICE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        DEVICE
    }

    public enum AlarmGroupingType
    {
        [Display(Description = "GROUPING_TYPE_GROUP", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        GROUP,
        [Display(Description = "GROUPING_TYPE_ONE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        ONE
    }

    public class AlarmConfiguration
    {
        [JsonProperty] public string FCBlockName = "fcAlarmGeneration";
        [JsonProperty] public uint FCBlockNumber = 100;
        [JsonProperty] public bool CoilFirst = true;

        [JsonProperty] public AlarmPartitionType PartitionType = AlarmPartitionType.DEVICE;
        [JsonProperty] public AlarmGroupingType GroupingType = AlarmGroupingType.GROUP;

        [JsonProperty] public uint StartingAlarmNum = 1000;
        [JsonProperty] public string AlarmNumFormat = "000.###";
        [JsonProperty] public uint AntiSlipNumber = 0;
        [JsonProperty] public uint SkipNumberAfterGroup = 0;
        [JsonProperty] public bool GenerateEmptyAlarmAntiSlip = false;
        [JsonProperty] public uint EmptyAlarmAtEnd = 0;
        [JsonProperty] public string EmptyAlarmContactAddress = "FALSE";

        [JsonProperty] public string DefaultCoilAddress = "Alm.Act.Alm{alarm_num}";
        [JsonProperty] public string DefaultSetCoilAddress = "Alm.Mem.Alm{alarm_num}";
        [JsonProperty] public string DefaultTimerAddress = "AlmTimers.Alm[{alarm_num}]";
        [JsonProperty] public string DefaultTimerType = "TON";
        [JsonProperty] public string DefaultTimerValue = "T#0s";

        [JsonProperty] public string AlarmAddressPrefix = "{user_name}.";
        [JsonProperty] public string CoilAddressPrefix = "";
        [JsonProperty] public string SetCoilAddressPrefix = "";
        [JsonProperty] public string TimerAddressPrefix = "";

        [JsonProperty] public string OneEachSegmentName = "Alm{alarm_num} - {user_description} {alarm_description}";
        [JsonProperty] public string OneEachEmptyAlarmSegmentName = "Alm{alarm_num} - SPARE";
        [JsonProperty] public string GroupSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - {user_description}";
        [JsonProperty] public string GroupEmptyAlarmSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - SPARE";
    }
}
