using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmGenTabSave
    {
        [JsonProperty] public string Name { get; set; } = "AlarmGenTab";
        [JsonProperty] public AlarmTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<DeviceData> DeviceGrid { get; set; } = new();
    }
}
