using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmTemplateConfiguration : ObservableConfiguration
    {
        [JsonProperty] public bool StandaloneAlarms { get => this.GetAs<bool>(); set => this.Set(value); }

        public AlarmTemplateConfiguration()
        {
            this.StandaloneAlarms = false;
        }
    }
}
