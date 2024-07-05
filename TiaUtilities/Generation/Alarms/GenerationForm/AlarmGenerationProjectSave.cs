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
using TiaXmlReader.Generation.Alarms.GenerationForm;
using TiaXmlReader.Generation.IO.GenerationForm;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public class AlarmGenerationProjectSave
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
            this.AlarmData.Add(rowIndex, alarmData);
        }

        public void AddDeviceData(DeviceData deviceData, int rowIndex)
        {
            this.DeviceData.Add(rowIndex, deviceData);
        }

        public static AlarmGenerationProjectSave Load(ref string? filePath)
        {
            return GenerationUtils.Load<AlarmGenerationProjectSave>(ref filePath, EXTENSION) ?? new AlarmGenerationProjectSave();
        }

        public bool Save(ref string? filePath, bool saveAs = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, saveAs);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlarmGenerationProjectSave compare &&
                this.AlarmConfig.Equals(compare.AlarmConfig) &&
                this.AlarmData.SequenceEqual(compare.AlarmData) &&
                this.DeviceData.SequenceEqual(compare.DeviceData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
