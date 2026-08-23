using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Generation.IO.Module.Tab;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.IO
{
    public class IOGenSaveV1
    {
        public static readonly int VERSION = 1; //Read via Reflection

        [JsonProperty] public IOMainConfiguration MainConfig { get; set; } = new();
        [JsonProperty] public IOExcelImportConfiguration ExcelImportConfiguration { get; set; } = new();

        [JsonProperty] public JSScriptSave ScriptSave { get; set; } = new();

        [JsonProperty] public GridSave<IOSuggestionData> SuggestionGrid { get; set; } = new();
        [JsonProperty] public List<IOGenTabSave> TabSaves { get; set; } = [];
    }
}
