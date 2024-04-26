using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Alarms.GenerationForm;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO.GenerationForm;
using TiaXmlReader.AutoSave;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public class AlarmGenerationSettings : ISettingsAutoSave
    {
        [JsonProperty] public AlarmConfiguration Configuration {  get; set; } = new AlarmConfiguration();
        [JsonProperty] public string DeviceJSScript { get; set; } = "";
        [JsonProperty] public string AlarmJSScript { get; set; } = "";

        public AlarmGenerationSettings() { }
    }
}
