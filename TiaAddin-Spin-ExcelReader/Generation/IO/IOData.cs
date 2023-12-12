using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.Generation
{
    public class IOData
    {
        public String Address { get; set; }
        public String IOTagName { get; set; }
        public String IOTagComment { get; set; }
        public String VariableAddress { get; set; }
        public String VariableComment { get; set; }

        public SimaticMemoryArea GetMemoryArea()
        {
            return SimaticMemoryAreaUtil.GetFromAddress(Address);
        }

        public uint GetAddressBit()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < Address.Length; x++)
            {
                var c = Address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = Address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[1]);
        }

        public uint GetAddressByte()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < Address.Length; x++)
            {
                var c = Address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = Address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[0]);
        }
    }
}
