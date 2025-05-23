﻿using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler.JSScript;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenSave
    {
        [JsonProperty] public GridScriptSave ScriptSave { get; set; } = new();

        [JsonProperty] public AlarmMainConfiguration AlarmMainConfig { get; set; } = new();
        [JsonProperty] public List<AlarmGenTabSave> TabSaves { get; set; } = [];
        [JsonProperty] public List<AlarmGenTemplateSave> TemplateSaves { get; set; } = [];
    }
}
