using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.GridHandler.CustomColumns
{
    public interface IGridCustomColumn
    {
        void RegisterEvents(DataGridView dataGridView);

        void UnregisterEvents(DataGridView dataGridView);

        bool ProcessCmdKey(ref Message msg, Keys keyData);
    }
}
