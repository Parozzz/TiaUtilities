using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Template
{
    public class AlarmGenTemplateSave
    {
        [JsonProperty] public string Name { get; set; } = "GenericTemplate";
        [JsonProperty] public GridSave<TemplateData> AlarmGrid { get; set; } = new();
        [JsonProperty] public AlarmTemplateConfiguration TemplateConfig { get; set; } = new();
    }
}
