using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridCellChange(int columnIndex, int rowIndex)
    {
        public object? OldValue {  get; set; }
        public object? NewValue { get; set; }
        public int RowIndex { get; private set; } = rowIndex;
        public int ColumnIndex { get; private set; } = columnIndex;

        public GridCellChange(DataGridViewCell cell) : this(cell.ColumnIndex, cell.RowIndex) { }
    }

}
