using DocumentFormat.OpenXml.Drawing;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public interface IGridDataSource
    {
        public int Count { get; }
        public IReadOnlyList<GridDataColumn> DataColumns { get; init; }

        public void InitializeData(uint dataAmount);

        public List<int> GetFirstEmptyRowIndexes(int num);

        public int GetFirstNotEmptyIndexStartingFrom(int indexStart);

        public ICollection<int> GetNotEmptyIndexes(int startRow = 0);

        public IGridRowData GetGeneric(int index);

        public Dictionary<IGridRowData, int> GetGenericNotEmptyDataDict(int startRow = 0);

        public IEnumerable<IGridRowData> GetGenericNotEmptyData(int startRow = 0);

        public Dictionary<IGridRowData, int> GetGenericNotEmptyClonedDataDict();

        public IEnumerable<IGridRowData> GetGenericNotEmptyClonedData();

    }
}
