using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.Generation.UserAlarms
{
    public class AlarmData : IGridData
    {
        [JsonProperty] public bool Enable {  get; set; }
        [JsonProperty] public string AlarmVariable { get; set; }
        [JsonProperty] public string CoilAddress { get; set; }
        [JsonProperty] public string SetCoilAddress { get; set; }
        [JsonProperty] public string TimerAddress { get; set; }
        [JsonProperty] public string TimerType { get; set; }
        [JsonProperty] public string TimerValue { get; set; }
        [JsonProperty] public string Description { get; set; }


        public void CopyFrom(AlarmData data)
        {
            this.AlarmVariable = data.AlarmVariable;
            this.CoilAddress = data.CoilAddress;
            this.SetCoilAddress = data.SetCoilAddress;
            this.TimerAddress = data.TimerAddress;
            this.TimerType = data.TimerType;
            this.TimerValue = data.TimerValue;
            this.Description = data.Description;
            this.Enable = data.Enable;
        }

        public void Clear()
        {
            this.AlarmVariable = this.CoilAddress = this.SetCoilAddress = this.TimerAddress = this.TimerType = this.TimerValue = this.Description = "";
            this.Enable = true;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmVariable) && string.IsNullOrEmpty(this.CoilAddress) && string.IsNullOrEmpty(this.SetCoilAddress) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.Description);
        }
    }
}
