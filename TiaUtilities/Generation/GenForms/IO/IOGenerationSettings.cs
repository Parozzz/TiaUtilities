using Newtonsoft.Json;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.AutoSave;
using TiaUtilities.Generation.GenForms.IO.ExcelImporter;

namespace TiaUtilities.Generation.GenForms.IO
{
    public class IOGenerationSettings : ISettingsAutoSave
    {
        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public IOGenerationExcelImportSettings ExcelImportConfiguration { get; set; } = new IOGenerationExcelImportSettings();
        [JsonProperty] public string JSScript { get; set; } = "";
        [JsonProperty] public string SuggestionJSScript { get; set; } = "";
    }
}
