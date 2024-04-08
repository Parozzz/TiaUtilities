
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.IO.Data;
using TiaXmlReader.GenerationForms.IO.Sorting;
using TiaXmlReader.UndoRedo;

namespace TiaXmlReader.GenerationForms.IO
{
    public partial class IOGenerationForm : Form
    {
        public const int TOTAL_ROW_COUNT = 1999;
        public static readonly Color SORT_ICON_COLOR = Color.Green;
        public static readonly Color SELECTED_CELL_COLOR = Color.DarkGreen;
        public static readonly Color DRAGGED_CELL_BACK_COLOR = Color.PaleGreen;

        public const int ADDRESS_COLUMN = 0;
        public const int IO_NAME_COLUMN = 1;
        public const int DB_COLUMN = 2;
        public const int VARIABLE_COLUMN = 3;
        public const int COMMENT_COLUMN = 4;

        private readonly IOGenerationDataSource dataSource;

        private readonly IOConfiguration config;
        private readonly IOGenerationFormConfigHandler configHandler;

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly ExcelDragHandler excelDragHandler;
        private readonly SortHandler sortHandler;


        public IOGenerationForm()
        {
            InitializeComponent();

            this.dataSource = new IOGenerationDataSource(this.dataGridView);

            this.config = new IOConfiguration();
            this.configHandler = new IOGenerationFormConfigHandler(this, config, this.dataGridView);

            this.undoRedoHandler = new UndoRedoHandler();
            this.sortHandler = new SortHandler(dataSource, dataGridView, undoRedoHandler);
            this.excelDragHandler = new ExcelDragHandler(this, this.dataGridView);

            Init();
        }

        public void Init()
        {
            //var addressColumn = this.dataTable.Rows.Add(TOTAL_ROW_COUNT);

            this.dataSource.Init();
            this.configHandler.Init();

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.dataGridView, new object[] { true });

            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.dataGridView.AllowUserToAddRows = false;

            var addressColumn = (DataGridViewTextBoxColumn)InitColumn(dataGridView.Columns[ADDRESS_COLUMN], "Indirizzo", 55);
            addressColumn.MaxInputLength = 10;

            var ioNameColumn = InitColumn(dataGridView.Columns[IO_NAME_COLUMN], "Nome IO", 80);
            var dbNameColumn = InitColumn(dataGridView.Columns[DB_COLUMN], "DB", 80);
            var variableColumn = InitColumn(dataGridView.Columns[VARIABLE_COLUMN], "Variabile", 105);
            var commentColumn = InitColumn(dataGridView.Columns[COMMENT_COLUMN], "Commento", 0);

            //this.dataGridView.RowCount = TOTAL_ROW_COUNT;

            #region Cell Paiting
            var paintHandler = new IOGenerationCellPaintHandler(this.dataGridView);
            paintHandler.AddPainter(sortHandler);
            paintHandler.AddPainter(excelDragHandler);
            paintHandler.AddPainter(new IOGenerationFormPreviewCellPainter(this.dataGridView, this.dataSource, this.config));
            paintHandler.Init();
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

                    ChangeCells(new List<CellChange> { cellEdit }, applyChanges: false);

                    cellEdit = null;
                }
            };
            #endregion

            excelDragHandler.Init();
            sortHandler.Init();

            UpdateConfigPanel();
        }

        private DataGridViewColumn CreateColumn(string name, int width)
        {
            var column = new DataGridViewTextBoxColumn();
            return InitColumn(column, name, width);
        }

        private T InitColumn<T>(T column, string name, int width) where T : DataGridViewColumn
        {
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


        private void Paste()
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                var pastedCellList = new List<CellChange>();

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
                                pastedCellList.Add(new CellChange(cell) { NewValue = pastedColumns[pastedColumnPointer] });
                            }
                        }
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

        public void ChangeCells(List<CellChange> cellChangeList, bool applyChanges = true)
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

        private void MemoryTypeComboBox_TextChanged(object sender, EventArgs e)
        {
            UpdateConfigPanel();
        }

        private void UpdateConfigPanel()
        {
            configButtonPanel.SuspendLayout();

            configButtonPanel.Controls.Clear();

            configButtonPanel.Controls.Add(fcConfigButton);
            if (memoryTypeComboBox.Text == "DB")
            {
                configButtonPanel.Controls.Add(dbConfigButton);
            }
            else if (memoryTypeComboBox.Text == "Merker")
            {
                configButtonPanel.Controls.Add(variableTableConfigButton);
            }
            configButtonPanel.Controls.Add(ioTableConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);

            configButtonPanel.ResumeLayout();
        }
    }

    public class CellChange
    {
        public DataGridViewCell cell;
        public object OldValue;
        public object NewValue;
        public int RowIndex => cell.RowIndex;
        public int ColumnIndex => cell.ColumnIndex;

        public CellChange(DataGridView dataGridView, int column, int row) : this(dataGridView.Rows[row].Cells[column])
        {
        }

        public CellChange(DataGridViewCell cell)
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