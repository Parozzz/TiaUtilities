using Newtonsoft.Json;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.IO.Module.ExcelImporter
{
    public class IOExcelImportConfiguration : ObservableConfiguration
    {
        [JsonProperty] public string AddressCellConfig { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string IONameCellConfig { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string CommentCellConfig { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint StartingRow { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string IgnoreRowExpressionConfig { get => this.GetAs<string>(); set => this.Set(value); }

        public IOExcelImportConfiguration()
        {
            this.AddressCellConfig = "$A";
            this.IONameCellConfig = "$A";
            this.CommentCellConfig = "$E $F $G $H (P$K - $O)";
            this.StartingRow = 2;
            this.IgnoreRowExpressionConfig = "$A != \"\"";
        }
    }
}
