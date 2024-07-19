using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaUtilities.Generation.GenForms.Alarm;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public class AlarmGenerationProjectSave : IGenerationProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public AlarmConfiguration AlarmConfig { get; set; } = new();
        [JsonProperty] public Dictionary<int, AlarmData> AlarmData = [];
        [JsonProperty] public Dictionary<int, DeviceData> DeviceData = [];

        public AlarmGenerationProjectSave()
        {
        }

        public void AddAlarmData(AlarmData alarmData, int rowIndex)
        {
            AlarmData.Add(rowIndex, alarmData);
        }

        public void AddDeviceData(DeviceData deviceData, int rowIndex)
        {
            DeviceData.Add(rowIndex, deviceData);
        }

        public static AlarmGenerationProjectSave Load(ref string? filePath)
        {
            return GenerationUtils.Deserialize<AlarmGenerationProjectSave>(ref filePath, EXTENSION) ?? new AlarmGenerationProjectSave();
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
            return obj is AlarmGenerationProjectSave compare &&
                AlarmConfig.Equals(compare.AlarmConfig) &&
                AlarmData.SequenceEqual(compare.AlarmData) &&
                DeviceData.SequenceEqual(compare.DeviceData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
