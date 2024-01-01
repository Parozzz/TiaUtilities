using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader
{
    public partial class IOGenerationForm : Form
    {
        public IOGenerationForm()
        {
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

            this.dataGridView.KeyDown += (sender, args) =>
            {
                bool ctrlV = args.Modifiers == Keys.Control && args.KeyCode == Keys.V;
                bool shiftIns = args.Modifiers == Keys.Shift && args.KeyCode == Keys.Insert;

                if (ctrlV || shiftIns)
                    Paste();

                if (args.KeyCode == Keys.Delete || args.KeyCode == Keys.Cancel)
                {
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
                            dataGridView.Rows.RemoveAt(selectedCell.RowIndex);
                        }
                        else
                        {
                            selectedCell.Value = "";
                        }
                    }

                }
            };

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
        }

        public void Paste()
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                string[] pastedRows = Regex.Split(clipboardDataObject.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");

                var currentCell = dataGridView.CurrentCell;
                int rowPointer = currentCell.RowIndex; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!

                if (dataGridView.Rows.Count < (currentCell.RowIndex + pastedRows.Length))
                {
                    dataGridView.Rows.Add(currentCell.RowIndex + pastedRows.Length - dataGridView.Rows.Count + 1); //Adding one extra to avoid remaining without rows!
                }

                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedRowCells = pastedRow.Split('\t');

                    var maxColumn = Math.Min(pastedRowCells.Length, dataGridView.ColumnCount - currentCell.ColumnIndex);
                    for (int pastedCellPointer = 0, columnPointer = currentCell.ColumnIndex; columnPointer < maxColumn; columnPointer++, pastedCellPointer++)
                    {
                        dataGridView.Rows[rowPointer].Cells[columnPointer].Value = pastedRowCells[pastedCellPointer];
                    }
                    rowPointer++;
                }
            }
        }

    }

    public class TestDataSource
    {
        public string Address { get; set; }
        public string IOName { get; set; }
        public string DB { get; set; }
        public string Variable { get; set; }
        public string Comment { get; set; }
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