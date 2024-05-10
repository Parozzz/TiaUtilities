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
        public static string DEFAULT_FILE_PATH = Directory.GetCurrentDirectory() + @"\tempAlarmSave." + EXTENSION;

        public class AlarmProjectSaveData
        {
            [JsonProperty] public Dictionary<int, AlarmData> AlarmDataDict = new Dictionary<int, AlarmData>();
            [JsonProperty] public Dictionary<int, DeviceData> DeviceDataDict = new Dictionary<int, DeviceData>();
        }

        [JsonProperty] public AlarmProjectSaveData SaveData { get; set; } = new AlarmProjectSaveData();

        public AlarmGenerationProjectSave()
        {
        }

        public void AddAlarmData(AlarmData alarmData, int rowIndex)
        {
            this.SaveData.AlarmDataDict.Add(rowIndex, alarmData);
        }

        public void AddDeviceData(DeviceData deviceData, int rowIndex)
        {
            this.SaveData.DeviceDataDict.Add(rowIndex, deviceData);
        }
        public static AlarmGenerationProjectSave Load(ref string filePath)
        {
            return GenerationUtils.Load(ref filePath, EXTENSION, out AlarmGenerationProjectSave projectSave) ? projectSave : new AlarmGenerationProjectSave();
        }

        public bool Save(ref string filePath, bool saveAs = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, saveAs);
        }
    }
}
