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
    public class GridCellChange
    {
        //public DataGridViewCell cell;
        //public object OldValue { get; set; }
        public object OldValue {  get; set; }
        public object NewValue { get; set; }
        public int RowIndex { get; private set; } = -1; //=> cell.RowIndex;
        public int ColumnIndex { get; private set; } = -1; //=> cell.ColumnIndex;

        /*
        public GridCellChange(DataGridView dataGridView, int column, int row) : this(dataGridView.Rows[row].Cells[column])
        {
        }
        */

        public GridCellChange(int columnIndex, int rowIndex) 
        { 
            this.ColumnIndex = columnIndex;
            this.RowIndex = rowIndex;
        }

        public GridCellChange(DataGridViewCell cell) : this(cell.ColumnIndex, cell.RowIndex)
        {
            //this.cell = cell;
            //this.OldValue = cell?.Value;
        }
        
        /*
        public void ReverseValues()
        {
            var savedOldValue = this.OldValue;
            this.OldValue = NewValue;
            this.NewValue = savedOldValue;
        }

        public void ApplyNewValue()
        {
            if (cell != null)
            {
                this.cell.Value = this.NewValue;
            }
        }

        public void ApplyOldValue()
        {
            if (cell != null)
            {
                this.cell.Value = this.OldValue;
            }
        }*/
    }

}
