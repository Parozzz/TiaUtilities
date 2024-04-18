using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.UndoRedo;
using static TiaXmlReader.GenerationForms.GridHandler.GridExcelDragHandler;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.GenerationForms.IO;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public class GridHandler<T> where T : IGridData
    {
        private readonly DataGridView dataGridView;
        private readonly GridSettings settings;
        public GridDataSource<T> DataSource { get; private set; }
        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridExcelDragHandler excelDragHandler;
        private readonly GridSortHandler<T> sortHandler;
        private readonly GridCellChangeAssociator<T> associator;
       

        private readonly List<IGridCellPainter> cellPainterList;

        public uint RowCount { get; set; } = 1999;
        public bool AddRowIndexToRowHeader { get; set; } = true;
        public bool EnablePasteFromExcel { get; set; } = true;
        public bool EnableRowSelectionFromRowHeaderClick { get; set; } = true;


        public GridHandler(DataGridView dataGridView, GridSettings settings, Func<T> newObjectFunction, Action<T, T> trasferDataAction, IGridRowComparer<T> comparer = null)
        {
            this.dataGridView = dataGridView;

            this.settings = settings;
            this.DataSource = new GridDataSource<T>(this.dataGridView, newObjectFunction, trasferDataAction);
            this.undoRedoHandler = new UndoRedoHandler();
            this.excelDragHandler = new GridExcelDragHandler(this.dataGridView, settings);
            this.sortHandler = new GridSortHandler<T>(this.dataGridView, this.DataSource, this.undoRedoHandler, comparer);
            this.associator = new GridCellChangeAssociator<T>(this.dataGridView);

            this.cellPainterList = new List<IGridCellPainter>();
        }

        public void AddCellPainter(IGridCellPainter cellPainter)
        {
            cellPainterList.Add(cellPainter);
        }

        public void SetDragPreviewAction(Action<DragData> action)
        {
            this.excelDragHandler.SetPreviewAction(action);
        }

        public void SetDragMouseUpAction(Action<DragData> action)
        {
            this.excelDragHandler.SetMouseUpAction(action);
        }

        public void SetDataAssociation(int columnIndex, Func<T, object> func)
        {
            associator.SetAssociation(columnIndex, func);
        }

        public void Init()
        {
            this.DataSource.InitializeData(this.RowCount);

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.dataGridView, new object[] { true });

            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.dataGridView.AllowUserToAddRows = false;

            #region Cell Paiting
            var paintHandler = new GridCellPaintHandler(this.dataGridView);
            paintHandler.AddPainter(this.sortHandler); //ORDER IS IMPORTANT!
            paintHandler.AddPainter(this.excelDragHandler);
            paintHandler.AddPainterRange(cellPainterList);
            paintHandler.Init();
            #endregion

            #region RowHeaderNumber
            if (AddRowIndexToRowHeader)
            {
                this.dataGridView.RowPostPaint += (sender, args) =>
                {
                    var style = args.InheritedRowStyle;

                    var rowIdx = (args.RowIndex + 1).ToString();

                    var centerFormat = new StringFormat()
                    {
                        // right alignment might actually make more sense for numbers
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Far
                    };

                    var textSize = TextRenderer.MeasureText(rowIdx, style.Font); //get the size of the string
                    dataGridView.RowHeadersWidth = Math.Max(dataGridView.RowHeadersWidth, textSize.Width + 15); //if header width lower then string width then resize

                    var headerBounds = new Rectangle(args.RowBounds.Left, args.RowBounds.Top, dataGridView.RowHeadersWidth, args.RowBounds.Height);
                    args.Graphics.DrawString(rowIdx, style.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
                };
            }
            #endregion

            #region KeyDown - Paste/Delete/Undo/Redo
            this.dataGridView.KeyDown += (sender, args) =>
            {
                bool ctrlZ = args.Modifiers == Keys.Control && args.KeyCode == Keys.Z;
                bool ctrlY = args.Modifiers == Keys.Control && args.KeyCode == Keys.Y;
                bool ctrlV = args.Modifiers == Keys.Control && args.KeyCode == Keys.V;
                bool shiftIns = args.Modifiers == Keys.Shift && args.KeyCode == Keys.Insert;

                if (EnablePasteFromExcel && (ctrlV || shiftIns))
                {
                    PasteFromExcel();
                }

                if (args.KeyCode == Keys.Delete || args.KeyCode == Keys.Cancel)
                {
                    DeleteSelectedCells();
                }

                if (ctrlZ)
                {
                    undoRedoHandler.Undo();
                }

                if (ctrlY)
                {
                    undoRedoHandler.Redo();
                }
            };
            #endregion

            #region MouseClick - RowSelection
            if (EnableRowSelectionFromRowHeaderClick)
            {
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
            }
            #endregion

            #region CellEdit Begin-End
            GridCellChange cellEdit = null;
            dataGridView.CellBeginEdit += (sender, args) =>
            {
                cellEdit = new GridCellChange(dataGridView, args.ColumnIndex, args.RowIndex);
            };

            dataGridView.CellEndEdit += (sender, args) =>
            {
                if (cellEdit.cell == null)
                {
                    return;
                }

                if (cellEdit.RowIndex == args.RowIndex && cellEdit.ColumnIndex == args.ColumnIndex)
                {
                    cellEdit.NewValue = dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Value;

                    ChangeCells(new List<GridCellChange> { cellEdit }, applyChanges: false);

                    cellEdit = null;
                }
            };
            #endregion

            excelDragHandler.Init();
            sortHandler.Init();
        }

        public DataGridViewColumn InitColumn(int columnIndex, string name, int width)
        {
            var column = this.dataGridView.Columns[columnIndex];
            column.Name = name;
            column.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            column.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
            column.DefaultCellStyle.SelectionForeColor = Color.Black;
            column.DefaultCellStyle.ForeColor = Color.Black;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
            column.AutoSizeMode = width <= 0 ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            return column;
        }

        private void PasteFromExcel()
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                var pastedCellList = new List<GridCellChange>();

                var pasteString = clipboardDataObject.GetData(DataFormats.Text).ToString();
                if (pasteString.Contains("\r\n") || pasteString.Contains('\t'))
                {//If contains new lines or tab it needs to handled like an excel file. New line => next row. Tab => next column.
                    string[] pastedRows = Regex.Split(pasteString.TrimEnd("\r\n".ToCharArray()), "\r\n");

                    int startRowPointer = dataGridView.CurrentCell.RowIndex; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!
                    int startColumnPointer = dataGridView.CurrentCell.ColumnIndex;

                    int rowPointer = startRowPointer;
                    for (int pastedRowIndex = startRowPointer; pastedRowIndex < pastedRows.Length; rowPointer++, pastedRowIndex++)
                    {
                        if (rowPointer >= dataGridView.RowCount)
                        {
                            break;
                        }

                        string[] pastedColumns = pastedRows[pastedRowIndex].Split('\t');

                        int columnPointer = startColumnPointer;
                        for (int pastedColumnPointer = 0; pastedColumnPointer < pastedColumns.Length; columnPointer++, pastedColumnPointer++)
                        {
                            if (columnPointer >= dataGridView.ColumnCount)
                            {
                                break;
                            }

                            var cell = dataGridView.Rows[rowPointer]?.Cells[columnPointer];
                            if (cell != null)
                            {
                                pastedCellList.Add(new GridCellChange(cell) { NewValue = pastedColumns[pastedColumnPointer] });
                            }
                        }
                    }
                }
                else
                {//If is a normal string, i will paste in ALL the selected cells!
                    foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
                    {
                        pastedCellList.Add(new GridCellChange(selectedCell) { NewValue = pasteString });
                    }
                }

                ChangeCells(pastedCellList);
            }
        }

        public void DeleteSelectedCells()
        {
            var deletedCellList = new List<GridCellChange>();
            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                deletedCellList.Add(new GridCellChange(selectedCell) { NewValue = "" });
            }

            this.ChangeCells(deletedCellList);
        }

        public void ChangeRow(int rowIndex, T data)
        {
            this.ChangeCells(this.associator.CreateCellChanges(rowIndex, data));
        }

        public void ChangeRow(int rowIndex, ICollection<T> dataCollection)
        {
            this.ChangeCells(this.associator.CreateCellChanges(rowIndex, dataCollection));
        }

        public void ChangeCells(List<GridCellChange> cellChangeList, bool applyChanges = true)
        {
            if (cellChangeList.Count == 0)
            {
                return;
            }

            try
            {
                if (applyChanges)
                {
                    undoRedoHandler.Lock(); //Lock the handler. I do not want more actions been added by events here since are all handled below!

                    cellChangeList.ForEach(cellChange => cellChange.ApplyNewValue());
                    dataGridView.Refresh();

                    undoRedoHandler.Unlock();
                }

                void undoRedoAction()
                {
                    undoRedoHandler.Lock();
                    cellChangeList.ForEach(cellChange => cellChange.ApplyOldValue());
                    undoRedoHandler.Unlock();

                    dataGridView.ClearSelection();

                    var firstCell = cellChangeList[0].cell;
                    dataGridView.CurrentCell = firstCell; //Setting se current cell already center the grid to it.
                    dataGridView.Refresh();

                    undoRedoHandler.AddRedo(() => ChangeCells(cellChangeList));
                }

                undoRedoHandler.AddUndo(undoRedoAction);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error while changing cells", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
