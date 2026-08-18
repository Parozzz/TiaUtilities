using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.GridImprovements
{
    public class GridDragDownHandler(ExcelLikeDataGridView dataGridView, GridHandlerEventCaller gridHandlerEventCaller, GridSettings settings)
    {
        public const int TRIANGLE_SIZE = 13;

        public bool Started { get; private set; } = false;
        public bool DraggingDown { get => topSelectionRowIndex == rowIndexStart; }

        private int rowIndexStart = -1;
        private int draggedColumnIndex = -1;

        private int topSelectionRowIndex = -1; //Lowest
        private int bottomSelectionRowIndex = -1; //Highest
        private uint SelectedRows { get => (uint)(bottomSelectionRowIndex - topSelectionRowIndex) + 1; }

        private ToolTip? dragToolTip;

        public bool RequiresCursorCross(int x, int y, int columnIndex, int rowIndex)
        {
            if (this.Started)
            {
                return false;
            }

            var currentCellAddress = dataGridView.CurrentCellAddress;
            if (columnIndex != currentCellAddress.X || rowIndex != currentCellAddress.Y)
            {
                return false;
            }

            var selectedCellCount = dataGridView.GetCellCount(DataGridViewElementStates.Selected);
            if (selectedCellCount != 1)
            {
                return false;
            }

            return this.IsInsideTriangle(x, y, dataGridView.CurrentCell);
        }

        public void EventSelectionChanged()
        {
            if (!Started)
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
            gridHandlerEventCaller.CallExcelDragPreviewEvent(eventArgs);

            if (!string.IsNullOrEmpty(eventArgs.TooltipString) && dataGridView.FindForm() is Form form)
            {
                dragToolTip.Show(eventArgs.TooltipString, form, form.PointToClient(Cursor.Position));
            }
        }

        public void EventMouseDown(Point location, int columnIndex, int rowIndex)
        {
            if (dataGridView.GetCellCount(DataGridViewElementStates.Selected) != 1)
            {
                return;
            }

            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            if (cell.ReadOnly)
            {
                return;
            }

            if (cell == dataGridView.CurrentCell && this.IsInsideTriangle(location, cell))
            {
                Started = true;

                bottomSelectionRowIndex = topSelectionRowIndex = rowIndexStart = rowIndex;
                draggedColumnIndex = columnIndex;
            }
        }

        public void EventMouseUp()
        {
            if (Started)
            {
                Started = false;

                var eventArgs = this.CreateDragEventArgs();
                gridHandlerEventCaller.CallExcelDragDoneEvent(eventArgs);

                this.Clear();
            }
        }

        public void EventLostFocus()
        {
            this.Clear();
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

        public void EventCellPainting(DataGridViewCellPaintingEventArgs args)
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

            if (Started)
            {
                style.SelectionBackColor = settings.DragSelectedCellBorderColor;
            }
        }

        public void PaintTriangle(Graphics graphics)
        {
            var currentCell = dataGridView.CurrentCell;
            if (currentCell is DataGridViewTextBoxCell textBoxCell && !textBoxCell.IsInEditMode && !currentCell.ReadOnly && dataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.

                var bounds = dataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);

                using var triangleBrush = new SolidBrush(settings.SelectedCellTriangleColor);

                //Little triangle in the lower part only for current cell
                var point1 = new Point(bounds.Right - 1, bounds.Bottom - TRIANGLE_SIZE);
                var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                var point3 = new Point(bounds.Right - TRIANGLE_SIZE, bounds.Bottom - 1);

                Point[] pt = [point1, point2, point3];
                graphics.FillPolygon(triangleBrush, pt);
            }

        }

        public bool IsInsideTriangle(Point p, DataGridViewCell cell, bool xyCellCoordinates = false) => IsInsideTriangle(p.X, p.Y, cell, xyCellCoordinates);

        public bool IsInsideTriangle(int x, int y, DataGridViewCell cell, bool xyCellCoordinates = false)
        {
            if (cell is not DataGridViewTextBoxCell textBoxCell || textBoxCell.IsInEditMode)
            {
                return false;
            }

            var bounds = dataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);

            var xMin = bounds.Right - TRIANGLE_SIZE;
            var xMax = bounds.Right + 2;
            var yMin = bounds.Bottom - TRIANGLE_SIZE;
            var yMax = bounds.Bottom + 2;

            if (xyCellCoordinates)
            {
                x += bounds.X;
                y += bounds.Y;
            }

            //Go outside a bit of the cell to avoid misclick that sometime happend
            return x >= xMin && x <= xMax && y >= yMin && y <= yMax;
        }

        private void Clear()
        {
            Started = false;
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
