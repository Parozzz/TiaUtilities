using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimaticML;
using SimaticML.XMLClasses;

namespace SimaticML.XMLClasses
{
    public class UnknownXmlGlobalObject : XmlNodeConfiguration, IGlobalObject
    {
        private readonly GlobalObjectData globalObjectData;
        public UnknownXmlGlobalObject(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required, namespaceURI, defaultInnerText)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
    }
}
