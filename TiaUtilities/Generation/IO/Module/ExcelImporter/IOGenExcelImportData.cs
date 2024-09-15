using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.IO.Module.ExcelImporter
{
    public class IOGenExcelImportData : IGridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn IO_NAME;
        public static readonly GridDataColumn COMMENT;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static IOGenExcelImportData()
        {
            var type = typeof(IOGenExcelImportData);
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(Address));
            IO_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOName));
            COMMENT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(Comment));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        public string? Address { get; set; }
        public string? IOName { get; set; }
        public string? Comment { get; set; }

        public object? this[int column]
        {
            get
            {
                if (column < 0 || column >= COLUMN_LIST.Count)
                {
                    throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }

                return COLUMN_LIST[column].PropertyInfo.GetValue(this);
            }
        }
        public IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public void Clear()
        {
            Address = IOName = Comment = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(Comment);
        }
    }
}
