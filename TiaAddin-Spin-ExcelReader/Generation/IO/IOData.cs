using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.IO;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Generation;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.SimaticML.Enums;

namespace TiaXmlReader.Generation.IO
{
    public class IOData : IGridData
    {
        [JsonProperty] public string Address {  get; set; }
        [JsonProperty] public string IOName {  get; set; }
        [JsonProperty] public string DBName {  get; set; }
        [JsonProperty] public string Variable {  get; set; }
        [JsonProperty] public string Comment { get; set; }

        public string this[int i]
        {
            get
            {
                switch (i)
                {
                    case IOGenerationForm.ADDRESS_COLUMN:
                        return Address;
                    case IOGenerationForm.IO_NAME_COLUMN:
                        return IOName;
                    case IOGenerationForm.DB_COLUMN:
                        return DBName;
                    case IOGenerationForm.VARIABLE_COLUMN:
                        return Variable;
                    case IOGenerationForm.COMMENT_COLUMN:
                        return Comment;
                    default:
                        throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }
            }

            set
            {
                switch (i)
                {
                    case IOGenerationForm.ADDRESS_COLUMN:
                        Address = value;
                        break;
                    case IOGenerationForm.IO_NAME_COLUMN:
                        IOName = value;
                        break;
                    case IOGenerationForm.DB_COLUMN:
                        DBName = value;
                        break;
                    case IOGenerationForm.VARIABLE_COLUMN:
                        Variable = value;
                        break;
                    case IOGenerationForm.COMMENT_COLUMN:
                        Comment = value;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid index for Set square bracket operator in IOData");
                }
            }
        }

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

        public void Clear()
        {
            this.Address = this.IOName = this.DBName = this.Variable = this.Comment = "";
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

        public void CopyFrom(IOData ioData)
        {
            this.Address = ioData.Address;
            this.IOName = ioData.IOName;
            this.DBName = ioData.DBName;
            this.Variable = ioData.Variable;
            this.Comment = ioData.Comment;
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
