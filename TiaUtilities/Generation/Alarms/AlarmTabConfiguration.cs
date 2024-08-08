using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmTabConfiguration : IGenerationConfiguration, ISettingsAutoSave
    {
        [JsonProperty] public AlarmPartitionType PartitionType = AlarmPartitionType.DEVICE;
        [JsonProperty] public AlarmGroupingType GroupingType = AlarmGroupingType.GROUP;

        [JsonProperty] public uint StartingAlarmNum = 1000;
        [JsonProperty] public string AlarmNumFormat = "000.###";
        [JsonProperty] public uint AntiSlipNumber = 0;
        [JsonProperty] public uint SkipNumberAfterGroup = 0;
        [JsonProperty] public AlarmCoilType Coil1Type = AlarmCoilType.COIL;
        [JsonProperty] public AlarmCoilType Coil2Type = AlarmCoilType.SET;

        [JsonProperty] public bool GenerateEmptyAlarmAntiSlip = false;
        [JsonProperty] public uint EmptyAlarmAtEnd = 0;
        [JsonProperty] public string EmptyAlarmContactAddress = "FALSE";
        [JsonProperty] public string EmptyAlarmTimerAddress = "\\";
        [JsonProperty] public string EmptyAlarmTimerType = "TON";
        [JsonProperty] public string EmptyAlarmTimerValue = "T#0s";

        [JsonProperty] public string DefaultCoil1Address = "Alm.Act.Alm{alarm_num}";
        [JsonProperty] public string DefaultCoil2Address = "Alm.Mem.Alm{alarm_num}";
        [JsonProperty] public string DefaultTimerAddress = "/";
        [JsonProperty] public string DefaultTimerType = "TON";
        [JsonProperty] public string DefaultTimerValue = "T#0s";

        [JsonProperty] public string AlarmAddressPrefix = "{device_address}.";
        [JsonProperty] public string CoilAddressPrefix = "";
        [JsonProperty] public string SetCoilAddressPrefix = "";
        [JsonProperty] public string TimerAddressPrefix = "";

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenerationUtils.CompareJsonFieldsAndProperties(this, obj, out object invalid);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
