using Newtonsoft.Json;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.GenForms.IO;

namespace TiaUtilities.Generation.GenForms.IO
{
    public class IOGenerationProjectSave : IGenerationProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public IOConfiguration IOConfiguration { get; set; } = new IOConfiguration();
        [JsonProperty] public Dictionary<int, IOData> RowDict { get; set; } = [];
        [JsonProperty] public Dictionary<int, IOSuggestionData> SuggestionRowDict = [];

        public IOGenerationProjectSave()
        {

        }

        public static IOGenerationProjectSave Load(ref string? filePath)
        {
            return GenerationUtils.Deserialize<IOGenerationProjectSave>(ref filePath, EXTENSION) ?? new IOGenerationProjectSave();
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
            return obj is IOGenerationProjectSave compare &&
                IOConfiguration.Equals(compare.IOConfiguration) &&
                RowDict.SequenceEqual(compare.RowDict) &&
                SuggestionRowDict.SequenceEqual(compare.SuggestionRowDict);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
