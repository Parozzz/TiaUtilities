using Newtonsoft.Json;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.GenForms.IO;
using TiaUtilities.Generation.GenForms.IO.ExcelImporter;
using TiaUtilities.Generation.GenForms.IO.Tab;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.JSScript;

namespace TiaUtilities.Generation.GenForms.IO
{
    public class IOGenSave : IGenProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public IOMainConfiguration MainConfig { get; set; } = new IOMainConfiguration();
        [JsonProperty] public IOGenerationExcelImportSettings ExcelImportConfiguration { get; set; } = new();

        [JsonProperty] public GridScriptContainer.ContainerSave ScriptContainer { get; set; } = new();

        [JsonProperty] public GridSave<IOMainConfiguration, IOSuggestionData> SuggestionGrid { get; set; } = new();
        [JsonProperty] public List<IOGenTabSave> TabSaves { get; set; } = [];

        public IOGenSave()
        {

        }

        public static IOGenSave Load(ref string? filePath)
        {
            return GenUtils.Deserialize<IOGenSave>(ref filePath, EXTENSION) ?? new IOGenSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool showFileDialog = false)
        {
            return GenUtils.Save(this, ref filePath, EXTENSION, showFileDialog);
        }

        public override bool Equals(object? obj)
        {
            return obj is IOGenSave compare &&
                MainConfig.Equals(compare.MainConfig) &&
                ExcelImportConfiguration.Equals(compare.ExcelImportConfiguration) &&
                SuggestionGrid.Equals(compare.SuggestionGrid) && 
                TabSaves.SequenceEqual(compare.TabSaves);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
