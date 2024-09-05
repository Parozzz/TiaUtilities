using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GenForms.IO;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridSave<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        [JsonProperty] public Dictionary<int, T> RowData { get; set; } = [];
    }
}
