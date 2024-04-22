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
    public class IOGenerationExcelImportData : IGridData<IOGenerationExcelImportConfiguration>
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
            ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Address"));
            IO_NAME = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("IOName"));
            COMMENT = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Comment"));

            COLUMN_LIST = new List<GridDataColumn>() { ADDRESS, IO_NAME, COMMENT };
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        public string Address { get; set; }
        public string IOName { get; set; }
        public string Comment { get; set; }

        public bool HasPreview(int column, IOGenerationExcelImportConfiguration config)
        {
            return false;
        }

        public GridDataPreview GetPreview(int column, IOGenerationExcelImportConfiguration config)
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
