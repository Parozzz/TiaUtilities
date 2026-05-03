namespace TiaUtilities.Generation.GridHandler.Data
{
    public class GridDataPreview
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

    public class GridDataPreviewer<T> where T : GridData
    {
        public Func<int, T, GridDataPreview?>? Function { private get; set; }

        public GridDataPreview? RequestPreview(GridDataColumn column, T gridData)
        {
            return this.RequestPreview(column.ColumnIndex, gridData);
        }

        public GridDataPreview? RequestPreview(int column, T gridData)
        {
            return Function?.Invoke(column, gridData);
        }
    }
}
