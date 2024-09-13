using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Languages;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{
    public enum AlarmGroupingType
    {
        [Locale(nameof(Locale.ALARM_CONFIG_GROUPING_TYPE_GROUP))] GROUP,
        [Locale(nameof(Locale.ALARM_CONFIG_GROUPING_TYPE_ONE))] ONE
    }

    public enum AlarmPartitionType
    {
        [Locale(nameof(Locale.ALARM_CONFIG_PARTITION_TYPE_ALARM_TYPE))] ALARM_TYPE,
        [Locale(nameof(Locale.ALARM_CONFIG_PARTITION_TYPE_DEVICE))] DEVICE
    }

    public enum AlarmCoilType
    {
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_NONE))] NONE,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_COIL))] COIL,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_SET))] SET,
        [Locale(nameof(Locale.ALARM_CONFIG_COIL_TYPE_RESET))] RESET
    }

    public class AlarmMainConfiguration : ObservableConfiguration, IGenerationConfiguration
    {
        [JsonProperty] public string FCBlockName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint FCBlockNumber { get => this.GetAs<uint>(); set => this.Set(value); }

        [JsonProperty] public string OneEachSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string OneEachEmptyAlarmSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string GroupSegmentName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string GroupEmptyAlarmSegmentName { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string AlarmTextInList { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTextInList { get => this.GetAs<string>(); set => this.Set(value); }

        public AlarmMainConfiguration()
        {
            FCBlockName = "fcAlarmGeneration";
            FCBlockNumber = 100;

            OneEachSegmentName = "Alm{alarm_num} - {device_description} {alarm_description}";
            OneEachEmptyAlarmSegmentName = "Alm{alarm_num} - SPARE";
            GroupSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - {device_description}";
            GroupEmptyAlarmSegmentName = "Alm{alarm_num_start} ~ {alarm_num_end} - SPARE";

            AlarmTextInList = "{device_name} - {alarm_description}";
            EmptyAlarmTextInList = "{device_name} - SPARE";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out object invalid);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
