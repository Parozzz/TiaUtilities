namespace TiaUtilities.Generation.GridHandler
{
    public class GridHandlerEventCaller(IGridHandlerEventCalls gridHandlerEventCalls)
    {
        public void CallPreSortEvent(GridPreSortEventArgs args) => gridHandlerEventCalls.CallPreSortEvent(args);
        public void CallPostSortEvent(GridPostSortEventArgs args) => gridHandlerEventCalls.CallPostSortEvent(args);
        public void CallExcelDragPreviewEvent(GridExcelDragEventArgs args) => gridHandlerEventCalls.CallExcelDragPreviewEvent(args);
        public void CallExcelDragDoneEvent(GridExcelDragEventArgs args) => gridHandlerEventCalls.CallExcelDragDoneEvent(args);
        public void CallDataChangedEvent(List<GridDataChangedCache> cachedChanges) => gridHandlerEventCalls.CallDataChangedEvent(cachedChanges);
        public void CallLoadDataEvent() => gridHandlerEventCalls.CallLoadDataEvent();
    }
}
