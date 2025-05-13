using System.Reflection;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public class GridDataHandler<T> where T : IGridData
    {
        private readonly DataGridView dataGridView;
        public IReadOnlyList<GridDataColumn> DataColumns { get; private set; }

        public GridDataHandler(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            this.DataColumns = ValidateColumnList();
        }

        private IReadOnlyList<GridDataColumn> ValidateColumnList()
        {
            var type = typeof(T);
            var fieldInfo = type.GetField("COLUMN_LIST", BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo == null)
            {
                throw new MissingFieldException($"IGridData must have a public static List<GridDataColumn> COLUMN_LIST field for {type.Name}");
            }

            if (fieldInfo.FieldType != typeof(IReadOnlyList<GridDataColumn>))
            {
                throw new MissingFieldException($"IGridData must have a public static List<GridDataColumn> COLUMN_LIST field for {type.Name}");
            }

            return (IReadOnlyList<GridDataColumn>)fieldInfo.GetValue(null);
        }

        public T CreateInstance()
        {
            var type = typeof(T);
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        public void CopyValues(T copyFrom, T moveTo)
        {
            foreach (var dataColumn in DataColumns)
            {
                var copeFromValue = dataColumn.GetValueFrom<object>(copyFrom);
                dataColumn.SetValueTo(moveTo, copeFromValue);
            }
        }

        public List<GridCellChange> CreateCellChanges(int rowIndex, T data)
        {
            return CreateCellChanges(new Dictionary<int, T> { { rowIndex, data } });
        }

        public List<GridCellChange> CreateCellChanges(Dictionary<int, T> dataDict)
        {
            var cellChangeList = new List<GridCellChange>();

            foreach (var entry in dataDict)
            {
                var rowIndex = entry.Key;
                var data = entry.Value;

                foreach (var dataColumn in DataColumns)
                {
                    var value = dataColumn.GetValueFrom<object>(data);
                    var columnIndex = dataColumn.ColumnIndex;
                    cellChangeList.Add(new GridCellChange(columnIndex, rowIndex) { NewValue = value });
                }
            }

            return cellChangeList;
        }
    }
}
