using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridSave<T> where T : GridRowData
    {
        [JsonProperty] public Dictionary<int, T> RowData { get; set; } = [];
    }
}
