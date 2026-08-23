using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Template;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmGenSaveV1
    {
        public static readonly int VERSION = 1; //Read via Reflection

        [JsonProperty] public JSScriptSave ScriptSave { get; set; } = new();

        [JsonProperty] public AlarmMainConfiguration AlarmMainConfig { get; set; } = new();
        [JsonProperty] public List<AlarmGenTabSave> TabSaves { get; set; } = [];
        [JsonProperty] public List<AlarmGenTemplateSave> TemplateSaves { get; set; } = [];
    }
}
