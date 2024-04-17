using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.Utility;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public class GridCellChange
    {
        public DataGridViewCell cell;
        public object OldValue;
        public object NewValue;
        public int RowIndex => cell.RowIndex;
        public int ColumnIndex => cell.ColumnIndex;

        public GridCellChange(DataGridView dataGridView, int column, int row) : this(dataGridView.Rows[row].Cells[column])
        {
        }

        public GridCellChange(DataGridViewCell cell)
        {
            this.cell = cell;
            OldValue = cell?.Value;
        }

        public void ReverseValues()
        {
            var savedOldValue = this.OldValue;
            OldValue = NewValue;
            NewValue = savedOldValue;
        }

        public void ApplyNewValue()
        {
            if (cell != null)
            {
                cell.Value = this.NewValue;
            }
        }

        public void ApplyOldValue()
        {
            if (cell != null)
            {
                cell.Value = this.OldValue;
            }
        }
    }

    public class GridCellChangeAssociator<T> where T : IGridData
    {
        private readonly DataGridView dataGridView;
        private readonly Dictionary<int, Func<T, object>> associationDict;
        public GridCellChangeAssociator(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            this.associationDict = new Dictionary<int, Func<T, object>>();
        }

        public void SetAssociation(int columnIndex, Func<T, object> func)
        {
            if (!associationDict.ContainsKey(columnIndex))
            {
                associationDict.Add(columnIndex, func);
            }
            else
            {
                associationDict[columnIndex] = func;
            }
        }

        public List<GridCellChange> CreateCellChanges(int rowIndex, T data)
        {
            return CreateCellChanges(rowIndex, Utils.SingletonCollection<T>(data));
        }

        public List<GridCellChange> CreateCellChanges(int rowIndex, ICollection<T> dataCollection)
        {
            var cellChangeList = new List<GridCellChange>();

            foreach (var data in dataCollection)
            {
                foreach (var entry in associationDict)
                {
                    var columnIndex = entry.Key;
                    var func = entry.Value;
                    cellChangeList.Add(new GridCellChange(dataGridView, columnIndex, rowIndex) { NewValue = func.Invoke(data) });
                }
            }

            return cellChangeList;
        }
    }
}
