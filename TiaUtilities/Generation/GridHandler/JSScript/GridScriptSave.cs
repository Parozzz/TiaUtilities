using Newtonsoft.Json;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptSave
    {
        [JsonProperty] public List<ScriptInfo> Scripts { get; set; } = [];
    }
}
