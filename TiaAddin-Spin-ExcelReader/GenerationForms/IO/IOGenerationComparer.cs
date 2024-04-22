using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.GenerationForms.IO
{

    public class IOGenerationComparer : IGridRowComparer<IOConfiguration, IOData>
    {
        private SortOrder sortOrder;
        private int sortedColumn;
        public IOGenerationComparer()
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
            this.sortedColumn = column;
        }

        private int GetModifier() //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        {
            return sortOrder == SortOrder.Ascending ? -1 : 1;
        }

        public int Compare(IOData x, IOData y)
        {
            if (sortedColumn == IOData.ADDRESS.ColumnIndex)
            {
                var tagX = x?.GetTagAddress();
                var tagY = y?.GetTagAddress();
                if (tagX == null && tagY == null)
                {
                    return 0;
                }
                else if (tagX == null)
                {
                    return -1 * this.GetModifier();
                }
                else if (tagY == null)
                {
                    return 1 * this.GetModifier(); ;
                }
                else
                {
                    return tagX.CompareTo(tagY);
                }
            }
            else
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
                    return -1 * this.GetModifier();
                }
                else if (yValue == null || !yIsString)
                {
                    return 1 * this.GetModifier(); ;
                }
                else
                {
                    return xValue.ToString().CompareTo(yValue.ToString());
                }
            }
        }


    }
}
