using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmTabConfiguration : ObservableConfiguration
    {
        [JsonProperty] public AlarmPartitionType PartitionType { get => this.GetAs<AlarmPartitionType>(); set => this.Set(value); }
        [JsonProperty] public AlarmGroupingType GroupingType { get => this.GetAs<AlarmGroupingType>(); set => this.Set(value); }

        [JsonProperty] public uint StartingAlarmNum { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string AlarmNumFormat { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint AntiSlipNumber { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public uint SkipNumberAfterGroup { get => this.GetAs<uint>(); set => this.Set(value); }

        [JsonProperty] public bool GenerateEmptyAlarmAntiSlip { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty] public uint EmptyAlarmAtEnd { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmContactAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerType { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string EmptyAlarmTimerValue { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string DefaultCustomVarAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultCustomVarValue { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultCoil1Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public AlarmCoilType DefaultCoil1Type { get => this.GetAs<AlarmCoilType>(); set => this.Set(value); }
        [JsonProperty] public string DefaultCoil2Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public AlarmCoilType DefaultCoil2Type { get => this.GetAs<AlarmCoilType>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerType { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultTimerValue { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string AlarmAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string Coil1AddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string Coil2AddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string TimerAddressPrefix { get => this.GetAs<string>(); set => this.Set(value); }

        public AlarmTabConfiguration()
        {
            this.PartitionType = AlarmPartitionType.DEVICE;
            this.GroupingType = AlarmGroupingType.GROUP;

            this.StartingAlarmNum = 1000;
            this.AlarmNumFormat = "000.###";
            this.AntiSlipNumber = 0;
            this.SkipNumberAfterGroup = 0;

            this.GenerateEmptyAlarmAntiSlip = false;
            this.EmptyAlarmAtEnd = 0;
            this.EmptyAlarmContactAddress = "FALSE";
            this.EmptyAlarmTimerAddress = "\\";
            this.EmptyAlarmTimerType = "TON";
            this.EmptyAlarmTimerValue = "T#0s";

            this.DefaultCustomVarAddress = $"Alm.Level[{GenPlaceholders.Alarms.ALARM_NUM}]";
            this.DefaultCustomVarValue = "0";
            this.DefaultCoil1Address = $"Alm.Act.Alm{GenPlaceholders.Alarms.ALARM_NUM}";
            this.DefaultCoil1Type = AlarmCoilType.COIL;
            this.DefaultCoil2Address = $"Alm.Mem.Alm{GenPlaceholders.Alarms.ALARM_NUM}";
            this.DefaultCoil2Type = AlarmCoilType.SET;
            this.DefaultTimerAddress = "/";
            this.DefaultTimerType = "TON";
            this.DefaultTimerValue = "T#0s";

            this.AlarmAddressPrefix = $"{GenPlaceholders.Alarms.DEVICE_NAME}.";
            this.Coil1AddressPrefix = "";
            this.Coil2AddressPrefix = "";
            this.TimerAddressPrefix = "";
        }
    }
}
