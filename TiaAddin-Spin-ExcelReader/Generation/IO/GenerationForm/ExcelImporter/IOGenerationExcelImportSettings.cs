using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.AutoSave;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter
{
    public class IOGenerationExcelImportSettings : IGenerationConfiguration, ISettingsAutoSave
    {
        [JsonProperty] public string AddressCellConfig = "$A";
        [JsonProperty] public string IONameCellConfig = "$A";
        [JsonProperty] public string CommentCellConfig = "$E $F $G $H (P$K - $O)";
        [JsonProperty] public uint StartingRow = 2;
        [JsonProperty] public string IgnoreRowExpressionConfig = "$A != \"\"";
        [JsonProperty] public string JSScript = "";
    }
}
