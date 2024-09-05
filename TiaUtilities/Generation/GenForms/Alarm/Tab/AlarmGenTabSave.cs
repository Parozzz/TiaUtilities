using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTabSave : IGenProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public string Name { get; set; } = "AlarmGenTab";
        [JsonProperty] public AlarmTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<AlarmTabConfiguration, AlarmData> AlarmGrid { get; set; } = new();
        [JsonProperty] public GridSave<AlarmTabConfiguration, DeviceData> DeviceGrid { get; set; } = new();

        public static AlarmGenSave Load(ref string? filePath)
        {
            return GenUtils.Deserialize<AlarmGenSave>(ref filePath, EXTENSION) ?? new AlarmGenSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool saveAs = false)
        {
            return GenUtils.Save(this, ref filePath, EXTENSION, saveAs);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlarmGenTabSave compare &&
                TabConfig.Equals(compare.TabConfig) &&
                AlarmGrid.Equals(compare.AlarmGrid) &&
                DeviceGrid.Equals(compare.DeviceGrid);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
