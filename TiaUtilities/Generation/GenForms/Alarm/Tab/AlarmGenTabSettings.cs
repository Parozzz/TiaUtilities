using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.Alarms;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTabSettings
    {
        [JsonProperty] public AlarmTabConfiguration TabConfiguration { get; set; } = new();
        [JsonProperty] public string DeviceJSScript { get; set; } = "";
        [JsonProperty] public string AlarmJSScript { get; set; } = "";

        public AlarmGenTabSettings() { }
    }
}
