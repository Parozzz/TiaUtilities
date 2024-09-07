using Newtonsoft.Json;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.GenModules.IO.ExcelImporter
{
    public class IOGenerationExcelImportSettings : IGenerationConfiguration
    {
        [JsonProperty] public string AddressCellConfig = "$A";
        [JsonProperty] public string IONameCellConfig = "$A";
        [JsonProperty] public string CommentCellConfig = "$E $F $G $H (P$K - $O)";
        [JsonProperty] public uint StartingRow = 2;
        [JsonProperty] public string IgnoreRowExpressionConfig = "$A != \"\"";
        [JsonProperty] public string JSScript = "";
    }
}
