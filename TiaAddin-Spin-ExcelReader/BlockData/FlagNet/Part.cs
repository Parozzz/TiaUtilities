using SpinAddIn.BlockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinAddIn.BlockData
{
    public static class PartParser
    {
        public static Part ParseXMLNode(XmlNode node)
        {
            bool parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["UId"].Value, out var parseUID);

            var name = node.Attributes["Name"].Value;

            return new Part()
            {
                UId = parseUID,
                Name = name
            };
        }
    }

    public class Part : UIdObject
    {
        public string Name { get; internal protected set; }

        internal Part()
        {

        }
    }
}
