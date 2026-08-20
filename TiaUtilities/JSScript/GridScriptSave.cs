using Newtonsoft.Json;

namespace TiaUtilities.JSScript
{
    public class GridScriptSave
    {
        [JsonProperty] public List<ScriptInfo> Scripts { get; set; } = [];
    }
}
