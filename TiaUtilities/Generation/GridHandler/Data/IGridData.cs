namespace TiaXmlReader.Generation.GridHandler.Data
{
    public interface IGridData
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!
        void Clear();
        bool IsEmpty();
        IReadOnlyList<GridDataColumn> GetColumns();
        GridDataColumn GetColumn(int column);
        object? this[int column] { get; }
    }
}
