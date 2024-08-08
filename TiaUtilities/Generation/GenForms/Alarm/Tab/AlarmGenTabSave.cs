using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTabSave : IGenerationProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public string Name { get; set; } = "AlarmGenTab";
        [JsonProperty] public AlarmTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public Dictionary<int, AlarmData> AlarmData = [];
        [JsonProperty] public Dictionary<int, DeviceData> DeviceData = [];

        public void AddAlarmData(AlarmData alarmData, int rowIndex)
        {
            AlarmData.Add(rowIndex, alarmData);
        }

        public void AddDeviceData(DeviceData deviceData, int rowIndex)
        {
            DeviceData.Add(rowIndex, deviceData);
        }

        public static AlarmGenSave Load(ref string? filePath)
        {
            return GenerationUtils.Deserialize<AlarmGenSave>(ref filePath, EXTENSION) ?? new AlarmGenSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenerationUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool saveAs = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, saveAs);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlarmGenTabSave compare &&
                TabConfig.Equals(compare.TabConfig) &&
                AlarmData.SequenceEqual(compare.AlarmData) &&
                DeviceData.SequenceEqual(compare.DeviceData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
