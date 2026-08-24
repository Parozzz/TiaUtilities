using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridViewManipulator(DataGridView dataGridView, GridDataChangedHandler dataChangedHandler)
    {
        public void AutoResizeColumns() => dataGridView.AutoResizeColumns();

        public void AutoResizeColumnHeadersHeight() => dataGridView.AutoResizeColumnHeadersHeight();


        public DataGridViewCell? GetCurrentCell() => dataGridView.CurrentCell;

        public void ChangeCurrentCell(int rowIndex, GridDataColumn column) => this.ChangeCurrentCell(rowIndex, column.ColumnIndex);

        public void ChangeCurrentCell(int rowIndex, int columnIndex)
        {
            if (GridUtils.AreCoordinatesValid(dataGridView, rowIndex, columnIndex))
            {
                dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            }
        }

        public void SetCell(int rowIndex, GridDataColumn column, object? value) => this.SetCell(rowIndex, column.ColumnIndex, value);

        public void SetCell(int rowIndex, int columnIndex, object? value) => dataGridView.Rows[rowIndex].Cells[columnIndex].Value = value;

        public DataGridViewCell? GetCell(int rowIndex, GridDataColumn column) => this.GetCell(rowIndex, column.ColumnIndex);

        public DataGridViewCell? GetCell(int rowIndex, int columnIndex)
        {
            return GridUtils.AreCoordinatesValid(dataGridView, rowIndex, columnIndex) ? dataGridView.Rows[rowIndex].Cells[columnIndex] : null;
        }

        public void RefreshCell(DataGridViewCell cell) => dataGridView.InvalidateCell(cell);

        public void RefreshCell(int columnIndex, int rowIndex) => dataGridView.InvalidateCell(columnIndex, rowIndex);

        public void Refresh()
        {
            dataGridView.RefreshEdit();
            dataGridView.Refresh();
        }

        public void RefreshRow(int rowIndex)
        {
            if (GridUtils.IsRowValid(dataGridView, rowIndex))
            {
                var row = dataGridView.Rows[rowIndex];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    this.RefreshCell(cell);
                }
            }
        }

        public void DeleteSelectedCells()
        {
            var req = dataChangedHandler.Join();

            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                selectedCell.Value = null; //Set value to null so it will clear also checkboxes
            }

            dataChangedHandler.End(req);
        }


        public void SuspendLayout() => dataGridView.SuspendLayout();

        public void ResumeLayout(bool refresh = false, bool performLayout = true)
        {
            if (refresh)
            {
                this.Refresh();
            }

            dataGridView.ResumeLayout(performLayout);
        }

    }
}
