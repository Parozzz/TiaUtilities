using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public interface IGridRowComparer<T> : IComparer<T> where T : IGridData
    {
        void SetSortOrder(SortOrder sortOrder);
        SortOrder GetSortOrder();

        bool CanSortColumn(int column);

        void SetSortedColumn(int column);
    }
}
