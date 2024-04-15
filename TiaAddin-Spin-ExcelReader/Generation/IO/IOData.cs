using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.Generation
{
    public class IOData
    {
        public string Address { get; set; }
        public string IOName { get; set; }
        public string DBName { get; set; }
        public string Variable { get; set; }
        public string Comment {  get; set; }

        public void LoadDefaults(IOConfiguration config)
        {
            if(string.IsNullOrEmpty(IOName))
            {
                IOName = config.DefaultIoName;
            }

            if(string.IsNullOrEmpty(Variable))
            {
                Variable = config.DefaultVariableName;
            }
        }

        public void ParsePlaceholders(GenerationPlaceholders placeholders)
        {
            Address = placeholders.Parse(Address);
            IOName = placeholders.Parse(IOName);
            Variable = placeholders.Parse(Variable);
            DBName = placeholders.Parse(DBName);
            Comment = placeholders.Parse(Comment);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(DBName) && string.IsNullOrEmpty(Variable);
        }

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

        public SimaticTagAddress GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(Address);
        }

        public IOData Clone()
        {
            return new IOData()
            {
                Address = this.Address,
                IOName = this.IOName,
                DBName = this.DBName,
                Variable = this.Variable,
                Comment = this.Comment
            };
        }
    }
}
