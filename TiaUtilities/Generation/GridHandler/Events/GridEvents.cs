using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler.Events;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Events
{
    public class GridEvents
    {
        public event GridCellChangeEventHandler CellChange = delegate { };

        public event GridScriptShowVariableEventHandler ScriptShowVariable = delegate {};
        public event GridScriptAddVariableEventHandler ScriptAddVariables = delegate { };

        public event GridScriptLoadEventHandlers ScriptLoad = delegate { };
        public event GridScriptChangedEventHandlers ScriptChanged = delegate { };


        public void CellChangeEvent(GridCellChangeEventArgs args)
        {
            CellChange(args);
        }

        public void ScriptShowVariableEvent(GridScriptShowVariableEventArgs args)
        {
            ScriptShowVariable(args);
        }

        public void ScriptAddVariablesEvent(GridScriptAddVariableEventArgs args)
        {
            ScriptAddVariables(args);
        }

        public void ScriptLoadEvent(GridScriptEventArgs args)
        {
            ScriptLoad(args);
        }

        public void ScriptChangedEvent(GridScriptEventArgs args)
        {
            ScriptChanged(args);
        }
    }
}
