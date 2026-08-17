using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridQOL(DataGridView dataGridView)
    {

        public void Scroll(int amount)
        {
            var rowCount = dataGridView.RowCount;
            var firstDisplayedRowIndex = dataGridView.FirstDisplayedScrollingRowIndex;

            var rowToShow = firstDisplayedRowIndex + amount;

            var displayedRowCount = dataGridView.DisplayedRowCount(false);
            rowToShow = Math.Max(rowToShow, 0);
            rowToShow = Math.Min(rowToShow, rowCount - displayedRowCount);

            dataGridView.FirstDisplayedScrollingRowIndex = rowToShow;
        }

        public void EventMouseWheel_UseWheelDuringSelection(int delta)
        {
            if ((Control.MouseButtons & MouseButtons.Left) != 0)
            {
                Scroll(-delta / 40);
            }
        }

        public void EventCellMouseDown_FullRowSelection(int rowIndex, int columnIndex)
        {
            if (columnIndex == -1 && rowIndex >= 0)
            {
                var currentRow = dataGridView.CurrentRow;
                if (Control.ModifierKeys == Keys.Shift && currentRow != null)
                {
                    var startRowIndex = currentRow.Index;
                    var endRowIndex = rowIndex;

                    dataGridView.ClearSelection();

                    var biggestIndex = Math.Max(startRowIndex, endRowIndex);
                    var lowestIndex = Math.Min(startRowIndex, endRowIndex);
                    for (int x = lowestIndex; x < biggestIndex + 1; x++)
                    {
                        foreach (DataGridViewCell cell in dataGridView.Rows[x].Cells)
                        {
                            cell.Selected = true;
                        }
                    }
                }
                else
                {
                    SelectRow(rowIndex);
                }
            }
        }

        public void EventCellClick_ImproveComboBox(int rowIndex, int columnIndex)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            if (cell == null)
            {
                return;
            }

            //This is to have a better user experience while dealing with combobox. When finishing the edit, it will revert back to a simple selected cell instead of selected text!
            if (cell is DataGridViewComboBoxCell comboBoxCell)
            {
                dataGridView.CurrentCell = cell;
                dataGridView.BeginEdit(false);
                if (dataGridView.EditingControl is DataGridViewComboBoxEditingControl comboBoxEditingControl)
                {
                    comboBoxEditingControl.DroppedDown = true;
                    comboBoxEditingControl.DropDownClosed += (sender, args) => dataGridView.EndEdit();
                }
            }
        }

        public void EventCellDoubleClick(int rowIndex, int columnIndex, MouseButtons button)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || button != MouseButtons.Left || rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            if (cell == null)
            {
                return;
            }

            if (cell is DataGridViewTextBoxCell textBoxCell && !textBoxCell.IsInEditMode)
            {
                dataGridView.BeginEdit(true);
            }
            else if (cell is DataGridViewCheckBoxCell checkboxCell)
            {
                checkboxCell.Value = (bool)(checkboxCell.Value ?? false) == false;
                dataGridView.FindForm()?.BeginInvoke(() =>
                { //Append a task. This refresh correctly the checkbox cell.
                    if (checkboxCell.DataGridView == dataGridView)
                    {
                        dataGridView.InvalidateCell(checkboxCell);
                    }
                });

            }
        }

        public void EventRowPostPaint_AddNumbers(Rectangle rowBounds, DataGridViewCellStyle inheritedRowStyle, Graphics graphics, int rowIndex)
        {
            var rowIdx = (rowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Far
            };

            var textSize = TextRenderer.MeasureText(rowIdx, inheritedRowStyle.Font); //get the size of the string
            dataGridView.RowHeadersWidth = Math.Max(dataGridView.RowHeadersWidth, textSize.Width + 15); //if header width lower then string width then resize

            var headerBounds = new Rectangle(rowBounds.Left, rowBounds.Top, dataGridView.RowHeadersWidth, rowBounds.Height);
            graphics.DrawString(rowIdx, inheritedRowStyle.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        public void SelectRow(int rowIndex)
        {
            dataGridView.ClearSelection();

            var row = dataGridView.Rows[rowIndex];
            if (row.Cells.Count > 0)
            {
                //I need to set the current cell, because i use the CurrentRow as a "starting row"
                //Do not cancel current cell! It might select the first cell in the grid and mess up selection.
                dataGridView.CurrentCell = row.Cells[0];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Selected = true;
                }
            }
        }
    }
}
