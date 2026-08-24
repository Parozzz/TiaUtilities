using TiaUtilities.Generation.GridHandler.GridImprovements;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.GridHandler
{
    public interface IGridHandler
    {
        public int RowCount { get; }
        public int ColumnCount { get; }

        public GridColumnHandler Columns { get; init; }
        public GridDataChangedHandler DataChangedHandler { get; init; }
        public IGridDataSource DataSource { get; }
        public List<JSScriptVariable> ScriptVariableList { get; init; }
        public GridSelectionBorder SelectionBorder { get; init; }

        public GridViewManipulator ViewManipulator { get; init; }
    }
}
