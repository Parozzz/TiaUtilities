using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler
{
    public interface IGridRowComparer<T> : IComparer<T> where T : IGridData
    {
        void SetSortOrder(SortOrder sortOrder);

        SortOrder GetSortOrder();

        bool CanSortColumn(int column);

        void SetSortedColumn(int column);
    }
}
