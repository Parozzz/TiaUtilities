using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaUtilities.Generation.GridHandler.JSScript;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public class AlarmGenSave : IGenProjectSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public GridScriptContainer.ContainerSave ScriptContainer { get; set; } = new();

        [JsonProperty] public AlarmMainConfiguration AlarmMainConfig { get; set; } = new();
        [JsonProperty] public List<AlarmGenTabSave> TabSaves { get; set; } = [];

        public static AlarmGenSave Load(ref string? filePath)
        {
            return GenUtils.Deserialize<AlarmGenSave>(ref filePath, EXTENSION) ?? new AlarmGenSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool saveAs = false)
        {
            return GenUtils.Save(this, ref filePath, EXTENSION, saveAs);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlarmGenSave compare &&
                TabSaves.SequenceEqual(compare.TabSaves);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
