using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.GridHandler.Events
{
    public delegate void GridScriptLoadEventHandlers(GridScriptEventArgs args);
    public delegate void GridScriptChangedEventHandlers(GridScriptEventArgs args);
    public class GridScriptEventArgs : EventArgs
    {
        public string Script { get; set; } = "";
    }

    public delegate void GridScriptAddVariableEventHandler(GridScriptAddVariableEventArgs args);
    public class GridScriptAddVariableEventArgs : EventArgs
    {
        public Dictionary<string, object> VariableDict { get; set; } = [];
    }

    public delegate void GridScriptShowVariableEventHandler(GridScriptShowVariableEventArgs args);
    public class GridScriptShowVariableEventArgs : EventArgs
    {
        public List<string> VariableList { get; set; } = [];
    }
}
