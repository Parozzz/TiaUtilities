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

        public event GridScriptShowVariableEventHandler<C, T> ScriptShowVariable = delegate {};

        public event GridScriptAddVariableEventHandler<C, T> ScriptAddVariables = delegate { };

        public void CellChangeEvent(GridHandler<C, T> gridHandler, GridCellChangeEventArgs args)
        {
            CellChange(gridHandler, args);
        }

        public void ScriptShowVariableEvent(GridHandler<C, T> gridHandler, GridScriptShowVariableEventArgs args)
        {
            ScriptShowVariable(gridHandler, args);
        }

        public void ScriptAddVariablesEvent(GridHandler<C, T> gridHandler, GridScriptAddVariableEventArgs args)
        {
            ScriptAddVariables(gridHandler, args);
        }
    }
}
