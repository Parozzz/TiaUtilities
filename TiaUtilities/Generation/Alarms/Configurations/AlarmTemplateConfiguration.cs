using Newtonsoft.Json;
using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.Alarms.Configurations
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
