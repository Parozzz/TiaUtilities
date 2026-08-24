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

        public IGridData GetGeneric(int index);

        public Dictionary<IGridData, int> GetGenericNotEmptyDataDict(int startRow = 0);

        public IEnumerable<IGridData> GetGenericNotEmptyData(int startRow = 0);

        public Dictionary<IGridData, int> GetGenericNotEmptyClonedDataDict();

        public IEnumerable<IGridData> GetGenericNotEmptyClonedData();

    }
}
