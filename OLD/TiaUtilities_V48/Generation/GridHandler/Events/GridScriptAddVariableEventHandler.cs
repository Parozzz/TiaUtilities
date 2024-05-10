using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Events
{

    public delegate void GridScriptAddVariableEventHandler<C, T>(GridHandler<C, T> sender, GridScriptAddVariableEventArgs args) where C : IGenerationConfiguration where T : IGridData<C>;

    public class GridScriptAddVariableEventArgs : EventArgs
    {
        public Dictionary<string, object> VariableDict { get; set; } = new Dictionary<string, object>();
    }
}
