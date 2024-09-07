using TiaXmlReader.Generation.Alarms;
using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTabSave
    {
        [JsonProperty] public string Name { get; set; } = "AlarmGenTab";
        [JsonProperty] public AlarmTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<AlarmTabConfiguration, AlarmData> AlarmGrid { get; set; } = new();
        [JsonProperty] public GridSave<AlarmTabConfiguration, DeviceData> DeviceGrid { get; set; } = new();

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
