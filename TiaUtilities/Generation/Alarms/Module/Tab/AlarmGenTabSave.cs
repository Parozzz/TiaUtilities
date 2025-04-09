using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmGenTabSave
    {
        [JsonProperty] public string Name { get; set; } = "AlarmGenTab";
        [JsonProperty] public AlarmTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<DeviceData> DeviceGrid { get; set; } = new();
    }
}
