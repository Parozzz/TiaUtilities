using Newtonsoft.Json;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{
    public enum AlarmGroupingType
    {
        [Localization("ALARM_CONFIG_GROUPING_TYPE_GROUP")] GROUP,
        [Localization("ALARM_CONFIG_GROUPING_TYPE_ONE")] ONE
    }

    public enum AlarmPartitionType
    {
        [Localization("ALARM_CONFIG_PARTITION_TYPE_ALARM_TYPE")] ALARM_TYPE,
        [Localization("ALARM_CONFIG_PARTITION_TYPE_DEVICE")] DEVICE
    }

    public class AlarmConfiguration : IGenerationConfiguration, ISettingsAutoSave
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
        [JsonProperty] public string EmptyAlarmTimerAddress = "\\";
        [JsonProperty] public string EmptyAlarmTimerType = "TON";
        [JsonProperty] public string EmptyAlarmTimerValue = "T#0s";

        [JsonProperty] public string DefaultCoilAddress = "Alm.Act.Alm{alarm_num}";
        [JsonProperty] public string DefaultSetCoilAddress = "Alm.Mem.Alm{alarm_num}";
        [JsonProperty] public string DefaultTimerAddress = "AlmTimers.Alm[{alarm_num}]";
        [JsonProperty] public string DefaultTimerType = "TON";
        [JsonProperty] public string DefaultTimerValue = "T#0s";

        [JsonProperty] public string AlarmAddressPrefix = "{device_address}.";
        [JsonProperty] public string CoilAddressPrefix = "";
        [JsonProperty] public string SetCoilAddressPrefix = "";
        [JsonProperty] public string TimerAddressPrefix = "";

        [JsonProperty] public string OneEachSegmentName = "Alm{alarm_num} - {device_description} {alarm_description}";
        [JsonProperty] public string OneEachEmptyAlarmSegmentName = "Alm{alarm_num} - SPARE";
        [JsonProperty] public string GroupSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - {device_description}";
        [JsonProperty] public string GroupEmptyAlarmSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - SPARE";

        [JsonProperty] public string AlarmTextInList = "{device_name} - {alarm_description}";
        [JsonProperty] public string EmptyAlarmTextInList = "{device_name} - SPARE";
    }
}
