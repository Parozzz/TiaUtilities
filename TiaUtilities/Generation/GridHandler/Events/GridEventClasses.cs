using DocumentFormat.OpenXml.Spreadsheet;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.Events
{
    #region SELECTED_ROW_CHANGED
    public delegate void GridSelectedRowChangedEventHandler(object? sender, GridSelectedRowChangedArgs args);

    public class GridSelectedRowChangedArgs : EventArgs
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
    }

    #endregion

    #region CELL_CHANGE
    public delegate void GridCellDataChangedEventHandler(object? sender, GridCellDataChangedEventArgs args);

    public class GridCellChangedData(GridData data, string propertyName, GridDataColumn column, int row, object? oldValue, object? newValue)
    {
        public GridData Data { get; init; } = data;
        public string PropertyName { get; init; } = propertyName;
        public GridDataColumn Column { get; init; } = column;
        public int ColumnIndex { get => this.Column.ColumnIndex; }
        public int RowIndex { get; init; } = row;
        public object? OldValue { get; init; } = oldValue;
        public object? NewValue { get; init; } = newValue;

        public GridCellChangedData(GridDataChangedEventArgs args, int row) : this(args.Data, args.PropertyName, args.Column, row, args.OldValue, args.NewValue) { }

        public void RestoreOldValue()
        {
            var property = this.Data.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this.Data, this.OldValue);
            }
        }

        public void RestoreNewValue()
        {
            var property = this.Data.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this.Data, this.NewValue);
            }
        }

        public override string ToString()
        {
            return $"Property: {PropertyName}, Column: {ColumnIndex}, Row: {RowIndex}, Old: {OldValue}, New: {NewValue}";
        }
    }

    public class GridCellDataChangedEventArgs : EventArgs
    {
        public List<GridCellChangedData> ChangedCellDataList { get; init; } = [];
        //public bool IsUndo { get; set; }
    }
    #endregion

    #region EXCEL_DRAG
    public delegate void GridExcelDragPreviewEventHandler(object? sender, GridExcelDragEventArgs args);

    public delegate void GridExcelDragDoneEventHandler(object? sender, GridExcelDragEventArgs args);

    public class GridExcelDragEventArgs
    {
        public int StartingRow { get; set; }
        public int ActualRow { get; set; }
        public uint SelectedRowCount { get; set; }
        public int DraggedColumn { get; set; }
        public bool DraggingDown { get; set; }
        public int TopSelectedRow { get; set; } //Lowest index number
        public int BottomSelectedRow { get; set; } //Highest index number
        public string? TooltipString { get; set; } //Only used for preview
    }
    #endregion

    #region SORT
    public delegate void GridPreSortEventHandler(object? sender, GridPreSortEventArgs args);
    public delegate void GridPostSortEventHandler(object? sender, GridPostSortEventArgs args);

    public class GridPreSortEventArgs(SortOrder oldSortOrder, SortOrder sortOrder, int column)
    {
        public SortOrder OldSortOrder { get; init; } = oldSortOrder;
        public int Column { get; init; } = column;
        public SortOrder SortOrder { get; set; } = sortOrder;
        public bool Handled { get; set; } = false;
    }

    public class GridPostSortEventArgs(SortOrder oldSortOrder, SortOrder sortOrder, int column)
    {
        public SortOrder OldSortOrder { get; init; } = oldSortOrder;
        public int Column { get; init; } = column;
        public SortOrder SortOrder { get; init; } = sortOrder;
    }
    #endregion
}
