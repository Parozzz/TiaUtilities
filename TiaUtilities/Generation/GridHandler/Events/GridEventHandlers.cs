using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.GridHandler.Events
{
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

    #region JS_SCRIPT
    public delegate void GridScriptAddVariableEventHandler(object? sender, GridScriptAddVariableEventArgs args);
    public class GridScriptAddVariableEventArgs : EventArgs
    {
        public Dictionary<string, object> VariableDict { get; set; } = [];
    }

    public delegate void GridScriptShowVariableEventHandler(object? sender, GridScriptShowVariableEventArgs args);
    public class GridScriptShowVariableEventArgs : EventArgs
    {
        public List<string> VariableList { get; set; } = [];
    }
    #endregion
}
