using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.UndoRedo;

namespace TiaUtilities.Generation.GridHandler.GridImprovements
{
    public class GridSortHandler<T>(
        ExcelLikeDataGridView dataGridView,
        GridDataSource<T> dataSource, 
        GridHandlerEventCaller gridHandlerEventCaller,
        UndoRedoHandler undoRedoHandler, 
        IGridRowComparer<T>? comparer) where T : GridData
    {
        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<T, int>? noSortIndexSnapshot;

        public Color SortIconColor { get; set; } = Color.Green;

        public void EventColumnHeaderMouseClick(MouseButtons button, int columnIndex)
        {
            if (comparer == null || button != MouseButtons.Left)
            {
                return;
            }

            if (comparer.CanSortColumn(columnIndex))
            {
                NextColumnSort(columnIndex);
            }
        }

        private void NextColumnSort(int columnIndex)
        {
            var oldSortOrder = this.sortOrder;
            switch (this.sortOrder)
            {
                case SortOrder.None:
                    this.sortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    this.sortOrder = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                    this.sortOrder = SortOrder.None;
                    break;
            }

            ColumnSort(oldSortOrder, columnIndex);
        }

        private void ColumnSort(SortOrder oldSortOrder, int columnIndex)
        {
            var preSortEventArgs = new GridPreSortEventArgs(oldSortOrder, this.sortOrder, columnIndex);
            gridHandlerEventCaller.CallPreSortEvent(preSortEventArgs);

            if (preSortEventArgs.Handled)
            {
                return;
            }

            this.sortOrder = preSortEventArgs.SortOrder;

            var snapDict = dataSource.CreateIndexListSnapshot();
            if (this.sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortIndexSnapshot = snapDict;
            }

            ClearAllSortGlyphDirection();
            dataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = this.sortOrder;

            if (this.sortOrder == SortOrder.None)
            {
                if (noSortIndexSnapshot != null)
                {
                    dataSource.RestoreIndexListSnapshot(noSortIndexSnapshot);
                    noSortIndexSnapshot = null;
                }
            }
            else
            {
                comparer.SetSortedColumn(columnIndex);
                comparer.SetSortOrder(this.sortOrder);
                dataSource.Sort(comparer, this.sortOrder);
            }

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                dataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = dataSource.CreateIndexListSnapshot();
                dataSource.RestoreIndexListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => dataSource.RestoreIndexListSnapshot(undoSnap));
            });

            var postSortEventArgs = new GridPostSortEventArgs(oldSortOrder, this.sortOrder, columnIndex);
            gridHandlerEventCaller.CallPostSortEvent(postSortEventArgs);
        }

        private void ClearAllSortGlyphDirection()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        public bool IsTopRow(int columnIndex, int rowIndex)
        {
            //Check column index to avoid 0xffffffff (Top left square corner!)
            return comparer != null && GridUtils.IsColumnValid(dataGridView, columnIndex) && rowIndex == -1;
        }

        public bool EventCellPainting_DrawText(Rectangle cellBounds, Graphics? graphics, DataGridViewCellStyle? cellStyle, int rowIndex, int columnIndex, object? formattedValue)
        {
            if (rowIndex != -1 || !GridUtils.IsColumnValid(dataGridView, columnIndex) || graphics == null || cellStyle == null)
            {
                return false;
            }

            TextRenderer.DrawText(graphics, string.Format("{0}", formattedValue), cellStyle.Font, cellBounds, cellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            var column = dataGridView.Columns[columnIndex];
            if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
            {
                var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                TextRenderer.DrawText(graphics, sortIcon, cellStyle.Font, cellBounds, SortIconColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }

            return true;
        }

        /*
        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest request, bool backgroundRequested)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;
            var style = args.CellStyle;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;
            if (rowIndex != -1 || columnIndex < 0 || columnIndex >= this.DataGridView.ColumnCount || graphics == null)
            {
                return;
            }

            if (backgroundRequested)
            {
                args.PaintBackground(args.ClipBounds, false);
            }

            TextRenderer.DrawText(graphics, string.Format("{0}", args.FormattedValue), style.Font, bounds, style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            var column = this.DataGridView.Columns[columnIndex];
            if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
            {
                var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, SortIconColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }

        public bool PaintContent(DataGridViewCellPaintingEventArgs args, bool backgroundPainter)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;
            var style = args.CellStyle;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            if (rowIndex != -1 || columnIndex < 0 || columnIndex >= this.DataGridView.ColumnCount || graphics == null)
            {
                return false;
            }

            TextRenderer.DrawText(graphics, string.Format("{0}", args.FormattedValue), style.Font, bounds, style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            
            var column = this.DataGridView.Columns[columnIndex];
            if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
            {
                var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, SortIconColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }

            return true;
        }

        public bool PaintBackground(DataGridViewCellPaintingEventArgs args, bool backgroundPainter) => false;

        public bool PaintBorder(DataGridViewCellPaintingEventArgs args, bool backgroundPainter) => false;*/
    }
}
