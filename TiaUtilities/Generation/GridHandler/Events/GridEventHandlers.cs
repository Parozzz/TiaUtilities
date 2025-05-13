using TiaUtilities.Generation.GridHandler;

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
    public delegate void GridCellChangeEventHandler(object? sender, GridCellChangeEventArgs args);

    public class GridCellChangeEventArgs : EventArgs
    {
        public List<GridCellChange>? CellChangeList { get; set; }
        public bool IsUndo { get; set; }
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
