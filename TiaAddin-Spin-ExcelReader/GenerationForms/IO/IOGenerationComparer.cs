using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.GenerationForms.IO
{

    public class IOGenerationComparer : IGridRowComparer<IOData>
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
            if (sortedColumn == IOGenerationForm.ADDRESS_COLUMN)
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
                var stringX = x[sortedColumn];
                var stringY = y[sortedColumn];
                if (stringX == null && stringY == null)
                {
                    return 0;
                }
                else if (stringX == null)
                {
                    return -1 * this.GetModifier();
                }
                else if (stringY == null)
                {
                    return 1 * this.GetModifier(); ;
                }
                else
                {
                    return stringX.CompareTo(stringY);
                }
            }
        }


    }
}
