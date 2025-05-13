using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms
{
    public enum AlarmGroupingType
    {
        [Locale(nameof(Locale.ALARM_CONFIG_GROUPING_TYPE_GROUP))] GROUP,
        [Locale(nameof(Locale.ALARM_CONFIG_GROUPING_TYPE_ONE))] ONE
    }

    public enum AlarmCoilType
    {
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_NONE))] NONE,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_COIL))] COIL,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_NCOIL))] NCOIL,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_SET))] SET,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_RESET))] RESET
    }

    public class AlarmMainConfiguration : ObservableConfiguration
    {
        [JsonProperty] public string FCBlockName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint FCBlockNumber { get => this.GetAs<uint>(); set => this.Set(value); }

        [JsonProperty] public string UDTBlockName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string AlarmNumFormat { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string AlarmNameTemplate { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string AlarmCommentTemplate { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string AlarmCommentTemplateSpare { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string HmiNameTemplate { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string HmiTriggerTagTemplate { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public bool HmiTriggerTagUseWordArray { get => this.GetAs<bool>(); set => this.Set(value); }

        [JsonProperty] public string OneEachSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string OneEachEmptyAlarmSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string GroupSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string GroupEmptyAlarmSegmentName { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public bool EnableCustomVariable { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty] public bool EnableTimer { get => this.GetAs<bool>(); set => this.Set(value); }

        public AlarmMainConfiguration()
        {
            this.FCBlockName = "fcAlarmGeneration";
            this.FCBlockNumber = 100;

            this.UDTBlockName = $"UdtAlm_{TiaUtilities.Generation.Placeholders.Generation.TAB_NAME}";
            this.AlarmNumFormat = "000.###";
            this.AlarmNameTemplate = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM}";
            this.AlarmCommentTemplate = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM} - {TiaUtilities.Generation.Placeholders.Alarms.DEVICE_NAME}: {TiaUtilities.Generation.Placeholders.Alarms.ALARM_DESCRIPTION}";
            this.AlarmCommentTemplateSpare = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM} - SPARE";
            this.HmiNameTemplate = $"{TiaUtilities.Generation.Placeholders.Generation.TAB_NAME}_Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM}";
            this.HmiTriggerTagTemplate = $"AlmAux_Mem{TiaUtilities.Generation.Placeholders.Generation.TAB_NAME}";
            this.HmiTriggerTagUseWordArray = true;

            this.OneEachSegmentName = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM} - {TiaUtilities.Generation.Placeholders.Alarms.DEVICE_DESCRIPTION} {TiaUtilities.Generation.Placeholders.Alarms.ALARM_DESCRIPTION}";
            this.OneEachEmptyAlarmSegmentName = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM} - SPARE";
            this.GroupSegmentName = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM_START} ~ {TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM_END} - {TiaUtilities.Generation.Placeholders.Alarms.DEVICE_DESCRIPTION}";
            this.GroupEmptyAlarmSegmentName = $"Alm{TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM_START} ~ {TiaUtilities.Generation.Placeholders.Alarms.ALARM_NUM_END} - SPARE";

            this.EnableCustomVariable = true;
            this.EnableTimer = true;
        }
    }
}
