using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.Generation.UserAlarms
{
    public class DeviceData : IGridData
    {
        [JsonProperty] public string Address { get; set; }
        [JsonProperty] public string Description { get; set; }

        public void CopyFrom(DeviceData data)
        {
            this.Address = data.Address;
            this.Description = data.Description;
        }

        public void Clear()
        {
            this.Address = this.Description = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Address) && string.IsNullOrEmpty(this.Description);
        }
    }
}
