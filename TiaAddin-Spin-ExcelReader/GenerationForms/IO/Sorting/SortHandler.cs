using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.GenerationForms.IO.Data;
using TiaXmlReader.UndoRedo;
using static TiaXmlReader.GenerationForms.IO.IOGenerationCellPaintHandler;

namespace TiaXmlReader.GenerationForms.IO.Sorting
{
    public class SortHandler : IOGenerationCellPainter
    {
        private readonly IOGenerationDataSource dataSource;
        private readonly DataGridView dataGridView;
        private readonly UndoRedoHandler undoRedoHandler;

        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<IOData, int> noSortSnapshot;

        public SortHandler(IOGenerationDataSource dataSource, DataGridView dataGridView, UndoRedoHandler undoRedoHandler)
        {
            this.dataSource = dataSource;
            this.dataGridView = dataGridView;
            this.undoRedoHandler = undoRedoHandler;
        }

        public void Init()
        {
            this.dataGridView.ColumnHeaderMouseClick += (sender, args) =>
            {
                if (args.ColumnIndex == 0)
                {
                    NextAddressColumnSort();
                }
            };
        }
        private void NextAddressColumnSort()
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

            AddressColumnSort(oldSortOrder);
        }

        private void AddressColumnSort(SortOrder oldSortOrder)
        {
            var snapDict = dataSource.CreateDataListSnapshot();

            if (sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortSnapshot = snapDict;
            }

            dataGridView.Columns[0].HeaderCell.SortGlyphDirection = sortOrder;
            if (sortOrder == SortOrder.None)
            {
                if (noSortSnapshot != null)
                {
                    dataSource.RestoreDataListSnapshot(noSortSnapshot);
                    noSortSnapshot = null;
                }
            }
            else
            {
                dataSource.Sort(new AddressColumnComparer(sortOrder), sortOrder);
            }

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                dataGridView.Columns[0].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = dataSource.CreateDataListSnapshot();
                dataSource.RestoreDataListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => dataSource.RestoreDataListSnapshot(undoSnap));
            });
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintResult = new PaintRequest();

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;
            return rowIndex == -1 && columnIndex == 0 ? paintResult.Content().Background() : paintResult;
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest request, bool backgroundRequested)
        {
            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;
            if (rowIndex != -1 || columnIndex != 0)
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

            var column = dataGridView.Columns[0];
            if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
            {
                var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, IOGenerationForm.SORT_ICON_COLOR, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }

    }
}
