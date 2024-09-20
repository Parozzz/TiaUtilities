using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.GridHandler;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptHandlerBind
    {
        public static GridScriptHandlerBind CreateBind<H>(GridHandler<H> gridHandler) where H : IGridData
        {
            return new()
            {
                DataTypeName = typeof(H).Name,
                DataGridView = gridHandler.DataGridView,
                GetNotEmptyRowIndexes = () => gridHandler.DataSource.GetNotEmptyDataDict().Values,
                ClearCachedCellChange = gridHandler.ClearCachedCellChange,
                ApplyCachedCellChange = () => gridHandler.ExecuteCachedCellChange(),
                IsGridDataEmpty = rowIndex => gridHandler.DataSource[rowIndex].IsEmpty(),
                GetFirstFullIndexStartingAt = gridHandler.DataSource.GetFirstNotEmptyIndexStartingFrom,
                SelectRow = gridHandler.SelectRow,
                IsSameGridHandler = x => x.Equals(gridHandler),
            };
        }

        public required string DataTypeName { get; init; }
        public required DataGridView DataGridView { get; init; }
        public required Func<ICollection<int>> GetNotEmptyRowIndexes { get; init; }
        public required Action ClearCachedCellChange { get; init; }
        public required Action ApplyCachedCellChange { get; init; }
        public required Predicate<int> IsGridDataEmpty { get; init; }
        public required Func<int, int> GetFirstFullIndexStartingAt { get; init; }
        public required Action<int> SelectRow { get; init; }
        public required Predicate<object> IsSameGridHandler {  get; init; }

        private GridScriptHandlerBind() { }
    }
}
