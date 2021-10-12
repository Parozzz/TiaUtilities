using System;
using System.Text;
using System.Xml;

namespace SpinAddIn.BlockData
{
    public class FlagNet
    {
        internal FlagNet()
        {

        }

        public class Part : UIdObject
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

            public string Name {  get; protected set; }

            internal Part()
            {

            }
        }
    }
}
