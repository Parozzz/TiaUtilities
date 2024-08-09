using Newtonsoft.Json;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.GenForms.IO;
using TiaUtilities.Generation.GenForms.IO.ExcelImporter;
using TiaUtilities.Generation.GenForms.IO.Tab;

namespace TiaUtilities.Generation.GenForms.IO
{
    public class IOGenSave : IGenerationProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public IOMainConfiguration MainConfig { get; set; } = new IOMainConfiguration();
        [JsonProperty] public IOGenerationExcelImportSettings ExcelImportConfiguration { get; set; } = new();
        [JsonProperty] public string SuggestionJSScript { get; set; } = "";
        [JsonProperty] public Dictionary<int, IOSuggestionData> SuggestionData { get; set; } = [];
        [JsonProperty] public List<IOGenTabSave> TabSaves { get; set; } = [];

        public IOGenSave()
        {

        }

        public static IOGenSave Load(ref string? filePath)
        {
            return GenerationUtils.Deserialize<IOGenSave>(ref filePath, EXTENSION) ?? new IOGenSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenerationUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool showFileDialog = false)
        {
            return GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog);
        }

        public override bool Equals(object? obj)
        {
            return obj is IOGenSave compare &&
                MainConfig.Equals(compare.MainConfig) &&
                ExcelImportConfiguration.Equals(compare.ExcelImportConfiguration) &&
                SuggestionJSScript.Equals(compare.SuggestionJSScript) && 
                SuggestionData.SequenceEqual(compare.SuggestionData) &&
                TabSaves.SequenceEqual(compare.TabSaves);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
