using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.Events
{
    public class GridEvents<T> where T : GridData
    {
        public event GridSelectedRowChangedEventHandler RowSelectedChanged = delegate { };
        public event GridCellDataChangedEventHandler CellDataChanged = delegate { };

        public event GridPreSortEventHandler PreSort = delegate { };
        public event GridPostSortEventHandler PostSort = delegate { };

        public event GridExcelDragPreviewEventHandler ExcelDragPreview = delegate { };
        public event GridExcelDragDoneEventHandler ExcelDragDone = delegate { };

        public void RowSelectedChangedEvent(object? sender, GridSelectedRowChangedArgs args)
        {
            RowSelectedChanged(sender, args);
        }

        public void CellDataChangedEvent(object? sender, GridCellDataChangedEventArgs args)
        {
            CellDataChanged(sender, args);
        }

        public void PreSortEvent(object? sender, GridPreSortEventArgs args)
        {
            PreSort(sender, args);
        }

        public void PostSortEvent(object? sender, GridPostSortEventArgs args)
        {
            PostSort(sender, args);
        }

        public void ExcelDragPreviewEvent(object? sender, GridExcelDragEventArgs args)
        {
            ExcelDragPreview(sender, args);
        }

        public void ExcelDragDoneEvent(object? sender, GridExcelDragEventArgs args)
        {
            ExcelDragDone(sender, args);
        }
    }
}
