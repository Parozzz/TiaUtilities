using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Events
{
    public class GridEvents<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        public event GridCellChangeEventHandler<C, T> CellChange = delegate { };

        public void CellChangeEvent(GridHandler<C, T> gridHandler, GridCellChangeEventArgs args)
        {
            CellChange(gridHandler, args);
        }
    }
}
