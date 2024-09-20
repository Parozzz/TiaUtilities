using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridCellChange(int columnIndex, int rowIndex)
    {
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
        public int RowIndex { get; private set; } = rowIndex;
        public int ColumnIndex { get; private set; } = columnIndex;

        public GridCellChange(DataGridViewCell cell) : this(cell.ColumnIndex, cell.RowIndex) { }

        public GridCellChange(GridDataColumn column, int rowIndex) : this(column.ColumnIndex, rowIndex) { }

        public bool IsOldValueEmptyString()
        {
            return IsObjectStringEmpty(OldValue);
        }

        public bool IsOldValueFullString()
        {
            return IsObjectStringFull(OldValue);
        }

        public bool IsNewValueEmptyString()
        {
            return IsObjectStringEmpty(NewValue);
        }

        public bool IsNewValueFullString()
        {
            return IsObjectStringFull(NewValue);
        }

        private static bool IsObjectStringEmpty(object? obj)
        {
            return obj == null || (obj is string str && string.IsNullOrWhiteSpace(str));
        }

        private static bool IsObjectStringFull(object? obj)
        {
            return obj != null && obj is string str && !string.IsNullOrWhiteSpace(str);
        }
    }

}
