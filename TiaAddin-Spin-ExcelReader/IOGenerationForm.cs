
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
        public const int TOTAL_ROW_COUNT = 9999;

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly ExcelDragHandler excelDragHandler;

        public IOGenerationForm()
        {
            InitializeComponent();

            this.undoRedoHandler = new UndoRedoHandler();
            this.excelDragHandler = new ExcelDragHandler(this.dataGridView);
            Init();
        }

        public void Init()
        {
            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.RowCount = TOTAL_ROW_COUNT; //To make it easier to handle, provide a fixed amount of rows. I don't think these much rows will ever be needed!

            this.dataGridView.CellPainting += (sender, args) =>
            {
                var currentCell = dataGridView.CurrentCell;
                if (args.RowIndex < 0 || args.ColumnIndex < 0 || currentCell == null || excelDragHandler.IsStarted())
                {
                    return;
                }

                if (currentCell.RowIndex == args.RowIndex && currentCell.ColumnIndex == args.ColumnIndex)
                {
                    var bounds = args.CellBounds;
                    args.PaintBackground(bounds, true);
                    args.PaintContent(bounds);
                    using (var pen = new Pen(Color.Green, 4))
                    {
                        args.Graphics.DrawRectangle(pen, new Rectangle(bounds.Right - 4, bounds.Bottom - 4, 4, 4));
                    }

                    args.Handled = true;
                }
            };

            excelDragHandler.Init();

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
                    DeleteSelectedCells();

                if (ctrlZ)
                    undoRedoHandler.Undo();

                if (ctrlY)
                    undoRedoHandler.Redo();
            };
            #endregion

            #region MouseClick - RowSelection
            int lastClickedRowIndex = -1;
            this.dataGridView.MouseDown += (sender, args) =>
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
                var pastedCellList = new List<PastedCellValue>();

                var pasteString = clipboardDataObject.GetData(DataFormats.Text).ToString();
                if (pasteString.Contains("\r\n") || pasteString.Contains('\t'))
                {//If contains new lines or tab it needs to handled like an excel file. New line => next row. Tab => next column.
                    string[] pastedRows = Regex.Split(pasteString.TrimEnd("\r\n".ToCharArray()), "\r\n");

                    var currentCell = dataGridView.CurrentCell;
                    int startRowPointer = currentCell.RowIndex; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!

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
                }
                else
                {//If is a normal string, i will paste in ALL the selected cells!
                    foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
                    {
                        pastedCellList.Add(new PastedCellValue()
                        {
                            rowIndex = selectedCell.RowIndex,
                            columnIndex = selectedCell.ColumnIndex,
                            oldValue = selectedCell.Value,
                            newValue = pasteString
                        });
                    }
                }

                PasteNewCells(pastedCellList);
            }
        }

        public void DeleteSelectedCells()
        {
            List<PastedCellValue> deletedCellList = new List<PastedCellValue>();
            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                deletedCellList.Add(new PastedCellValue()
                {
                    rowIndex = selectedCell.RowIndex,
                    columnIndex = selectedCell.ColumnIndex,
                    oldValue = selectedCell.Value,
                    newValue = ""
                });
            }

            this.PasteNewCells(deletedCellList);
        }

        private void PasteNewCells(List<PastedCellValue> pastedCellList)
        {
            undoRedoHandler.Lock(); //Lock the handler. I do not want more actions been added by events here since are all handled below!
            foreach (var pastedCell in pastedCellList)
            {
                dataGridView.Rows[pastedCell.rowIndex].Cells[pastedCell.columnIndex].Value = pastedCell.newValue;
            }
            undoRedoHandler.Unlock();

            undoRedoHandler.AddUndo(() =>
            {
                foreach (var pastedCell in pastedCellList)
                {
                    dataGridView.Rows[pastedCell.rowIndex].Cells[pastedCell.columnIndex].Value = pastedCell.oldValue;
                }

                undoRedoHandler.AddRedo(() => PasteNewCells(pastedCellList));
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

    public class ExcelDragHandler
    {
        private readonly DataGridView dataGridView;

        private bool started = false;
        private int rowIndexStart = -1;
        private int columnIndex = -1;

        public ExcelDragHandler(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
        }

        public bool IsStarted()
        {
            return started;
        }

        public void Init()
        {
            dataGridView.MouseMove += (sender, args) =>
            {
                if (started)
                {
                    return;
                }

                dataGridView.Cursor = Cursors.Default;

                var currentCell = dataGridView.CurrentCell;
                if (currentCell == null)
                {
                    return;
                }

                var bounds = dataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                if (args.X >= bounds.Right - 4 && args.X <= bounds.Right && args.Y >= bounds.Bottom - 4 && args.Y <= bounds.Bottom)
                {
                    dataGridView.Cursor = Cursors.Cross;
                }
            };

            this.dataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    var bounds = dataGridView.GetCellDisplayRectangle(hitTest.ColumnIndex, hitTest.RowIndex, false);
                    if (args.X >= bounds.Right - 4 && args.X <= bounds.Right && args.Y >= bounds.Bottom - 4 && args.Y <= bounds.Bottom)
                    {
                        started = true;
                        rowIndexStart = hitTest.RowIndex;
                        columnIndex = hitTest.ColumnIndex;
                    }
                }
            };


            this.dataGridView.LostFocus += (sender, args) =>
            {
                started = false;
            };

            this.dataGridView.MouseUp += (sender, args) =>
            {
                if (started)
                {
                    started = false;

                    var highestRowIndex = -1;
                    var lowestRowIndex = -1;

                    for (int x = 0; x < dataGridView.SelectedCells.Count; x++)
                    {
                        var selectedCell = dataGridView.SelectedCells[x];
                        if (highestRowIndex == -1 || selectedCell.RowIndex > highestRowIndex)
                        {
                            highestRowIndex = selectedCell.RowIndex;
                        }

                        if (lowestRowIndex == -1 || selectedCell.RowIndex < lowestRowIndex)
                        {
                            lowestRowIndex = selectedCell.RowIndex;
                        }
                    }

                    var rowIndexEnd = highestRowIndex != rowIndexStart ? highestRowIndex : lowestRowIndex;

                    var rowIndexList = new List<int>();
                    bool draggingDown = (lowestRowIndex == rowIndexStart); //false = draggingUP

                    if (draggingDown)
                    {
                        draggingDown = true;
                        for (int rowIndex = rowIndexStart + 1; rowIndex <= rowIndexEnd; rowIndex++)
                        {
                            rowIndexList.Add(rowIndex);
                        }
                    }
                    else
                    {
                        for (int rowIndex = rowIndexStart - 1; rowIndex >= rowIndexEnd; rowIndex--)
                        {
                            rowIndexList.Add(rowIndex);
                        }
                    }

                    if (columnIndex == 0)
                    {
                        var cellValue = (dataGridView.Rows[rowIndexStart].Cells[0].Value ?? "").ToString();

                        var tagAddress = SimaticTagAddress.FromAddress(cellValue);
                        if (tagAddress != null) //If is not a valid address, i won't care about doing any stuff.
                        {
                            rowIndexList.ForEach(rowIndex =>
                            {
                                if (draggingDown)
                                {
                                    tagAddress.bitOffset += 1;
                                    if (tagAddress.bitOffset > 7)
                                    {
                                        tagAddress.bitOffset = 0;
                                        tagAddress.byteOffset += 1;
                                    }
                                }
                                else
                                {
                                    if (tagAddress.bitOffset == 0)
                                    {
                                        if (tagAddress.byteOffset == 0)
                                        {
                                            return;
                                        }

                                        tagAddress.bitOffset = 7;
                                        tagAddress.byteOffset -= 1;
                                    }
                                    else
                                    {
                                        tagAddress.bitOffset -= 1;
                                    }
                                }

                                dataGridView.Rows[rowIndex].Cells[0].Value = tagAddress.GetAddress();
                            });

                        }
                    }

                    dataGridView.ClearSelection();
                    dataGridView.CurrentCell = dataGridView.Rows[rowIndexEnd].Cells[columnIndex];
                    dataGridView.CurrentCell.Selected = true;
                }

            };


            dataGridView.SelectionChanged += (sender, args) =>
            {
                if (started)
                {
                    for (int x = 0; x < dataGridView.SelectedCells.Count; x++)
                    {
                        var selectedCell = dataGridView.SelectedCells[x];
                        if (selectedCell.ColumnIndex != columnIndex)
                        {
                            selectedCell.Selected = false;
                            continue;
                        }
                    }
                }
            };

        }
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