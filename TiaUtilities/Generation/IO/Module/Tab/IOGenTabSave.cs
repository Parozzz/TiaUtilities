using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.IO.Module.Tab
{
    public class IOGenTabSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public string Name { get; set; } = "IOGenTab";
        [JsonProperty] public IOTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<IOData> IOGrid { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return obj is IOGenTabSave compare &&
                Name.Equals(compare.Name) &&
                TabConfig.Equals(compare.TabConfig) &&
                IOGrid.Equals(compare.IOGrid);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
