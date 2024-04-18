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
        public string Name {  get; set; }
        public string Description { get; set; }

        public void CopyFrom(DeviceData data)
        {
            this.Name = data.Name;
            this.Description = data.Description;
        }

        public void Clear()
        {
            this.Name = this.Description = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Description);
        }
    }
}
