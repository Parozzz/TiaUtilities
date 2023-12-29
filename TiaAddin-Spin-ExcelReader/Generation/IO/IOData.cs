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
        public string IOAddress { get; set; }
        public string IOName { get; set; }
        public string VariableName { get; set; }
        public string DBName {  get; set; }
        public string Comment {  get; set; }

        public void ParsePlaceholders(GenerationPlaceholders placeholders)
        {
            IOAddress = placeholders.Parse(IOAddress);
            IOName = placeholders.Parse(IOName);
            VariableName = placeholders.Parse(VariableName);
            DBName = placeholders.Parse(DBName);
            Comment = placeholders.Parse(Comment);
        }

        public SimaticMemoryArea GetMemoryArea()
        {
            return SimaticMemoryAreaUtil.GetFromAddress(IOAddress);
        }

        public uint GetAddressBit()
        {
            if (string.IsNullOrEmpty(IOAddress))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < IOAddress.Length; x++)
            {
                var c = IOAddress[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = IOAddress.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[1]);
        }

        public uint GetAddressByte()
        {
            if (string.IsNullOrEmpty(IOAddress))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < IOAddress.Length; x++)
            {
                var c = IOAddress[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = IOAddress.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[0]);
        }
    }
}
