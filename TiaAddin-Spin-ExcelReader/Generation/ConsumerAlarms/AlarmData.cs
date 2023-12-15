using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.AlarmGeneration
{
    public class AlarmData
    {
        public string ConsumerAddress { get; set; }
        public string CoilAddress { get; set; }
        public string SetCoilAddress { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}
