using Newtonsoft.Json;

namespace TiaUtilities.JSScript
{
    public class JSScriptSave
    {
        [JsonProperty] public List<ScriptInfo> Scripts { get; set; } = [];
    }
}
