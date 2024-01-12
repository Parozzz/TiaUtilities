
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;
using TiaXmlReader.UndoRedo;

namespace TiaXmlReader
{
    public partial class IOGenerationForm : Form
    {
        private readonly UndoRedoHandler undoRedoHandler;

        public IOGenerationForm()
        {
            this.undoRedoHandler = new UndoRedoHandler();

            InitializeComponent();

            Init();
        }

        public void Init()
        {
            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            #region Sorting
            this.dataGridView.SortCompare += (sender, args) =>
            {
                if (args.Column.Index == 0)
                {
                    var cell1SortValue = SimaticTagAddress.FromAddress(args.CellValue1.ToString()).GetSortingNumber();
                    var cell2SortValue = SimaticTagAddress.FromAddress(args.CellValue2.ToString()).GetSortingNumber();

                    args.Handled = true;
                    args.SortResult = cell1SortValue.CompareTo(cell2SortValue);
                }
            };
            #endregion

            #region RowHeaderNumber
            this.dataGridView.RowPostPaint += (sender, args) =>
            {
                var grid = sender as DataGridView;
                var rowIdx = (args.RowIndex + 1).ToString();

                var centerFormat = new StringFormat()
                {
                    // right alignment might actually make more sense for numbers
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                };

                Size textSize = TextRenderer.MeasureText(rowIdx, this.Font); //get the size of the string
                grid.RowHeadersWidth = Math.Max(grid.RowHeadersWidth, textSize.Width + 15); //if header width lower then string width then resize

                var headerBounds = new Rectangle(args.RowBounds.Left, args.RowBounds.Top, grid.RowHeadersWidth, args.RowBounds.Height);
                args.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            };
            #endregion

            #region KeyDown - Paste/Delete
            this.dataGridView.KeyDown += (sender, args) =>
            {
                bool ctrlZ = args.Modifiers == Keys.Control && args.KeyCode == Keys.Z;
                bool ctrlY = args.Modifiers == Keys.Control && args.KeyCode == Keys.Y;
                bool ctrlV = args.Modifiers == Keys.Control && args.KeyCode == Keys.V;
                bool shiftIns = args.Modifiers == Keys.Shift && args.KeyCode == Keys.Insert;

                if (ctrlV || shiftIns)
                    Paste();

                if (args.KeyCode == Keys.Delete || args.KeyCode == Keys.Cancel)
                    DeleteSelectedRows();

                if (ctrlZ)
                    undoRedoHandler.Undo();

                if (ctrlY)
                    undoRedoHandler.Redo();
            };
            #endregion

            #region MouseClick - RowSelection
            int lastClickedRowIndex = -1;
            this.dataGridView.MouseClick += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type != DataGridViewHitTestType.RowHeader)
                {
                    lastClickedRowIndex = -1;
                }

                switch (hitTest.Type)
                {
                    case DataGridViewHitTestType.None: //I want that to clear the selection, you do a simple click in an empty area!
                        dataGridView.ClearSelection();
                        dataGridView.CurrentCell = null; //This avoid the situation where if you click the old cell again, it start editing immediately! 
                        break;
                    case DataGridViewHitTestType.RowHeader: //If i click a row head, i want the whole row to be selected!
                        if (Control.ModifierKeys == Keys.Shift && dataGridView.CurrentRow != null)
                        {
                            var biggestIndex = Math.Max(lastClickedRowIndex, hitTest.RowIndex);
                            var lowestIndex = Math.Min(lastClickedRowIndex, hitTest.RowIndex);
                            for (int x = lowestIndex + 1; x < biggestIndex + 1; x++)
                            {
                                if (x == dataGridView.CurrentRow.Index)
                                {
                                    continue;
                                }

                                foreach (DataGridViewCell cell in dataGridView.Rows[x].Cells)
                                {
                                    cell.Selected = !cell.Selected;
                                }
                            }

                            lastClickedRowIndex = hitTest.RowIndex;
                        }
                        else
                        {
                            dataGridView.ClearSelection();
                            dataGridView.CurrentCell = dataGridView.Rows[hitTest.RowIndex].Cells[0]; //I need to set the current cell, because i use the CurrentRow as a "starting row"
                            //Do not cancel current cell! It might select the first cell in the grid and mess up selection.

                            lastClickedRowIndex = hitTest.RowIndex;
                            foreach (DataGridViewCell cell in dataGridView.Rows[hitTest.RowIndex].Cells)
                            {
                                cell.Selected = true;
                            }
                        }

                        break;
                    case DataGridViewHitTestType.Cell:
                        if (args.Button == MouseButtons.Right && hitTest.ColumnIndex == 0)
                        {
                            var dropDown = new ToolStripDropDown();

                            var addByteButton = new ToolStripButton
                            {
                                ForeColor = Color.Black,
                                Text = "Add byte"
                            };
                            addByteButton.Click += (sender1, e1) =>
                            {
                                var cellValue = (dataGridView.Rows[hitTest.RowIndex].Cells[0].Value ?? "").ToString();

                                var tagAddress = SimaticTagAddress.FromAddress(cellValue) ?? new SimaticTagAddress()
                                {
                                    memoryArea = SimaticMemoryArea.INPUT,
                                    bitOffset = 0,
                                    byteOffset = 0,
                                    length = 0
                                };

                                tagAddress.byteOffset += 1;

                                var row = hitTest.RowIndex + 1;
                                bool addReq = row == dataGridView.Rows.Count;
                                for (int x = 0; x < 8; x++)
                                {
                                    tagAddress.bitOffset = (uint)x;

                                    if (addReq)
                                    {
                                        dataGridView.Rows.Add(new string[] { tagAddress.GetAddress() });
                                    }
                                    else
                                    {
                                        dataGridView.Rows.Insert(row + x, new string[] { tagAddress.GetAddress() });
                                    }

                                }
                            };
                            dropDown.Items.AddRange(new ToolStripItem[] { addByteButton });

                            dropDown.Show(new Point(Cursor.Position.X, Cursor.Position.Y));
                        }
                        break;
                }
            };
            #endregion

            var cellEditList = new List<PastedCellValue>();
            dataGridView.CellBeginEdit += (sender, args) =>
            {
                cellEditList.Add(new PastedCellValue()
                {
                    rowIndex = args.RowIndex,
                    columnIndex = args.ColumnIndex,
                    oldValue = dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Value
                });
            };

            dataGridView.CellEndEdit += (sender, args) =>
            {
                PastedCellValue found = null;
                foreach (var cellEditValue in cellEditList)
                {
                    var rowIndex = args.RowIndex;
                    var columnIndex = args.ColumnIndex;
                    if (cellEditValue.rowIndex == rowIndex && cellEditValue.columnIndex == columnIndex)
                    {
                        cellEditValue.newValue = dataGridView.Rows[rowIndex].Cells[columnIndex].Value;
                        AddUndoCell(cellEditValue);

                        found = cellEditValue;
                    }
                }

                if (found != null)
                    cellEditList.Remove(found);
            };

            dataGridView.RowsAdded += (sender, args) =>
            {
                var rowIndex = args.RowIndex;
                var rowCount = args.RowCount;
                undoRedoHandler.AddUndo(() =>
                {
                    for (int x = 0; x < rowCount; x++)
                    {
                        dataGridView.Rows.RemoveAt(rowIndex - 1); //I will remove the row previous to the added one! Since is the one that triggered the rows added
                    }

                    undoRedoHandler.AddRedo(() =>
                    {
                        for (int x = 0; x < rowCount; x++)
                        {
                            dataGridView.Rows.AddCopies(rowIndex, rowCount);
                        }
                    });
                });

            };
        }

        private void AddUndoCell(PastedCellValue cell)
        {
            undoRedoHandler.AddUndo(() =>
            {
                dataGridView.CurrentCell = dataGridView.Rows[cell.rowIndex].Cells[cell.columnIndex];
                dataGridView.CurrentCell.Value = cell.oldValue;

                undoRedoHandler.AddRedo(() =>
                {
                    dataGridView.CurrentCell = dataGridView.Rows[cell.rowIndex].Cells[cell.columnIndex];
                    dataGridView.CurrentCell.Value = cell.newValue;

                    AddUndoCell(cell);
                });
            });

        }

        public void Paste()
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                string[] pastedRows = Regex.Split(clipboardDataObject.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");

                var currentCell = dataGridView.CurrentCell;
                int startRowPointer = currentCell.RowIndex; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!

                int rowsToAdd = 0;
                if (dataGridView.Rows.Count < (currentCell.RowIndex + pastedRows.Length))
                {
                    rowsToAdd = currentCell.RowIndex + pastedRows.Length - dataGridView.Rows.Count + 1;  //Adding one extra to avoid remaining without rows!
                }

                List<PastedCellValue> pastedCellList = new List<PastedCellValue>();

                var rowPointer = startRowPointer;
                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedColumns = pastedRow.Split('\t');
                    for (int pastedColumnPointer = 0, columnPointer = currentCell.ColumnIndex; columnPointer < dataGridView.ColumnCount && pastedColumnPointer < pastedColumns.Length; columnPointer++, pastedColumnPointer++)
                    {
                        var cell = dataGridView.Rows.Count > rowPointer ? dataGridView.Rows[rowPointer].Cells[columnPointer] : null;
                        pastedCellList.Add(new PastedCellValue()
                        {
                            rowIndex = rowPointer,
                            columnIndex = columnPointer,
                            oldValue = cell?.Value,
                            newValue = pastedColumns[pastedColumnPointer]
                        });
                    }
                    rowPointer++;
                }

                PasteNewCells(pastedCellList, rowsToAdd);
            }
        }

        private void PasteNewCells(List<PastedCellValue> pastedCellList, int rowsToAdd)
        {
            undoRedoHandler.Lock(); //Lock the handler. I do not work actions here since are all handled below!
            if (rowsToAdd > 0)
            {
                dataGridView.Rows.Add(rowsToAdd);
            }

            foreach (var cell in pastedCellList)
            {
                dataGridView.Rows[cell.rowIndex].Cells[cell.columnIndex].Value = cell.newValue;
            }
            undoRedoHandler.Unlock();

            undoRedoHandler.AddUndo(() =>
            {
                foreach (var cell in pastedCellList)
                {
                    dataGridView.Rows[cell.rowIndex].Cells[cell.columnIndex].Value = cell.oldValue;
                }

                bool hasNewRow = dataGridView.Rows[dataGridView.Rows.Count - 1].IsNewRow;
                for (int x = 0; x < rowsToAdd; x++)
                {
                    var row = dataGridView.Rows[dataGridView.Rows.Count - (hasNewRow ? 2 : 1)]; //Always remove the last one (If is not a new row. It cannot be deleted)! While removing the count will decrease.
                    dataGridView.Rows.Remove(row);
                }

                undoRedoHandler.AddRedo(() => PasteNewCells(pastedCellList, rowsToAdd));
            });
        }

        public void DeleteSelectedRows()
        {
            List<int> deletedRowList = new List<int>();
            List<PastedCellValue> deletedCellList = new List<PastedCellValue>();

            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                if (selectedCell.RowIndex == -1) //If there is no RowIndex, the cell has already been deleted!
                {
                    continue;
                }

                bool allSelected = true;
                foreach (DataGridViewCell cell in dataGridView.Rows[selectedCell.RowIndex].Cells)
                {
                    allSelected &= cell.Selected;
                }


                if (allSelected)
                {
                    if (!deletedRowList.Contains(selectedCell.RowIndex))
                    {
                        deletedRowList.Add(selectedCell.RowIndex);
                    }
                }
                else
                {
                    deletedCellList.Add(new PastedCellValue()
                    {
                        rowIndex = selectedCell.RowIndex,
                        columnIndex = selectedCell.ColumnIndex,
                        oldValue = selectedCell.Value,
                    });
                }
            }

            undoRedoHandler.Lock();
            foreach(var rowIndex in deletedRowList)
            {
                var row = dataGridView.Rows[rowIndex];
                if(!row.IsNewRow)
                {
                    dataGridView.Rows.Remove(row);
                }
            }

            foreach(var cellValue in deletedCellList)
            {
                dataGridView.Rows[cellValue.rowIndex].Cells[cellValue.columnIndex].Value = "";
            }
            undoRedoHandler.Unlock();

            undoRedoHandler.AddUndo(() =>
            {

            });
        }
    }

    public class PastedCellValue
    {
        public int rowIndex;
        public int columnIndex;
        public object oldValue;
        public object newValue;
    }
}

/*
dataGridView.MouseDoubleClick += (sender, args) =>
{
    var hitTest = dataGridView.HitTest(args.X, args.Y);
    if (hitTest.Type == DataGridViewHitTestType.Cell)
    {
        var cell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
        dataGridView.CurrentCell = cell;
        dataGridView.BeginEdit(true);
    }
};

dataGridView.CellPainting += (sender, args) =>
{
    if (args.RowIndex >= 0 && args.ColumnIndex >= 0)
    {
        if (dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Selected == true)
        {
            args.Paint(args.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
            using (Pen p = new Pen(Color.Red, 3))
            {
                Rectangle rect = args.CellBounds;
                rect.Width -= 2;
                rect.Height -= 2;
                args.Graphics.DrawRectangle(p, rect);
            }
            args.Handled = true;
        }
    }

    e.PaintBackground(e.CellBounds, true);  
    e.PaintContent(e.CellBounds);  
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))  
    {  
        Point[] pt = new Point[] { new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 10), new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1), new Point(e.CellBounds.Right - 10, e.CellBounds.Bottom - 1) };  
        e.Graphics.FillPolygon(brush, pt);  
    }  
    e.Handled = true;  
};
*/