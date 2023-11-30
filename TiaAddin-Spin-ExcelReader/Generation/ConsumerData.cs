using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation
{
    public class ConsumerData
    {
        private readonly string name;
        private readonly string dbName;

        public ConsumerData(string name, string dbName)        {
            this.name = name;
            this.dbName = dbName;
        }

        public string GetName()
        {
            return name;
        }

        public string GetDbName()
        {
            return dbName;
        }
    }
}
