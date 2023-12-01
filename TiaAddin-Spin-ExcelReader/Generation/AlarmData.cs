using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.AlarmGeneration
{
    public class AlarmData
    {
        private readonly string consumerAddress;
        private readonly string coil1Address;
        private readonly string coil2Address;
        private readonly string description;
        private readonly bool enable;

        public AlarmData(string consumerAddress, string coil1Address, string coil2Address, string description, bool enable)
        {
            this.consumerAddress = consumerAddress;
            this.coil1Address = coil1Address;
            this.coil2Address = coil2Address;
            this.description = description;
            this.enable = enable;
        }

        public string GetConsumerAddress(Func<string, string> placeholderFunction)
        {
            return placeholderFunction.Invoke(consumerAddress);
        }

        public string GetCoil1Address(Func<string, string> placeholderFunction)
        {
            return placeholderFunction.Invoke(coil1Address);
        }

        public string GetCoil2Address(Func<string, string> placeholderFunction)
        {
            return placeholderFunction.Invoke(coil2Address);
        }

        public string GetDescription(Func<string, string> placeholderFunction)
        {
            return placeholderFunction.Invoke(description);
        }

        public bool GetEnable()
        {
            return enable;
        }
    }
}
