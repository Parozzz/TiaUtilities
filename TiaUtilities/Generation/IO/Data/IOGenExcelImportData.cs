using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.IO.Data
{
    public class IOGenExcelImportData : GridData
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

        [Locale(nameof(Locale.GENERICS_ADDRESS))] public string? Address { get => this.GetAs<string>(); set => this.Set(value); }
        [Locale(nameof(Locale.IO_SETTINGS_EXCELIMPORT_IO_NAME))] public string ? IOName { get => this.GetAs<string>(); set => this.Set(value); }
        [Locale(nameof(Locale.GENERICS_COMMENT))] public string? Comment { get => this.GetAs<string>(); set => this.Set(value); }


        public override IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public override GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public override void Clear()
        {
            Address = IOName = Comment = null;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(Comment);
        }
    }
}
