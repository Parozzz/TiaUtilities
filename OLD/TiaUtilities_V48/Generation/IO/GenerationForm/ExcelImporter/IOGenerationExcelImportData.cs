using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter;

namespace TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter
{
    public class IOGenerationExcelImportData : IGridData<IOGenerationExcelImportSettings>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn IO_NAME;
        public static readonly GridDataColumn COMMENT;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static IOGenerationExcelImportData()
        {
            var type = typeof(IOGenerationExcelImportData);
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOGenerationExcelImportData.Address));
            IO_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOGenerationExcelImportData.IOName));
            COMMENT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOGenerationExcelImportData.Comment));

            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        public string Address { get; set; }
        public string IOName { get; set; }
        public string Comment { get; set; }

        public object this[int column]
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

        public GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }
        public GridDataPreview GetPreview(GridDataColumn column, IOGenerationExcelImportSettings config)
        {
            return this.GetPreview(column.ColumnIndex, config);
        }

        public GridDataPreview GetPreview(int column, IOGenerationExcelImportSettings config)
        {
            return null;
        }

        public void Clear()
        {
            this.Address = this.IOName = this.Comment = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(Comment);
        }
    }
}
