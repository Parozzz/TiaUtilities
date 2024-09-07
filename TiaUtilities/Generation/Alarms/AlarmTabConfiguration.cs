using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmTabConfiguration : ObservableConfiguration, IGenerationConfiguration
    {
        [JsonProperty] public AlarmPartitionType PartitionType { get => this.GetAs<AlarmPartitionType>(); set => this.Set(value); }
        [JsonProperty] public AlarmGroupingType GroupingType { get => this.GetAs<AlarmGroupingType>(); set => this.Set(value); }

        [JsonProperty] public uint StartingAlarmNum { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string AlarmNumFormat { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint AntiSlipNumber { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public uint SkipNumberAfterGroup { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public AlarmCoilType DefaultCoil1Type { get => this.GetAs<AlarmCoilType>(); set => this.Set(value); }
        [JsonProperty] public AlarmCoilType DefaultCoil2Type { get => this.GetAs<AlarmCoilType>(); set => this.Set(value); }

        [JsonProperty] public bool GenerateEmptyAlarmAntiSlip { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty] public uint EmptyAlarmAtEnd { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmContactAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerType { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerValue { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string DefaultCoil1Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultCoil2Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerType { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerValue { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string AlarmAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string CoilAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string SetCoilAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string TimerAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }

        public AlarmTabConfiguration()
        {
            PartitionType = AlarmPartitionType.DEVICE;
            GroupingType = AlarmGroupingType.GROUP;

            StartingAlarmNum = 1000;
            AlarmNumFormat = "000.###";
            AntiSlipNumber = 0;
            SkipNumberAfterGroup = 0;
            DefaultCoil1Type = AlarmCoilType.COIL;
            DefaultCoil2Type = AlarmCoilType.SET;

            GenerateEmptyAlarmAntiSlip = false;
            EmptyAlarmAtEnd = 0;
            EmptyAlarmContactAddress = "FALSE";
            EmptyAlarmTimerAddress = "\\";
            EmptyAlarmTimerType = "TON";
            EmptyAlarmTimerValue = "T#0s";

            DefaultCoil1Address = "Alm.Act.Alm{alarm_num}";
            DefaultCoil2Address = "Alm.Mem.Alm{alarm_num}";
            DefaultTimerAddress = "/";
            DefaultTimerType = "TON";
            DefaultTimerValue = "T#0s";

            AlarmAddressPrefix = "{device_name}.";
            CoilAddressPrefix = "";
            SetCoilAddressPrefix = "";
            TimerAddressPrefix = "";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out _);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
