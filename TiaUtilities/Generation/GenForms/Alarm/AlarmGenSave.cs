using Newtonsoft.Json;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaUtilities.Generation.GridHandler.JSScript;

namespace TiaUtilities.Generation.GenForms.Alarm
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
