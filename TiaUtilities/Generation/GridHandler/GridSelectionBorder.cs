using System.Diagnostics;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.GridHandler
{
    internal class SelectedCellsBorderCoordinates
    {
        public required int Top { get; init; } //Lowest Row Index
        public required int Bottom { get; init; } //Highest Row Index
        public required int Left { get; init; } //Lowest Column Index
        public required int Right { get; init; } //Highest Column Index

        public bool IsTop(DataGridViewCell cell) => cell.RowIndex == this.Top;
        public bool IsBottom(DataGridViewCell cell) => cell.RowIndex == this.Bottom;
        public bool IsLeft(DataGridViewCell cell) => cell.ColumnIndex == this.Left;
        public bool IsRight(DataGridViewCell cell) => cell.ColumnIndex == this.Right;

        public override string ToString()
        {
            return $"Top:{Top},Left:{Left},Bottom:{Bottom},Right:{Right}";
        }

        internal static SelectedCellsBorderCoordinates CreateFromGrid(DataGridView dataGridView, int rowOffset = 0, int columnOffset = 0)
        {
            int lowestRowIndex = -1;
            int highestRowIndex = -1;

            int lowestColumnIndex = -1;
            int highestColumnIndex = -1;

            var selectedCells = dataGridView.SelectedCells.Cast<DataGridViewCell>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                var rowIndex = cell.RowIndex;
                var columnIndex = cell.ColumnIndex;

                if (!GridUtils.AreCoordinatesValid(dataGridView, rowIndex, columnIndex))
                {
                    continue;
                }

                lowestRowIndex = (lowestRowIndex == -1 || rowIndex < lowestRowIndex) ? rowIndex : lowestRowIndex;
                highestRowIndex = (highestRowIndex == -1 || rowIndex > highestRowIndex) ? rowIndex : highestRowIndex;

                lowestColumnIndex = (lowestColumnIndex == -1 || columnIndex < lowestColumnIndex) ? columnIndex : lowestColumnIndex;
                highestColumnIndex = (highestColumnIndex == -1 || columnIndex > highestColumnIndex) ? columnIndex : highestColumnIndex;
            }

            return new()
            {
                Top = lowestRowIndex + rowOffset,
                Bottom = highestRowIndex + rowOffset,
                Left = lowestColumnIndex + columnOffset,
                Right = highestColumnIndex + columnOffset,
            };
        }
    }


    public class GridSelectionBorder(DataGridView dataGridView, GridSettings gridSettings)
    {
        const int EXTRA_PX_BORDER_MOUSE_SELECTION = 4;

        const int BORDER_TOP = 0;
        const int BORDER_RIGHT = 1;
        const int BORDER_BOTTOM = 2;
        const int BORDER_LEFT = 3;

        public RectangleF[] Borders { get; private set; } = { RectangleF.Empty, RectangleF.Empty, RectangleF.Empty, RectangleF.Empty };

        public bool CursorInsideBorder { get; private set; } = false;

        private bool selectionIsPlanar = false;
        private readonly List<DataGridViewCell> oldSelectedCellsList = [];

        public void EventSelectionChanged()
        {
            var selectedCells = dataGridView.SelectedCells;
            if (selectedCells == null || selectedCells.Count == 0)
            {
                return;
            }

            this.selectionIsPlanar = GridUtils.AreSelectedCellsPlanar(selectedCells, dataGridView.Columns, dataGridView.Rows);

            if (this.oldSelectedCellsList.Count > 0)
            {
                selectedCells.Cast<DataGridViewCell>()
                             .Intersect(oldSelectedCellsList)
                             .Where(c => c.DataGridView == dataGridView) //Sometime it could get confused and select cells from different dataGridView.
                             .ForEach(c => dataGridView.InvalidateCell(c)); //Invalide cells so the are redrawn.

                this.oldSelectedCellsList.Clear();
            }

            this.oldSelectedCellsList.AddRange(selectedCells.Cast<DataGridViewCell>());
        }

        public RectangleF[] CalculateBorders(int borderSize, int rowOffset = 0, int columnOffset = 0)
        {
            RectangleF[] borders = { RectangleF.Empty, RectangleF.Empty, RectangleF.Empty, RectangleF.Empty };
            if (!dataGridView.AreAllCellsSelected(false) && selectionIsPlanar)
            {
                var coords = SelectedCellsBorderCoordinates.CreateFromGrid(dataGridView, rowOffset, columnOffset);

                var top = Math.Max(0, coords.Top);
                var left = Math.Max(0, coords.Left);
                var right = Math.Max(0, coords.Right);
                var bottom = Math.Min(dataGridView.RowCount - 1, coords.Bottom);

                Debug.WriteLine($"CalculateBorders. Coords: {coords}");

                DataGridViewCell? topLeftCell, topRightCell, bottomLeftCell, bottomRightCell;

                if (rowOffset != 0 || columnOffset != 0)
                {
                    topLeftCell = dataGridView.Rows[top].Cells[left];
                    topRightCell = dataGridView.Rows[top].Cells[right];
                    bottomLeftCell = dataGridView.Rows[bottom].Cells[left];
                    bottomRightCell = dataGridView.Rows[bottom].Cells[right];
                }
                else
                {
                    topLeftCell = this.oldSelectedCellsList.FirstOrDefault(c => c.DisplayColumnIndex() == left && c.RowIndex == top);
                    topRightCell = this.oldSelectedCellsList.FirstOrDefault(c => c.DisplayColumnIndex() == right && c.RowIndex == top);
                    bottomLeftCell = this.oldSelectedCellsList.FirstOrDefault(c => c.DisplayColumnIndex() == left && c.RowIndex == bottom);
                    bottomRightCell = this.oldSelectedCellsList.FirstOrDefault(c => c.DisplayColumnIndex() == right && c.RowIndex == bottom);
                }

                if (topLeftCell == null || !topLeftCell.Visible ||
                    topRightCell == null || !topRightCell.Visible ||
                    bottomLeftCell == null || !bottomLeftCell.Visible ||
                    bottomRightCell == null || !bottomRightCell.Visible)
                {
                    return borders;
                }

                Rectangle topLeftBounds;
                Rectangle topRightBounds;
                if (topLeftCell.Displayed && topRightCell.Displayed)
                {
                    topLeftBounds = dataGridView.GetCellDisplayRectangle(topLeftCell.ColumnIndex, topLeftCell.RowIndex, true);
                    topRightBounds = dataGridView.GetCellDisplayRectangle(topRightCell.ColumnIndex, topRightCell.RowIndex, true);

                    borders[BORDER_TOP] = new()
                    {
                        X = topLeftBounds.X,
                        Y = topLeftBounds.Y,
                        Width = (topRightBounds.X + topRightBounds.Width) - (topLeftBounds.X + 1),
                        Height = borderSize
                    };
                }
                else
                {
                    var topDisplayRow = dataGridView.FirstDisplayedScrollingRowIndex;
                    topLeftBounds = dataGridView.GetCellDisplayRectangle(topLeftCell.ColumnIndex, topDisplayRow, true);
                    topRightBounds = dataGridView.GetCellDisplayRectangle(topRightCell.ColumnIndex, topDisplayRow, true);
                }

                var rowHeight = dataGridView.RowTemplate.Height;

                Rectangle bottomLeftBounds = dataGridView.GetCellDisplayRectangle(bottomLeftCell.ColumnIndex, bottomLeftCell.RowIndex, true);
                Rectangle bottomRightBounds = dataGridView.GetCellDisplayRectangle(bottomRightCell.ColumnIndex, bottomRightCell.RowIndex, true);
                if (bottomLeftCell.Displayed && bottomLeftBounds.Height == rowHeight && bottomRightCell.Displayed && bottomRightBounds.Height == rowHeight)
                {
                    borders[BORDER_BOTTOM] = new()
                    {
                        X = bottomLeftBounds.X,
                        Y = bottomLeftBounds.Y + bottomLeftBounds.Height - 1 - borderSize,
                        Width = (bottomRightBounds.X + bottomRightBounds.Width) - (bottomLeftBounds.X + 1),
                        Height = borderSize
                    };
                }
                else
                {
                    var lastDisplayedRow = dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.Rows.GetRowCount(DataGridViewElementStates.Displayed | DataGridViewElementStates.Visible) - 1;

                    bottomLeftBounds = dataGridView.GetCellDisplayRectangle(bottomLeftCell.ColumnIndex, lastDisplayedRow, true);
                    bottomRightBounds = dataGridView.GetCellDisplayRectangle(bottomRightCell.ColumnIndex, lastDisplayedRow, true);
                }

                borders[BORDER_LEFT] = new()
                {
                    X = topLeftBounds.X,
                    Y = topLeftBounds.Y,
                    Width = borderSize,
                    Height = (bottomLeftBounds.Y + bottomLeftBounds.Height) - topLeftBounds.Y
                };

                borders[BORDER_RIGHT] = new()
                {
                    X = topRightBounds.X + topRightBounds.Width - borderSize - 1,
                    Y = topRightBounds.Y,
                    Width = borderSize,
                    Height = (bottomRightBounds.Y + bottomRightBounds.Height) - topRightBounds.Y
                };
            }

            return borders;
        }

        public void DrawBorders(Graphics graphics, RectangleF[] borders, Color color)
        {
            using Brush borderBrush = new SolidBrush(color);

            GridSelectionBorder.FillRectangleIfNotEmpty(graphics, borderBrush, borders[BORDER_TOP]);
            GridSelectionBorder.FillRectangleIfNotEmpty(graphics, borderBrush, borders[BORDER_BOTTOM]);
            GridSelectionBorder.FillRectangleIfNotEmpty(graphics, borderBrush, borders[BORDER_LEFT]);
            GridSelectionBorder.FillRectangleIfNotEmpty(graphics, borderBrush, borders[BORDER_RIGHT]);
        }

        public void EventPaint(Graphics graphics)
        {
            if (dataGridView.SelectedCells.Cast<DataGridViewCell>().Any(c => c.Displayed))
            {
                this.Borders = this.CalculateBorders(gridSettings.BorderWeight); //Do NOT use offset here!
                this.DrawBorders(graphics, this.Borders, gridSettings.SingleSelectedCellBorderColor);
            }
        }

        public void EventMouseMove_CursorInsideBorder(int x, int y)
        {
            this.CursorInsideBorder = this.IsCursorInsideInflated(x, y, EXTRA_PX_BORDER_MOUSE_SELECTION);
        }

        public void EventMouseLeave()
        {
            this.ClearCursor();
        }

        public void ClearCursor()
        {
            this.CursorInsideBorder = false;
        }

        public bool IsCursorInsideInflated(float x, float y, int inflate)
        {
            var topBorder = InflateRectangleIfNotEmpty(this.Borders[BORDER_TOP], inflate);
            var bottomBorder = InflateRectangleIfNotEmpty(this.Borders[BORDER_BOTTOM], inflate);
            var leftBorder = InflateRectangleIfNotEmpty(this.Borders[BORDER_LEFT], inflate);
            var rightBorder = InflateRectangleIfNotEmpty(this.Borders[BORDER_RIGHT], inflate);

            PointF point = new(x, y);
            return topBorder.Contains(point) || bottomBorder.Contains(point) || leftBorder.Contains(point) || rightBorder.Contains(point);
        }

        private static void FillRectangleIfNotEmpty(Graphics graphics, Brush brush, RectangleF rectangle)
        {
            if (rectangle != RectangleF.Empty)
            {
                graphics.FillRectangle(brush, rectangle);
            }
        }

        private static RectangleF InflateRectangleIfNotEmpty(RectangleF rectangle, int inflate)
        {
            var rectangleCopy = rectangle;
            if (rectangleCopy != RectangleF.Empty)
            {
                rectangleCopy.Inflate(inflate * 2, inflate * 2);
            }
            return rectangleCopy;
        }

    }
}
