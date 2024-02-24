using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.GenerationForms.IO
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

        public SimaticTagAddress GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(address);
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
