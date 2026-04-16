using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Binds
{
    public class GridHandlerBind
    {
        public static GridHandlerBind CreateBind<T>(GridHandler<T> gridHandler) where T : GridData
        {
            return new()
            {
                DataTypeName = typeof(T).Name,
                DataGridView = gridHandler.DataGridView,
                DataColumns = gridHandler.DataHandler.DataColumns,

                GetNotEmptyRowIndexesStartingAt = gridHandler.DataSource.GetNotEmptyIndexes,
                GetFirstFullIndexStartingAt = gridHandler.DataSource.GetFirstNotEmptyIndexStartingFrom,

                SetColumnData = (column, row, strValue) =>
                {
                    var gridData = gridHandler.DataSource[row];
                    column.SetValueTo(gridData, strValue);
                },
                GetColumnStringData = (column, row) =>
                {
                    var gridData = gridHandler.DataSource[row];
                    return column.GetValueFrom<string>(gridData);
                },
                GetScriptVariables = gridHandler.ScriptVariableList.AsReadOnly,

                SetCacheChanges = () => gridHandler.CacheChanges = true,
                ResetCacheChanges = () => gridHandler.CacheChanges = false,

                SelectRow = gridHandler.SelectRow,

                IsGridDataEmpty = rowIndex => gridHandler.DataSource[rowIndex].IsEmpty(),
                IsSameGridHandler = x => Utils.AreEqualsObject(x, gridHandler),
            };
        }

        public required string DataTypeName { get; init; }
        public required DataGridView DataGridView { get; init; }
        public required IReadOnlyList<GridDataColumn> DataColumns { get; init; }

        public required Func<int, ICollection<int>> GetNotEmptyRowIndexesStartingAt { get; init; }
        public required Func<int, int> GetFirstFullIndexStartingAt { get; init; }

        public required Action<GridDataColumn, int, string> SetColumnData { get; init; }
        public required Func<GridDataColumn, int, string?> GetColumnStringData { get; init; }

        public IReadOnlyList<GridScriptVariable> ScriptVariables { get => this.GetScriptVariables(); }
        public required Func<IReadOnlyList<GridScriptVariable>> GetScriptVariables { private get; init; }


        public required Action SetCacheChanges { get; init; }
        public required Action ResetCacheChanges { get; init; }


        public required Action<int> SelectRow { get; init; }

        public required Predicate<int> IsGridDataEmpty { get; init; }
        public required Predicate<object?> IsSameGridHandler { get; init; }

        private GridHandlerBind()
        {

        }
    }
}
