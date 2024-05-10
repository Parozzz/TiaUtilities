using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Events
{
    public delegate void GridScriptShowVariableEventHandler<C, T>(GridHandler<C, T> sender, GridScriptShowVariableEventArgs args) where C : IGenerationConfiguration where T : IGridData<C>;

    public class GridScriptShowVariableEventArgs : EventArgs
    {
        public List<string> VariableList { get; set; } = new List<string>();
    }
}
