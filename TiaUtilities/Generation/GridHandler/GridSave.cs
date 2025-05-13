using Newtonsoft.Json;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridSave<T> where T : IGridData
    {
        [JsonProperty] public Dictionary<int, T> RowData { get; set; } = [];
    }
}
