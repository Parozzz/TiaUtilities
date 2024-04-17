using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.UserAlarms
{
    public class AlarmData
    {
        public string AlarmAddress { get; set; }
        public string CoilAddress { get; set; }
        public string SetCoilAddress { get; set; }
        public string TimerAddress { get; set; }
        public string TimerType {  get; set; }
        public string TimerValue {  get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}
