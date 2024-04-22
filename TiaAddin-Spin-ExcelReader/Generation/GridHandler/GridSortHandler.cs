using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.UndoRedo;
using static TiaXmlReader.Generation.GridHandler.GridCellPaintHandler;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridSortHandler<C, T> : IGridCellPainter where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridDataSource<C, T> dataSource;
        private readonly DataGridView dataGridView;
        private readonly UndoRedoHandler undoRedoHandler;
        private readonly IGridRowComparer<C, T> comparer;

        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<T, int> noSortIndexSnapshot;

        public Color SortIconColor { get; set; } = Color.Green;

        public GridSortHandler(DataGridView dataGridView, GridDataSource<C, T> dataSource, UndoRedoHandler undoRedoHandler, IGridRowComparer<C, T> comparer)
        {
            this.dataGridView = dataGridView;
            this.dataSource = dataSource;
            this.undoRedoHandler = undoRedoHandler;
            this.comparer = comparer;
        }

        public void Init()
        {
            this.dataGridView.ColumnHeaderMouseClick += (sender, args) =>
            {
                if (this.comparer == null)
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
            var oldSortOrder = sortOrder;
            switch (sortOrder)
            {
                case SortOrder.None:
                    sortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    sortOrder = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                    sortOrder = SortOrder.None;
                    break;
            }

            ColumnSort(oldSortOrder, columnIndex);
        }

        private void ColumnSort(SortOrder oldSortOrder, int columnIndex)
        {
            var snapDict = dataSource.CreateIndexListSnapshot();

            if (sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortIndexSnapshot = snapDict;
            }

            ClearAllSortGlyphDirection();
            dataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = sortOrder;

            if (sortOrder == SortOrder.None)
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
                comparer.SetSortOrder(sortOrder);
                dataSource.Sort(comparer, sortOrder);
            }

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                dataGridView.Columns[columnIndex].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = dataSource.CreateIndexListSnapshot();
                dataSource.RestoreIndexListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => dataSource.RestoreIndexListSnapshot(undoSnap));
            });
        }

        private void ClearAllSortGlyphDirection()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
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
            if (columnIndex < 0 || columnIndex >= dataGridView.ColumnCount) //To avoid 0xffffffff (Top left square corner!)
            {
                return paintResult;
            }

            return args.RowIndex == -1 ? paintResult.Content().Background() : paintResult;
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest request, bool backgroundRequested)
        {
            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;
            if (rowIndex != -1 || columnIndex < 0 || columnIndex >= dataGridView.ColumnCount)
            {
                return;
            }

            if (backgroundRequested)
            {
                args.PaintBackground(args.ClipBounds, false);
            }

            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var style = args.CellStyle;

            TextRenderer.DrawText(graphics, string.Format("{0}", args.FormattedValue), style.Font, bounds, style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            var column = dataGridView.Columns[columnIndex];
            if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
            {
                var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, SortIconColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }

    }
}
