using TiaUtilities.Generation.GridHandler.Events;

namespace TiaXmlReader.Generation.GridHandler.Events
{
    public class GridEvents
    {
        public event GridCellChangeEventHandler CellChange = delegate { };

        public event GridScriptShowVariableEventHandler ScriptShowVariable = delegate {};
        public event GridScriptAddVariableEventHandler ScriptAddVariables = delegate { };

        public event GridPreSortEventHandler PreSort = delegate { };
        public event GridPostSortEventHandler PostSort = delegate { };

        public event GridExcelDragPreviewEventHandler ExcelDragPreview = delegate { };
        public event GridExcelDragDoneEventHandler ExcelDragDone = delegate { };

        public void CellChangeEvent(object? sender, GridCellChangeEventArgs args)
        {
            CellChange(sender, args);
        }

        public void ScriptShowVariableEvent(object? sender, GridScriptShowVariableEventArgs args)
        {
            ScriptShowVariable(sender, args);
        }

        public void ScriptAddVariablesEvent(object? sender, GridScriptAddVariableEventArgs args)
        {
            ScriptAddVariables(sender, args);
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
