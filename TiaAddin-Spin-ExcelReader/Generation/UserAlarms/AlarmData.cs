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
        public string AlarmAddress { get; set; }
        public string CoilAddress { get; set; }
        public string SetCoilAddress { get; set; }
        public string TimerAddress { get; set; }
        public string TimerType {  get; set; }
        public string TimerValue {  get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }

        public void CopyFrom(AlarmData data)
        {
            this.AlarmAddress = data.AlarmAddress;
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
            this.AlarmAddress = this.CoilAddress = this.SetCoilAddress = this.TimerAddress = this.TimerType = this.TimerValue = this.Description = "";
            this.Enable = true;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmAddress) && string.IsNullOrEmpty(this.CoilAddress) && string.IsNullOrEmpty(this.SetCoilAddress) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.Description);
        }
    }
}
