using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.GenerationForms.IO.Data;

namespace TiaXmlReader.GenerationForms.IO.Sorting
{

    public class AddressColumnComparer : IComparer<IOGenerationData>
    {
        //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        private readonly int sortOrderModifier;
        public AddressColumnComparer(SortOrder sortOrder)
        {
            sortOrderModifier = (sortOrder == SortOrder.Ascending ? -1 : 1);
        }

        public int Compare(IOGenerationData x, IOGenerationData y)
        {
            var tagX = x?.GetTagAddress();
            var tagY = y?.GetTagAddress();
            if (tagX == null && tagY == null)
            {
                return 0;
            }
            else if (tagX == null)
            {
                return -1 * sortOrderModifier;
            }
            else if (tagY == null)
            {
                return 1 * sortOrderModifier;
            }
            else
            {
                return tagX.CompareTo(tagY);
            }
        }
    }
}
