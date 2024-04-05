using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.GenerationForms.IO.Data
{
    public class IOGenerationData
    {
        public string Address { get => address; set => address = value; }
        private string address;

        public string IOName { get => ioName; set => ioName = value; }
        private string ioName;

        public string DBName { get => dbName; set => dbName = value; }
        private string dbName;

        public string Variable { get => variable; set => variable = value; }
        private string variable;

        public string Comment { get => comment; set => comment = value; }
        private string comment;

        public IOGenerationData()
        {

        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(DBName) && string.IsNullOrEmpty(Variable);
        }

        public SimaticTagAddress GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(address);
        }

        public IOData CreateIOData()
        {
            return new IOData()
            {
                IOAddress = address,
                IOName = IOName,
                VariableName = Variable,
                DBName = DBName,
                Comment = comment
            };
        }

        public IOGenerationData Clone()
        {
            return new IOGenerationData()
            {
                Address = Address,
                IOName = IOName,
                DBName = DBName,
                Variable = Variable,
                Comment = Comment
            };
        }
    }

}
