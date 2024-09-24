using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.Find_Replace
{
    public class GridFindHandlerBind
    {
        public static GridFindHandlerBind Bind<T>(GridHandler<T> gridHandler) where T : IGridData
        {
            return new()
            {
                DataGridView = gridHandler.DataGridView,
                DataColumns = gridHandler.DataHandler.DataColumns,
                GetNotEmptyRowIndexesStartingAt = gridHandler.DataSource.GetNotEmptyIndexes,
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
                AddCachedCellChange = gridHandler.AddCachedCellChange,
                ClearCachedCellChange = gridHandler.ClearCachedCellChange,
                ApplyCachedCellChange = () => gridHandler.ExecuteCachedCellChange(),
            };
        }

        public required DataGridView DataGridView { get; init; }
        public required IReadOnlyList<GridDataColumn> DataColumns { get; init; }
        public required Func<int, ICollection<int>> GetNotEmptyRowIndexesStartingAt { get; init; }
        public required Action<GridDataColumn, int, string> SetColumnData { get; init; }
        public required Func<GridDataColumn, int, string?> GetColumnStringData { get; init; }
        public required Action<GridCellChange> AddCachedCellChange { get; init; }
        public required Action ClearCachedCellChange { get; init; }
        public required Action ApplyCachedCellChange { get; init; }


        private GridFindHandlerBind()
        {

        }
    }
}
