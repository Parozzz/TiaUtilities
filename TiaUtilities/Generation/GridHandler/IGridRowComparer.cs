using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.GridHandler
{
    public interface IGridRowComparer<T> : IComparer<T> where T : IGridData
    {
        void SetSortOrder(SortOrder sortOrder);

        SortOrder GetSortOrder();

        bool CanSortColumn(int column);

        void SetSortedColumn(int column);
    }
}
