using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public class Preview
    {
        public string? Prefix { get; set; }
        public string? DefaultValue { get; set; }
        public string? Value { get; set; }
        public string? Suffix { get; set; }

        public string ComposeDefaultValue()
        {
            return Prefix ?? "" + DefaultValue ?? "" + Suffix ?? "";
        }
    }

    public class GridDataPreviewer<T> where T : IGridData
    {
        public Func<int, T, Preview?>? Function { private get; set; }

        public Preview? RequestPreview(GridDataColumn column, T gridData)
        {
            return this.RequestPreview(column.ColumnIndex, gridData);
        }

        public Preview? RequestPreview(int column, T gridData)
        {
            return Function?.Invoke(column, gridData);
        }
    }
}
