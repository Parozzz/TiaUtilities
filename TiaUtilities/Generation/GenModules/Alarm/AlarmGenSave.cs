using Newtonsoft.Json;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.GenModules.Alarm;
using TiaUtilities.Generation.GenModules.Alarm.Tab;

namespace TiaUtilities.Generation.GenModules.Alarm
{
    public class AlarmGenSave
    {
        [JsonProperty] public GridScriptContainer.ContainerSave ScriptContainer { get; set; } = new();

        [JsonProperty] public AlarmMainConfiguration AlarmMainConfig { get; set; } = new();
        [JsonProperty] public List<AlarmGenTabSave> TabSaves { get; set; } = [];

        public override bool Equals(object? obj)
        {
            return obj is AlarmGenSave compare &&
                TabSaves.SequenceEqual(compare.TabSaves);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
