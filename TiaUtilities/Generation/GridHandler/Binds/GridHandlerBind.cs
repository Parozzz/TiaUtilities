using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility;
using System.Runtime.CompilerServices;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.GridHandler.Binds
{
    public class GridHandlerBind
    {
        public static GridHandlerBind CreateBind<T>(GridHandler<T> gridHandler) where T : GridData
        {
            GirdHadlerBindActions actions = new()
            {
                SuspendLayoutAction = gridHandler.SuspendLayout,
                ResumeLayoutAction = refresh => gridHandler.ResumeLayout(refresh),

                GetData = (row) => gridHandler.DataSource[row],

                GetRowCount = () => gridHandler.RowCount,
                GetColumnCount = () => gridHandler.ColumnCount,

                SelectRow = gridHandler.SelectRow,

                Join = () => gridHandler.DataChangedHandler.Join(),
                End = gridHandler.DataChangedHandler.End,

                GetCurrentCell = gridHandler.GetCurrentCell,
                ChangeCurrentCell = gridHandler.ChangeCurrentCell,
            };
            return new(actions)
            {
                DataTypeName = typeof(T).Name,
                DataColumns = gridHandler.DataSource.DataColumns,

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



                IsGridDataEmpty = rowIndex => gridHandler.DataSource[rowIndex].IsEmpty(),
                IsSameGridHandler = x => Utils.AreEqualsObject(x, gridHandler),
            };
        }

        private class GirdHadlerBindActions()
        {
            public required Action SuspendLayoutAction { get; init; }
            public required Action<bool> ResumeLayoutAction { get; init; }

            public required Func<int, GridData> GetData { get; init; }

            public required Func<int> GetRowCount { get; init; }
            public required Func<int> GetColumnCount { get; init; }

            public required Action<int> SelectRow { get; init; }

            public required Func<GridDataChangedOperationRequest> Join { get; init; }
            public required Action<GridDataChangedOperationRequest> End { get; init; }


            public required Func<DataGridViewCell?> GetCurrentCell {  get; init; }
            public required Action<int, int> ChangeCurrentCell { get; init; }
        }

        public required string DataTypeName { get; init; }
        public required IReadOnlyList<GridDataColumn> DataColumns { get; init; }

        public int RowCount { get => this.actions.GetRowCount(); }
        public int ColumnCount { get => this.actions.GetColumnCount(); }

        public required Func<int, ICollection<int>> GetNotEmptyRowIndexesStartingAt { get; init; }
        public required Func<int, int> GetFirstFullIndexStartingAt { get; init; }

        public required Action<GridDataColumn, int, string> SetColumnData { get; init; }
        public required Func<GridDataColumn, int, string?> GetColumnStringData { get; init; }

        public IReadOnlyList<GridScriptVariable> ScriptVariables { get => this.GetScriptVariables(); }
        public required Func<IReadOnlyList<GridScriptVariable>> GetScriptVariables { private get; init; }

        public required Predicate<int> IsGridDataEmpty { get; init; }
        public required Predicate<object?> IsSameGridHandler { get; init; }

        private readonly GirdHadlerBindActions actions;

        private GridHandlerBind(GirdHadlerBindActions actions)
        {
            this.actions = actions;
        }

        public void SuspendLayout() => this.actions.SuspendLayoutAction();

        public void ResumeLayout(bool refresh = false) => this.actions.ResumeLayoutAction(refresh);

        public void SelectRow(int row) => this.actions.SelectRow(row);

        public DataGridViewCell? GetCurrentCell() => this.actions.GetCurrentCell();

        public void ChangeCurrentCell(int row, int column) => this.actions.ChangeCurrentCell(row, column);

        public GridDataChangedOperationRequest Join() => this.actions.Join();

        public void End(GridDataChangedOperationRequest request) => this.actions.End(request);

        public GridData this[int i]
        {
            get => this.actions.GetData(i);
        }
    }
}
