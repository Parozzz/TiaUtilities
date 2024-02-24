using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.UndoRedo;

namespace TiaXmlReader.GenerationForms.IO
{
    public class SortHandler
    {
        private readonly IOGenerationForm form;
        private readonly DataGridView dataGridView;
        private readonly UndoRedoHandler undoRedoHandler;

        private SortOrder sortOrder = SortOrder.None;
        private Dictionary<IOGenerationData, int> noSortSnapshot;


        public SortHandler(IOGenerationForm form, DataGridView dataGridView, UndoRedoHandler undoRedoHandler)
        {
            this.form = form;
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

        public void PaintCell(DataGridViewCellPaintingEventArgs args)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            var style = args.CellStyle;

            if (rowIndex == -1)
            {
                if (columnIndex == 0)
                {
                    args.PaintBackground(bounds, false);

                    TextRenderer.DrawText(graphics, string.Format("{0}", args.FormattedValue), style.Font, bounds, style.ForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                    var column = dataGridView.Columns[0];
                    if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                    {
                        var sortIcon = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "▲" : "▼";
                        TextRenderer.DrawText(graphics, sortIcon, style.Font, bounds, IOGenerationForm.SORT_ICON_COLOR,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                    }

                    args.Handled = true;
                }
            }
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
            var snapDict = form.CreateDataListSnapshot();

            if (sortOrder != SortOrder.None && oldSortOrder == SortOrder.None)
            {
                this.noSortSnapshot = snapDict;
            }

            dataGridView.Columns[0].HeaderCell.SortGlyphDirection = sortOrder;
            if (sortOrder == SortOrder.None)
            {
                if (noSortSnapshot != null)
                {
                    form.RestoreDataListSnapshot(noSortSnapshot);
                    noSortSnapshot = null;
                }
            }
            else
            {
                form.Sort(new AddressColumnComparer(sortOrder), sortOrder);
            }

            undoRedoHandler.AddUndo(() =>
            {
                this.sortOrder = oldSortOrder;
                dataGridView.Columns[0].HeaderCell.SortGlyphDirection = oldSortOrder;

                var undoSnap = form.CreateDataListSnapshot();
                form.RestoreDataListSnapshot(snapDict);
                undoRedoHandler.AddRedo(() => form.RestoreDataListSnapshot(undoSnap));
            });
        }

    }
}
