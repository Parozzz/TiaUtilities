using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler;
using TiaXmlReader.Generation.IO;

namespace TiaUtilities.Generation.IO.Module.Tab
{
    public class IOGenTabSave
    {
        [JsonProperty] public string Name { get; set; } = "IOGenTab";
        [JsonProperty] public IOTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<IOData> IOGrid { get; set; } = new();
    }
}
