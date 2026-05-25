using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridExcelDragHandler<T>(GridHandler<T> gridHandler, GridSettings settings) where T : GridData
    {
        public const int TRIANGLE_SIZE = 13;

        private readonly GridHandler<T> gridHandler = gridHandler;
        private DataGridView DataGridView { get => this.gridHandler.InternalDataGridView; }

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

        public bool MouseShouldDisplayCursor(MouseEventArgs args)
        {
            return !started && IsInsideTriangle(args.X, args.Y, this.DataGridView.CurrentCell, xyCellCoordinates: true);
        }

        public void Init()
        {
            this.DataGridView.LostFocus += (sender, args) => this.Clear();

            this.DataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = this.DataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    var hitCell = this.DataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
                    if(hitCell.ReadOnly)
                    {
                        return;
                    }

                    if (hitCell == this.DataGridView.CurrentCell && IsInsideTriangle(args.X, args.Y, hitCell, xyCellCoordinates: false))
                    {
                        started = true;
                        bottomSelectionRowIndex = topSelectionRowIndex = rowIndexStart = hitTest.RowIndex;
                        draggedColumnIndex = hitTest.ColumnIndex;
                    }
                }
            };

            this.DataGridView.MouseUp += (sender, args) =>
            {
                if (started)
                {
                    started = false;

                    var eventArgs = this.CreateDragEventArgs();
                    this.gridHandler.CallExcelDragDoneEvent(eventArgs);

                    this.Clear();
                }
            };

            this.DataGridView.SelectionChanged += (sender, args) =>
            {
                if (!started)
                {
                    return;
                }

                var highestRowIndex = int.MinValue;
                var lowestRowIndex = int.MaxValue;

                //When dragging, only allow cell on the column to be selected! I don't care about other ways and want it simple.
                foreach (DataGridViewCell selectedCell in this.DataGridView.SelectedCells)
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
                this.gridHandler.CallExcelDragPreviewEvent(eventArgs);

                if (!string.IsNullOrEmpty(eventArgs.TooltipString) && this.DataGridView.FindForm() is Form form)
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

        public void CellPaiting(DataGridViewCellPaintingEventArgs args)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            var style = args.CellStyle;

            if (rowIndex < 0 || columnIndex < 0 || graphics == null || style == null)
            {
                return;
            }

            if (started)
            {
                style.SelectionBackColor = settings.DragSelectedCellBorderColor;
            }
        }

        public void PaintTriangle(Graphics graphics)
        {
            var currentCell = this.DataGridView.CurrentCell;
            if (currentCell is DataGridViewTextBoxCell && !currentCell.ReadOnly && this.DataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.

                var bounds = this.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);

                using var triangleBrush = new SolidBrush(settings.SelectedCellTriangleColor);

                //Little triangle in the lower part only for current cell
                var point1 = new Point(bounds.Right - 1, bounds.Bottom - TRIANGLE_SIZE);
                var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                var point3 = new Point(bounds.Right - TRIANGLE_SIZE, bounds.Bottom - 1);

                Point[] pt = [point1, point2, point3];
                graphics.FillPolygon(triangleBrush, pt);
            }

        }

        public bool IsInsideTriangle(int x, int y, DataGridViewCell cell, bool xyCellCoordinates)
        {
            if (cell is not DataGridViewTextBoxCell)
            {
                return false;
            }

            var bounds = this.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);

            var xMin = bounds.Right - TRIANGLE_SIZE;
            var xMax = bounds.Right + 2;
            var yMin = bounds.Bottom - TRIANGLE_SIZE;
            var yMax = bounds.Bottom + 2;

            if(xyCellCoordinates)
            {
                x += bounds.X;
                y += bounds.Y;
            }

            //Debug.WriteLine($"Pos: {x},{y}. X: [{xMin},{xMax}], Y: [{yMin},{yMax}]");

            //Go outside a bit of the cell to avoid misclick that sometime happend
            return x >= xMin && x <= xMax && y >= yMin && y <= yMax;
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
