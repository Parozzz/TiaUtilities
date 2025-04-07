using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public class AlarmGenTemplateSave
    {
        [JsonProperty] public string Name { get; set; } = "GenericTemplate";
        [JsonProperty] public GridSave<AlarmData> AlarmGrid { get; set; } = new();
    }
}
