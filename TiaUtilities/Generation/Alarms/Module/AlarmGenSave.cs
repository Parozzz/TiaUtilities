using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenSave
    {
        [JsonProperty] public GridScriptContainer.ContainerSave ScriptContainer { get; set; } = new();

        [JsonProperty] public AlarmMainConfiguration AlarmMainConfig { get; set; } = new();
        [JsonProperty] public List<AlarmGenTabSave> TabSaves { get; set; } = [];
    }
}
