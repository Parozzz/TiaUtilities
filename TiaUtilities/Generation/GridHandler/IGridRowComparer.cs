using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public interface IGridRowComparer<T> : IComparer<T> where T : GridRowData
    {
        void SetSortOrder(SortOrder sortOrder);

        SortOrder GetSortOrder();

        bool CanSortColumn(int column);

        void SetSortedColumn(int column);
    }
}
