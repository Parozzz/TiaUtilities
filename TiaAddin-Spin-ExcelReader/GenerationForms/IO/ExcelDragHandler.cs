using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.GenerationForms.IO
{
    public class ExcelDragHandler
    {
        private readonly IOGenerationForm form;
        private readonly DataGridView dataGridView;

        private bool started = false;
        private int rowIndexStart = -1;
        private int columnIndex = -1;

        private int topSelectionRowIndex = -1; //Lowest
        private int bottomSelectionRowIndex = -1; //Highest
        private uint SelectedRows { get => (uint)(bottomSelectionRowIndex - topSelectionRowIndex) + 1; }

        private ToolTip dragToolTip;

        public bool DraggingDown { get => topSelectionRowIndex == rowIndexStart; }

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

                if (IsInsideTriangle(args.X, args.Y, dataGridView.CurrentCell))
                {
                    dataGridView.Cursor = Cursors.Cross;
                }
            };

            this.dataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    var hitCell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
                    if (hitCell == dataGridView.CurrentCell && IsInsideTriangle(args.X, args.Y, hitCell))
                    {
                        started = true;
                        bottomSelectionRowIndex = topSelectionRowIndex = rowIndexStart = hitTest.RowIndex;
                        columnIndex = hitTest.ColumnIndex;
                    }
                }
            };


            this.dataGridView.LostFocus += delegate { this.Clear(); };

            this.dataGridView.MouseUp += (sender, args) =>
            {
                if (started)
                {
                    started = false;

                    var rowIndexEnumeration = Enumerable.Range(topSelectionRowIndex, (int)SelectedRows);
                    if (columnIndex == 0)
                    {
                        var tagAddress = SimaticTagAddress.FromAddress(dataGridView.Rows[rowIndexStart]?.Cells[0].Value?.ToString());
                        if (tagAddress != null) //If is not a valid address, i won't care about doing any stuff.
                        {
                            if (!DraggingDown)
                            {//Since i always start from the top to parse the addresses, if i am dragging down i need to start from the lowest value!
                                tagAddress.PreviousBit(SimaticDataType.BYTE, SelectedRows - 1);
                            }

                            var dragCellList = new List<CellChange>();
                            foreach (var rowIndex in rowIndexEnumeration)
                            {
                                var cellChange = new CellChange(dataGridView, 0, rowIndex) { NewValue = tagAddress.GetAddress() };
                                dragCellList.Add(cellChange);

                                tagAddress.NextBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                            }

                            form.ChangeCells(dragCellList);
                        }
                    }

                    dataGridView.Refresh();

                    this.Clear();
                }

            };

            dataGridView.SelectionChanged += (sender, args) =>
            {
                if (!started)
                {
                    return;
                }

                var highestRowIndex = int.MinValue;
                var lowestRowIndex = int.MaxValue;

                //When dragging, only allow cell on the column to be selected! I don't care about other ways and want it simple.
                foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
                {
                    bool sameColumn = selectedCell.ColumnIndex == columnIndex;
                    if (!sameColumn)
                    {
                        selectedCell.Selected = false; //Only keep the ones on the same columns of the selected!
                        continue;
                    }

                    highestRowIndex = Math.Max(highestRowIndex, selectedCell.RowIndex);
                    lowestRowIndex = Math.Min(lowestRowIndex, selectedCell.RowIndex);
                }

                topSelectionRowIndex = lowestRowIndex;
                bottomSelectionRowIndex = highestRowIndex;

                if (dragToolTip == null)
                {
                    dragToolTip = new ToolTip
                    {
                        Active = true,
                        BackColor = Color.DarkGray,
                        ForeColor = Color.Black,
                        ShowAlways = true,
                    };
                }

                if (columnIndex == 0)
                {
                    var tagAddress = SimaticTagAddress.FromAddress(dataGridView.Rows[rowIndexStart]?.Cells[0].Value?.ToString());
                    if (tagAddress != null)
                    {
                        var _ = DraggingDown
                                    ? tagAddress.NextBit(SimaticDataType.BYTE, SelectedRows - 1)
                                    : tagAddress.PreviousBit(SimaticDataType.BYTE, SelectedRows - 1);
                        dragToolTip.Show(tagAddress.GetAddress(), form, form.PointToClient(Cursor.Position));
                    }
                }
            };
        }

        public bool PaintCell(DataGridViewCellPaintingEventArgs args)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            var style = args.CellStyle;

            if (rowIndex < 0 || columnIndex < 0)
            {
                return false;
            }
            else if (started)
            {
                style.SelectionBackColor = IOGenerationForm.DRAGGED_CELL_BACK_COLOR;

                args.PaintBackground(bounds, true);
                args.PaintContent(bounds);

                args.Handled = true;
                return true;
            }

            args.PaintBackground(bounds, true);
            args.PaintContent(bounds);

            var currentCell = dataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.
                args.Paint(bounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using (var pen = new Pen(IOGenerationForm.SELECTED_CELL_COLOR, 2))
                {//Border
                    Rectangle rect = args.CellBounds;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    graphics.DrawRectangle(pen, rect);
                }

                using (var brush = new SolidBrush(IOGenerationForm.SELECTED_CELL_COLOR))
                {//Little triangle in the lower part only for current cell
                    var point1 = new Point(bounds.Right - 1, bounds.Bottom - 10);
                    var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                    var point3 = new Point(bounds.Right - 10, bounds.Bottom - 1);

                    Point[] pt = new Point[] { point1, point2, point3 };
                    graphics.FillPolygon(brush, pt);
                }
            }

            args.Handled = true;
            return true;
        }

        public bool IsInsideTriangle(int x, int y, DataGridViewCell cell)
        {
            return cell != null && IsInsideTriangle(x, y, cell.ColumnIndex, cell.RowIndex);
        }

        public bool IsInsideTriangle(int x, int y, int columnIndex, int rowIndex)
        {
            var bounds = dataGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            return x >= bounds.Right - 6 && x <= bounds.Right + 2 //Go outside a bit of the cell to avoid misclick that sometime happend
                && y >= bounds.Bottom - 6 && y <= bounds.Bottom + 2;
        }

        private void Clear()
        {
            started = false;
            rowIndexStart = columnIndex = topSelectionRowIndex = bottomSelectionRowIndex = -1;

            if (dragToolTip != null)
            {
                dragToolTip.Active = false;
                dragToolTip.Dispose();
                dragToolTip = null;
            }
        }
    }
}
