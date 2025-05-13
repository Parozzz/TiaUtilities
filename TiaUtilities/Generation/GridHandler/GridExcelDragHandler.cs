using TiaUtilities.Generation.GridHandler.CellPainters;
using TiaUtilities.Generation.GridHandler.Events;
using static TiaUtilities.Generation.GridHandler.CellPainters.GridCellPaintHandler;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridExcelDragHandler<T>(DataGridView dataGridView, GridEvents<T> gridEvents, GridSettings settings) : IGridCellPainter where T : IGridData
    {
        public const int TRIANGLE_SIZE = 13;

        private bool started = false;
        private int rowIndexStart = -1;
        private int draggedColumnIndex = -1;

        private int topSelectionRowIndex = -1; //Lowest
        private int bottomSelectionRowIndex = -1; //Highest
        private uint SelectedRows { get => (uint)(bottomSelectionRowIndex - topSelectionRowIndex) + 1; }

        private ToolTip? dragToolTip;

        public bool DraggingDown { get => topSelectionRowIndex == rowIndexStart; }

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

            dataGridView.LostFocus += (sender, args) => this.Clear();

            dataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    var hitCell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
                    if (hitCell == dataGridView.CurrentCell && IsInsideTriangle(args.X, args.Y, hitCell))
                    {
                        started = true;
                        bottomSelectionRowIndex = topSelectionRowIndex = rowIndexStart = hitTest.RowIndex;
                        draggedColumnIndex = hitTest.ColumnIndex;
                    }
                }
            };

            dataGridView.MouseUp += (sender, args) =>
            {
                if (started)
                {
                    started = false;

                    var eventArgs = this.CreateDragEventArgs();
                    gridEvents.ExcelDragDoneEvent(dataGridView, eventArgs);

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
                    bool sameColumn = selectedCell.ColumnIndex == draggedColumnIndex;
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

                dragToolTip ??= new ToolTip
                {
                    Active = true,
                    BackColor = Color.DarkGray,
                    ForeColor = Color.Black,
                    ShowAlways = true,
                };

                var eventArgs = this.CreateDragEventArgs();
                gridEvents.ExcelDragPreviewEvent(dataGridView, eventArgs);
                if (!string.IsNullOrEmpty(eventArgs.TooltipString) && dataGridView.FindForm() is Form form)
                {
                    dragToolTip.Show(eventArgs.TooltipString, form, form.PointToClient(Cursor.Position));
                }
            };
        }

        private GridExcelDragEventArgs CreateDragEventArgs()
        {
            return new()
            {
                StartingRow = this.rowIndexStart,
                SelectedRowCount = this.SelectedRows,
                DraggedColumn = this.draggedColumnIndex,
                DraggingDown = this.DraggingDown,
                TopSelectedRow = this.topSelectionRowIndex,
                BottomSelectedRow = this.bottomSelectionRowIndex,
                TooltipString = ""
            };
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintRequest = new PaintRequest();

            var columnIndex = args.ColumnIndex;
            var rowIndex = args.RowIndex;

            if (columnIndex >= 0 && rowIndex >= 0)
            {
                if (started && this.draggedColumnIndex == columnIndex)
                {
                    return paintRequest.Background();
                }

                var currentCell = dataGridView.CurrentCell;
                if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
                {
                    return paintRequest.Background();
                }
            }

            return paintRequest;
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest paintRequest, bool backgroundRequested)
        {
            if (args.Handled)
            {
                return;
            }

            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            var style = args.CellStyle;

            if (graphics == null || style == null)
            {
                return;
            }

            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }
            else if (started)
            {
                style.SelectionBackColor = settings.DragSelectedCellBorderColor;

                args.PaintBackground(bounds, true);
                return;
            }

            var currentCell = dataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.
                args.PaintBackground(bounds, true);

                //args.Paint(bounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using var borderPen = new Pen(settings.SingleSelectedCellBorderColor, 2);

                //Border
                Rectangle rect = args.CellBounds;
                rect.Width -= 1;
                rect.Height -= 1;
                graphics.DrawRectangle(borderPen, rect);


                if (currentCell is DataGridViewTextBoxCell)
                {
                    using var triangleBrush = new SolidBrush(settings.SelectedCellTriangleColor);

                    //Little triangle in the lower part only for current cell
                    var point1 = new Point(bounds.Right - 1, bounds.Bottom - TRIANGLE_SIZE);
                    var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                    var point3 = new Point(bounds.Right - TRIANGLE_SIZE, bounds.Bottom - 1);

                    Point[] pt = [point1, point2, point3];
                    graphics.FillPolygon(triangleBrush, pt);
                }
            }
        }

        public bool IsInsideTriangle(int x, int y, DataGridViewCell cell)
        {
            if (cell is not DataGridViewTextBoxCell)
            {
                return false;
            }

            var bounds = dataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
            return x >= bounds.Right - TRIANGLE_SIZE && x <= bounds.Right + 2 //Go outside a bit of the cell to avoid misclick that sometime happend
                && y >= bounds.Bottom - TRIANGLE_SIZE && y <= bounds.Bottom + 2;
        }

        private void Clear()
        {
            started = false;
            rowIndexStart = draggedColumnIndex = topSelectionRowIndex = bottomSelectionRowIndex = -1;

            if (dragToolTip != null)
            {
                dragToolTip.Active = false;
                dragToolTip.Dispose();
                dragToolTip = null;
            }
        }
    }
}
