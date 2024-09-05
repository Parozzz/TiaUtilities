using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.UndoRedo;
using static TiaXmlReader.Generation.GridHandler.GridCellPaintHandler;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaUtilities.Generation.GridHandler.Events;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridSortHandler<C, T> : IGridCellPainter where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridHandler<C, T> gridHandler;
        private readonly UndoRedoHandler undoRedoHandler;
        private readonly IGridRowComparer<C, T> comparer;

        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<T, int>? noSortIndexSnapshot;

        private DataGridView DataGridView { get => gridHandler.DataGridView; }
        private GridDataSource<C, T> DataSource { get => gridHandler.DataSource; }

        public Color SortIconColor { get; set; } = Color.Green;

        public GridSortHandler(GridHandler<C, T> gridHandler, UndoRedoHandler undoRedoHandler, IGridRowComparer<C, T> comparer)
        {
            this.gridHandler = gridHandler;
            this.undoRedoHandler = undoRedoHandler;
            this.comparer = comparer;
        }

        public void Init()
        {
            this.DataGridView.ColumnHeaderMouseClick += (sender, args) =>
            {
                if (this.comparer == null || args.Button == MouseButtons.Right)
                {
                    return;
                }

                if (this.comparer.CanSortColumn(args.ColumnIndex))
                {
                    NextColumnSort(args.ColumnIndex);
                }
            };
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
            this.gridHandler.Events.PreSortEvent(this.gridHandler.DataGridView, preSortEventArgs);

            if(preSortEventArgs.Handled)
            {
                return;
            }

            this.sortOrder = preSortEventArgs.SortOrder;

            var snapDict = this.DataSource.CreateIndexListSnapshot();
            if (this.sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortIndexSnapshot = snapDict;
            }

            ClearAllSortGlyphDirection();
            this.DataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = this.sortOrder;

            if (this.sortOrder == SortOrder.None)
            {
                if (noSortIndexSnapshot != null)
                {
                    this.DataSource.RestoreIndexListSnapshot(noSortIndexSnapshot);
                    noSortIndexSnapshot = null;
                }
            }
            else
            {
                comparer.SetSortedColumn(columnIndex);
                comparer.SetSortOrder(this.sortOrder);
                this.DataSource.Sort(comparer, this.sortOrder);
            }

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                this.DataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = this.DataSource.CreateIndexListSnapshot();
                this.DataSource.RestoreIndexListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => this.DataSource.RestoreIndexListSnapshot(undoSnap));
            });

            var postSortEventArgs = new GridPostSortEventArgs(oldSortOrder, this.sortOrder, columnIndex);
            this.gridHandler.Events.PostSortEvent(this.gridHandler.DataGridView,postSortEventArgs);
        }

        private void ClearAllSortGlyphDirection()
        {
            foreach (DataGridViewColumn column in this.DataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintResult = new PaintRequest();
            if (this.comparer == null)
            {
                return paintResult;
            }

            var columnIndex = args.ColumnIndex;
            if (columnIndex < 0 || columnIndex >= this.DataGridView.ColumnCount) //To avoid 0xffffffff (Top left square corner!)
            {
                return paintResult;
            }

            return args.RowIndex == -1 ? paintResult.Content().Background() : paintResult;
        }

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

    }
}
