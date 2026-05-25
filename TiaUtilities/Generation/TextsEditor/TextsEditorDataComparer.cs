using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.IO.Data;

namespace TiaUtilities.Generation.TextsEditor
{
    public class TextsEditorDataComparer : IGridRowComparer<TextsEditorData>
    {
        private SortOrder sortOrder;
        private int sortedColumn;
        public TextsEditorDataComparer()
        {
        }

        public bool CanSortColumn(int column)
        {
            return true;
        }

        public SortOrder GetSortOrder()
        {
            return sortOrder;
        }

        public void SetSortOrder(SortOrder sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        public void SetSortedColumn(int column)
        {
            sortedColumn = column;
        }

        private int GetModifier() //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        {
            return sortOrder == SortOrder.Ascending ? -1 : 1;
        }

        public int Compare(TextsEditorData? x, TextsEditorData? y)
        {
            var xValue = x[sortedColumn];
            var yValue = y[sortedColumn];

            var xIsString = xValue is string;
            var yIsString = yValue is string;

            if (!xIsString && !yIsString)
            {
                return 0;
            }
            else if (xValue == null || !xIsString)
            {
                return -1 * GetModifier();
            }
            else if (yValue == null || !yIsString)
            {
                return 1 * GetModifier(); ;
            }
            else
            {
                return xValue.ToString().CompareTo(yValue.ToString());
            }
        }
    }
}