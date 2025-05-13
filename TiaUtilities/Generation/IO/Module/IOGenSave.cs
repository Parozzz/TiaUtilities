using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.IO.Module.ExcelImporter;
using TiaUtilities.Generation.IO.Module.Tab;
using TiaUtilities.Generation.IO;

namespace TiaUtilities.Generation.IO.Module
{
    public class IOGenSave
    {
        [JsonProperty] public IOMainConfiguration MainConfig { get; set; } = new();
        [JsonProperty] public IOExcelImportConfiguration ExcelImportConfiguration { get; set; } = new();

        [JsonProperty] public GridScriptSave ScriptSave { get; set; } = new();

        [JsonProperty] public GridSave<IOSuggestionData> SuggestionGrid { get; set; } = new();
        [JsonProperty] public List<IOGenTabSave> TabSaves { get; set; } = [];
    }
}
