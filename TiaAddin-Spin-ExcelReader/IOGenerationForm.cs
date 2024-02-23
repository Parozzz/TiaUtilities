
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;
using TiaXmlReader.UndoRedo;

namespace TiaXmlReader
{
    public class IOGenerationData
    {
        public string Address { get => address; set => address = value; }
        private string address;

        public string IOName { get => ioName; set => ioName = value; }
        private string ioName;

        public string DBName { get => dbName; set => dbName = value; }
        private string dbName;

        public string Variable { get => variable; set => variable = value; }
        private string variable;

        public string Comment { get => comment; set => comment = value; }
        private string comment;
        public SimaticTagAddress TagAddress { get => SimaticTagAddress.FromAddress(address); }

        public IOGenerationData()
        {

        }

        public IOGenerationData Clone()
        {
            return new IOGenerationData()
            {
                Address = Address,
                IOName = IOName,
                DBName = DBName,
                Variable = Variable,
                Comment = Comment
            };
        }
    }

    public partial class IOGenerationForm : Form
    {
        public const int TOTAL_ROW_COUNT = 1999;
        public static readonly Color SORT_ICON_COLOR = Color.Green;
        public static readonly Color SELECTED_CELL_COLOR = Color.DarkGreen;

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly ExcelDragHandler excelDragHandler;

        private readonly List<IOGenerationData> dataList;
        private readonly BindingList<IOGenerationData> bindingList;
        private readonly BindingSource bindingSource;

        private List<DataGridViewRow> oldRowPosition;

        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<IOGenerationData, int> noSortSnap;

        public IOGenerationForm()
        {
            InitializeComponent();

            dataList = new List<IOGenerationData>();
            for (int i = 0; i < TOTAL_ROW_COUNT; i++)
            {
                dataList.Add(new IOGenerationData());
            }

            bindingList = new BindingList<IOGenerationData>(dataList);
            bindingSource = new BindingSource() { DataSource = bindingList };

            this.undoRedoHandler = new UndoRedoHandler();
            this.excelDragHandler = new ExcelDragHandler(this, this.dataGridView);
            Init();
        }

        public void Init()
        {
            //var addressColumn = this.dataTable.Rows.Add(TOTAL_ROW_COUNT);

            this.dataGridView.DataSource = bindingSource;

            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.dataGridView.AllowUserToAddRows = false;

            var addressColumn = (DataGridViewTextBoxColumn)InitColumn(dataGridView.Columns[0], "Indirizzo", 60, false);
            addressColumn.MaxInputLength = 10;

            var ioNameColumn = InitColumn(dataGridView.Columns[1], "Nome IO", 75, false);
            var dbNameColumn = InitColumn(dataGridView.Columns[2], "DB", 75, false);
            var variableColumn = InitColumn(dataGridView.Columns[3], "Variabile", 75, false);
            var commentColumn = InitColumn(dataGridView.Columns[4], "Commento", 0, true);
            var tagAddressColumn = InitColumn(dataGridView.Columns[5], "Tag", 40, false);

            //this.dataGridView.RowCount = TOTAL_ROW_COUNT;

            #region Cell Paiting
            this.dataGridView.CellPainting += (sender, args) =>
            {
                var bounds = args.CellBounds;
                var graphics = args.Graphics;

                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;

                var style = args.CellStyle;

                if (rowIndex == -1)
                {
                    if (columnIndex == 0)
                    {
                        args.PaintBackground(bounds, false);

                        TextRenderer.DrawText(graphics, string.Format("{0}", args.FormattedValue), style.Font, bounds, style.ForeColor,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                        var column = dataGridView.Columns[0];
                        if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                        {
                            var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                            TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, SORT_ICON_COLOR,
                                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                        }

                        args.Handled = true;
                    }
                }
                else
                {
                    args.PaintBackground(bounds, true);
                    args.PaintContent(bounds);
                    args.Handled = true;

                    if (excelDragHandler.IsStarted())
                    {
                        return;
                    }

                    var currentCell = dataGridView.CurrentCell;
                    if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
                    {//I only want to apply the effect when the only selected cell is the current cell.
                        args.Paint(bounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                        using (Pen p = new Pen(SELECTED_CELL_COLOR, 2))
                        {//Border
                            Rectangle rect = args.CellBounds;
                            rect.Width -= 1;
                            rect.Height -= 1;
                            graphics.DrawRectangle(p, rect);
                        }

                        using (SolidBrush brush = new SolidBrush(SELECTED_CELL_COLOR))
                        {//Little triangle in the lower part only for current cell
                            var point1 = new Point(bounds.Right - 1, bounds.Bottom - 10);
                            var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                            var point3 = new Point(bounds.Right - 10, bounds.Bottom - 1);

                            Point[] pt = new Point[] { point1, point2, point3 };
                            graphics.FillPolygon(brush, pt);
                        }
                    }
                }


            };

            #endregion

            #region Sorting
            this.dataGridView.ColumnHeaderMouseClick += (sender, args) =>
            {
                if (args.ColumnIndex == 0)
                {
                    NextAddressColumnSort();
                }
            };
            #endregion

            #region RowHeaderNumber
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

            #region CellEdit Begin-End
            CellChange cellEdit = null;
            dataGridView.CellBeginEdit += (sender, args) =>
            {
                cellEdit = new CellChange(dataGridView, args.ColumnIndex, args.RowIndex);
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
                    AddUndoCell(cellEdit);

                    cellEdit = null;
                }
            };
            #endregion

            excelDragHandler.Init();
        }

        private void NextAddressColumnSort()
        {
            var column = dataGridView.Columns[0];

            var oldSortOrder = sortOrder;
            switch (sortOrder)
            {
                case SortOrder.None:
                    sortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    sortOrder = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                    sortOrder = SortOrder.None;
                    break;
            }

            AddressColumnSort(this.sortOrder, oldSortOrder);
        }

        private void AddressColumnSort(SortOrder sortOrder, SortOrder oldSortOrder)
        {
            var snapDict = this.CreateDataListSnapshot();

            if (sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortSnap = snapDict;
            }

            dataGridView.Columns[0].HeaderCell.SortGlyphDirection = sortOrder;
            if (sortOrder == SortOrder.None)
            {
                if (noSortSnap != null)
                {
                    this.RestoreDataListSnapshot(noSortSnap);
                    noSortSnap = null;
                }
            }
            else
            {
                dataList.Sort(new AddressColumnComparer(sortOrder));
                if (sortOrder == SortOrder.Descending)
                {
                    dataList.Reverse();
                }
            }
            dataGridView.Refresh();

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                dataGridView.Columns[0].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = CreateDataListSnapshot();
                RestoreDataListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => RestoreDataListSnapshot(undoSnap));
            });
        }

        private Dictionary<IOGenerationData, int> CreateDataListSnapshot()
        {
            var dict = new Dictionary<IOGenerationData, int>();
            for (int x = 0; x < dataList.Count; x++)
            {
                dict.Add(dataList[x], x);
            }
            return dict;
        }

        private void RestoreDataListSnapshot(Dictionary<IOGenerationData, int> dict)
        {
            dataList.Sort((x, y) =>
            {
                var indexX = dict[x];
                var indexY = dict[y];

                return indexX.CompareTo(indexY);
            });
            dataGridView.Refresh();
        }

        private DataGridViewColumn CreateColumn(string name, int width, bool fill)
        {
            var column = new DataGridViewTextBoxColumn();
            return InitColumn(column, name, width, fill);
        }

        private T InitColumn<T>(T column, string name, int width, bool fill) where T : DataGridViewColumn
        {
            column.Name = name;
            column.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            column.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
            column.DefaultCellStyle.SelectionForeColor = Color.Black;
            column.DefaultCellStyle.ForeColor = Color.Black;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = 60;
            column.AutoSizeMode = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            return column;
        }

        private void AddUndoCell(CellChange cell)
        {
            undoRedoHandler.AddUndo(() =>
            {
                dataGridView.CurrentCell = dataGridView.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
                dataGridView.CurrentCell.Value = cell.OldValue;

                undoRedoHandler.AddRedo(() =>
                {
                    dataGridView.CurrentCell = dataGridView.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
                    dataGridView.CurrentCell.Value = cell.NewValue;

                    AddUndoCell(cell);
                });
            });

        }

        public void Paste()
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                var pastedCellList = new List<CellChange>();

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
                            if (cell != null)
                            {
                                pastedCellList.Add(new CellChange(cell) { NewValue = pastedColumns[pastedColumnPointer] });
                            }

                        }
                        rowPointer++;
                    }
                }
                else
                {//If is a normal string, i will paste in ALL the selected cells!
                    foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
                    {
                        pastedCellList.Add(new CellChange(selectedCell) { NewValue = pasteString });
                    }
                }

                ChangeCells(pastedCellList);
            }
        }

        public void DeleteSelectedCells()
        {
            List<CellChange> deletedCellList = new List<CellChange>();
            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                deletedCellList.Add(new CellChange(selectedCell) { NewValue = "" });
            }

            this.ChangeCells(deletedCellList);
        }

        public void ChangeCells(List<CellChange> cellChangeList)
        {
            undoRedoHandler.Lock(); //Lock the handler. I do not want more actions been added by events here since are all handled below!
            foreach (var cellChange in cellChangeList)
            {
                dataGridView.Rows[cellChange.RowIndex].Cells[cellChange.ColumnIndex].Value = cellChange.NewValue;
            }
            dataGridView.Refresh(); //This is required to refresh data inside the Binding.
            undoRedoHandler.Unlock();

            undoRedoHandler.AddUndo(() =>
            {
                undoRedoHandler.Lock();
                foreach (var cellChange in cellChangeList)
                {
                    dataGridView.Rows[cellChange.RowIndex].Cells[cellChange.ColumnIndex].Value = cellChange.OldValue;
                }
                dataGridView.Refresh(); //This is required to refresh data inside the Binding.
                undoRedoHandler.Unlock();

                dataGridView.ClearSelection();
                dataGridView.CurrentCell = cellChangeList[0].cell;
                if (!cellChangeList[0].cell.Visible)
                {
                    dataGridView.FirstDisplayedCell = cellChangeList[0].cell;
                }

                undoRedoHandler.AddRedo(() => ChangeCells(cellChangeList));
            });
        }
    }

    public class CellChange
    {
        public DataGridViewCell cell;
        public object OldValue;
        public object NewValue;
        public int RowIndex { get { return cell.RowIndex; } }
        public int ColumnIndex { get { return cell.ColumnIndex; } }

        private readonly DataGridView dataGridView;

        public CellChange(DataGridViewCell cell)
        {
            this.cell = cell;
            if (cell != null)
            {
                OldValue = cell.Value;
            }
        }

        public CellChange(DataGridView dataGridView, int column, int row)
        {
            cell = dataGridView.Rows[row].Cells[column];
            if (cell != null)
            {
                OldValue = cell.Value;
            }
        }

    }

    public class ExcelDragHandler
    {
        private readonly IOGenerationForm form;
        private readonly DataGridView dataGridView;

        private bool started = false;
        private int rowIndexStart = -1;
        private int columnIndex = -1;

        public ExcelDragHandler(IOGenerationForm form, DataGridView dataGridView)
        {
            this.form = form;
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

                if (IsInsideTriangle(args.X, args.Y, currentCell.ColumnIndex, currentCell.RowIndex))
                {
                    dataGridView.Cursor = Cursors.Cross;
                }
            };

            this.dataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    if (IsInsideTriangle(args.X, args.Y, hitTest.ColumnIndex, hitTest.RowIndex))
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
                            var dragCellList = new List<CellChange>();
                            rowIndexList.ForEach(rowIndex =>
                            {
                                tagAddress = draggingDown
                                                    ? tagAddress.GetNextBit(SimaticDataType.BYTE)
                                                    : tagAddress.GetPreviousBit(SimaticDataType.BYTE);
                                dragCellList.Add(new CellChange(dataGridView, 0, rowIndex) { NewValue = tagAddress.GetAddress() });
                            });

                            form.ChangeCells(dragCellList);
                        }
                    }

                    dataGridView.ClearSelection();
                    dataGridView.CurrentCell = dataGridView.Rows[rowIndexEnd].Cells[columnIndex];
                    dataGridView.CurrentCell.Selected = true;
                    dataGridView.Refresh();
                }

            };

            dataGridView.SelectionChanged += (sender, args) =>
            {
                if (started)
                {//When dragging, only allow cell on the column to be selected! I don't care about other ways and want it simple.
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

        public bool IsInsideTriangle(int x, int y, int columnIndex, int rowIndex)
        {
            var bounds = dataGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            return x >= bounds.Right - 6 && x <= bounds.Right
                && y >= bounds.Bottom - 6 && y <= bounds.Bottom;
        }
    }

    public class AddressColumnComparer : IComparer<IOGenerationData>
    {
        //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        private readonly int sortOrderModifier;
        public AddressColumnComparer(SortOrder sortOrder)
        {
            sortOrderModifier = (sortOrder == SortOrder.Ascending ? -1 : 1);
        }

        public int Compare(IOGenerationData x, IOGenerationData y)
        {
            var tagX = x?.TagAddress;
            var tagY = y?.TagAddress;
            if (tagX == null && tagY == null)
            {
                return 0;
            }
            else if (tagX == null)
            {
                return -1 * sortOrderModifier;
            }
            else if (tagY == null)
            {
                return 1 * sortOrderModifier;
            }
            else
            {
                return tagX.CompareTo(tagY);
            }
        }
        /*
        public int Compare(SimaticTagAddress x, SimaticTagAddress y)
        {
            if (x == null)
            {
                return -1 * sortOrderModifier;
            }
            else if (y == null)
            {
                return 1 * sortOrderModifier;
            }
            else
            {
                return x.CompareTo(y);
            }
        }


        private SimaticTagAddress GetAddressFromRow(Object obj)
        {
            if (!(obj is DataGridViewRow row))
            {
                return null;
            }

            return SimaticTagAddress.FromAddress(row.Cells[0].Value?.ToString());
        }*/
    }
}

/*
this.dataGridView.SortCompare += (sender, args) =>
{
    if (args.Column.Index == 0)
    {
        //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        var sortOrderMultiplier = dataGridView.SortOrder == SortOrder.Ascending ? -1 : 1;

        var cell1Address = SimaticTagAddress.FromAddress(args.CellValue1?.ToString());
        var cell2Address = SimaticTagAddress.FromAddress(args.CellValue2?.ToString());
        if (cell1Address == null)
        {
            args.SortResult = -1 * sortOrderMultiplier;
        }
        else if (cell2Address == null)
        {
            args.SortResult = 1 * sortOrderMultiplier;
        }
        else
        {
            args.SortResult = cell1Address.CompareTo(cell2Address);
        }

        args.Handled = true;
    }
};

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