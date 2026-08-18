using DocumentFormat.OpenXml.Spreadsheet;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    #region DATA_LOADED
    public delegate void GridDataLoadedEvent(object? sender, EventArgs eventArgs);
    #endregion

    #region DATA_CHANGED
    public delegate void GridDataChangedEventHandler(object? sender, GridDataChangedEventArgs args);

    public class GridChangedData(GridData gridData, string propertyName, GridDataColumn column, int row, object? oldValue, object? newValue)
    {
        public GridData GridData { get; init; } = gridData;
        public string PropertyName { get; init; } = propertyName;
        public GridDataColumn Column { get; init; } = column;
        public int ColumnIndex { get => this.Column.ColumnIndex; }
        public int RowIndex { get; init; } = row;
        public object? OldValue { get; init; } = oldValue;
        public object? NewValue { get; init; } = newValue;

        public GridChangedData(GridDataPropertyChangedEventArgs args, int row) : this(args.Data, args.PropertyName, args.Column, row, args.OldValue, args.NewValue) { }

        public void RestoreOldValue()
        {
            var property = this.GridData.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this.GridData, this.OldValue);
            }
        }

        public void RestoreNewValue()
        {
            var property = this.GridData.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this.GridData, this.NewValue);
            }
        }

        public override string ToString()
        {
            return $"Property: {PropertyName}, Column: {ColumnIndex}, Row: {RowIndex}, Old: {OldValue}, New: {NewValue}";
        }
    }

    public class GridDataChangedEventArgs : EventArgs
    {
        public List<GridChangedData> ChangedCellDataList { get; init; } = [];
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
