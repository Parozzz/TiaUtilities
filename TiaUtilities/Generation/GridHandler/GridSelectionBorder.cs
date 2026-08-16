using DocumentFormat.OpenXml.Wordprocessing;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridSelectionBorder
    {
        const int EXTRA_PX_BORDER_MOUSE_SELECTION = 3;

        public RectangleF TopBorder { get; private set; } = RectangleF.Empty;
        public RectangleF BottomBorder { get; private set; } = RectangleF.Empty;
        public RectangleF LeftBorder { get; private set; } = RectangleF.Empty;
        public RectangleF RightBorder { get; private set; } = RectangleF.Empty;

        public bool CursorInsideBorder { get; private set; } = false;

        private readonly DataGridView dataGridView;
        private readonly GridSettings gridSettings;

        private bool selectionIsPlanar = false;
        private readonly List<DataGridViewCell> oldSelectedCellsList = [];

        public GridSelectionBorder(DataGridView dataGridView, GridSettings gridSettings)
        {
            this.dataGridView = dataGridView;
            this.gridSettings = gridSettings;
        }

        public void EventSelectionChanged()
        {
            var selectedCells = this.dataGridView.SelectedCells;
            if (selectedCells == null || selectedCells.Count == 0)
            {
                return;
            }

            this.selectionIsPlanar = GridUtils.AreSelectedCellsPlanar(selectedCells, this.dataGridView.Columns, dataGridView.Rows);

            if (this.oldSelectedCellsList.Count > 0)
            {
                selectedCells.Cast<DataGridViewCell>()
                             .Intersect(oldSelectedCellsList)
                             .Where(c => c.DataGridView == this.dataGridView) //Sometime it could get confused and select cells from different dataGridView.
                             .ForEach(c => this.dataGridView.InvalidateCell(c)); //Invalide cells so the are redrawn.

                this.oldSelectedCellsList.Clear();
            }

            this.oldSelectedCellsList.AddRange(selectedCells.Cast<DataGridViewCell>());
        }

        public void CalculateSelectionBorders()
        {
            TopBorder = BottomBorder = LeftBorder = RightBorder = RectangleF.Empty;
            if (!this.dataGridView.AreAllCellsSelected(false) && selectionIsPlanar && this.dataGridView.SelectedCells.Cast<DataGridViewCell>().Any(c => c.Displayed))
            {
                var borders = GridUtils.GetSelectedCellsBorderCoordinates(this.dataGridView);

                var topLeft = this.oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Top);
                var topRight = this.oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Top);
                var bottomLeft = this.oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Bottom);
                var bottomRight = this.oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Bottom);

                if (topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
                {
                    var borderSize = this.gridSettings.BorderWeight;

                    Rectangle topLeftBounds;
                    Rectangle topRightBounds;
                    if (topLeft.Displayed && topRight.Displayed)
                    {
                        topLeftBounds = this.dataGridView.GetCellDisplayRectangle(topLeft.ColumnIndex, topLeft.RowIndex, true);
                        topRightBounds = this.dataGridView.GetCellDisplayRectangle(topRight.ColumnIndex, topRight.RowIndex, true);

                        TopBorder = new()
                        {
                            X = topLeftBounds.X,
                            Y = topLeftBounds.Y,
                            Width = (topRightBounds.X + topRightBounds.Width) - (topLeftBounds.X + 1),
                            Height = borderSize
                        };
                    }
                    else
                    {
                        var topDisplayRow = this.dataGridView.FirstDisplayedScrollingRowIndex;
                        topLeftBounds = this.dataGridView.GetCellDisplayRectangle(topLeft.ColumnIndex, topDisplayRow, true);
                        topRightBounds = this.dataGridView.GetCellDisplayRectangle(topRight.ColumnIndex, topDisplayRow, true);
                    }

                    var rowHeight = this.dataGridView.RowTemplate.Height;

                    Rectangle bottomLeftBounds = this.dataGridView.GetCellDisplayRectangle(bottomLeft.ColumnIndex, bottomLeft.RowIndex, true);
                    Rectangle bottomRightBounds = this.dataGridView.GetCellDisplayRectangle(bottomRight.ColumnIndex, bottomRight.RowIndex, true);
                    if (bottomLeft.Displayed && bottomLeftBounds.Height == rowHeight && bottomRight.Displayed && bottomRightBounds.Height == rowHeight)
                    {
                        BottomBorder = new()
                        {
                            X = bottomLeftBounds.X,
                            Y = bottomLeftBounds.Y + bottomLeftBounds.Height - 1 - borderSize,
                            Width = (bottomRightBounds.X + bottomRightBounds.Width) - (bottomLeftBounds.X + 1),
                            Height = borderSize
                        };
                    }
                    else
                    {
                        var lastDisplayedRow = this.dataGridView.FirstDisplayedScrollingRowIndex + this.dataGridView.Rows.GetRowCount(DataGridViewElementStates.Displayed | DataGridViewElementStates.Visible) - 1;

                        bottomLeftBounds = this.dataGridView.GetCellDisplayRectangle(bottomLeft.ColumnIndex, lastDisplayedRow, true);
                        bottomRightBounds = this.dataGridView.GetCellDisplayRectangle(bottomRight.ColumnIndex, lastDisplayedRow, true);
                    }

                    LeftBorder = new()
                    {
                        X = topLeftBounds.X,
                        Y = topLeftBounds.Y,
                        Width = borderSize,
                        Height = (bottomLeftBounds.Y + bottomLeftBounds.Height) - topLeftBounds.Y
                    };

                    RightBorder = new()
                    {
                        X = topRightBounds.X + topRightBounds.Width - borderSize - 1,
                        Y = topRightBounds.Y,
                        Width = borderSize,
                        Height = (bottomRightBounds.Y + bottomRightBounds.Height) - topRightBounds.Y
                    };
                }
            }
        }

        public void EventPaint(Graphics graphics)
        {
            var borderSize = this.gridSettings.BorderWeight;

            using Pen borderPen = new(this.gridSettings.SingleSelectedCellBorderColor, borderSize);
            using Brush borderBrush = new SolidBrush(this.gridSettings.SingleSelectedCellBorderColor);

            if (this.TopBorder != RectangleF.Empty)
            {
                graphics.FillRectangle(borderBrush, this.TopBorder);
            }

            if (this.BottomBorder != RectangleF.Empty)
            {
                graphics.FillRectangle(borderBrush, this.BottomBorder);
            }

            if (this.LeftBorder != RectangleF.Empty)
            {
                graphics.FillRectangle(borderBrush, LeftBorder);
            }

            if (this.RightBorder != RectangleF.Empty)
            {
                graphics.FillRectangle(borderBrush, this.RightBorder);
            }
        }

        public void EventCellMouseMove(int columnIndex, int rowIndex)
        {
            this.CursorInsideBorder = false;

            if(columnIndex >= 0 && columnIndex < this.dataGridView.ColumnCount && rowIndex >= 0 && rowIndex < this.dataGridView.RowCount)
            {
                //Display cursor for drag&drop
                var cell = this.dataGridView.Rows[rowIndex].Cells[columnIndex];
                this.CursorInsideBorder = (cell.Selected && this.IsCursorInsideInflated(EXTRA_PX_BORDER_MOUSE_SELECTION));
            }
        }

        public void EventMouseLeave()
        {
            this.ClearCursor();
        }

        public void ClearCursor()
        {
            this.CursorInsideBorder = false;
        }

        public bool IsCursorInsideInflated(int inflate)
        {
            var topBorder = TopBorder;
            if (topBorder != RectangleF.Empty)
            {
                topBorder.Inflate(inflate, inflate);
            }

            var bottomBorder = BottomBorder;
            if (bottomBorder != RectangleF.Empty)
            {
                bottomBorder.Inflate(inflate, inflate);
            }

            var leftBorder = LeftBorder;
            if (leftBorder != RectangleF.Empty)
            {
                leftBorder.Inflate(inflate, inflate);
            }

            var rightBorder = RightBorder;
            if (rightBorder != RectangleF.Empty)
            {
                rightBorder.Inflate(inflate, inflate);
            }

            var cursorPosition = this.dataGridView.PointToClient(Cursor.Position);
            return topBorder.Contains(cursorPosition) || 
                bottomBorder.Contains(cursorPosition) || 
                leftBorder.Contains(cursorPosition) || 
                rightBorder.Contains(cursorPosition);
        }
    }
}
