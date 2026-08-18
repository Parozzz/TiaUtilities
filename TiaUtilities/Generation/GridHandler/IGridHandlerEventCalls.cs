namespace TiaUtilities.Generation.GridHandler
{
    public interface IGridHandlerEventCalls
    {
        internal void CallPreSortEvent(GridPreSortEventArgs args);
        internal void CallPostSortEvent(GridPostSortEventArgs args);
        internal void CallExcelDragPreviewEvent(GridExcelDragEventArgs args);
        internal void CallExcelDragDoneEvent(GridExcelDragEventArgs args);
        internal void CallDataChangedEvent(List<GridDataChangedCache> cachedChanges);
        internal void CallLoadDataEvent();
    }
}
