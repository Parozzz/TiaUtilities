using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public class AlarmGenTemplateSave
    {
        [JsonProperty] public string Name { get; set; } = "GenericTemplate";
        [JsonProperty] public GridSave<AlarmData> AlarmGrid { get; set; } = new();
        [JsonProperty] public AlarmTemplateConfiguration TemplateConfig { get; set; } = new();
    }
}
