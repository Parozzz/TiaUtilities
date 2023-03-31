using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace SpinXmlReader.Block
{
    public static class PartParser
    {
        public static Part Parse(XmlNode node)
        {
            var parseOK = uint.TryParse(node.Attributes["UId"]?.Value, out uint uid);
            parseOK &= Util.TryNotNull(node.Attributes["Name"]?.Value, out string name);
            if (!parseOK)
            {
                return null;
            }

            Part part = null;
            switch (name)
            {
                case "Contact":
                    part = new ContactPart()
                    {
                        UId = uid,
                        OperandNegated = XmlSearchEngine.Of(node).AddSearch("Negated").AttributeRequired("Name", "operand").HasAnyNode()
                    };
                    break;
                case "Coil":
                    part = new CoilPart()
                    {
                        UId = uid,
                        OperandNegated = XmlSearchEngine.Of(node).AddSearch("Negated").AttributeRequired("Name", "operand").HasAnyNode()
                    };
                    break;
                case "Move":
                    break;
                case "Add":
                    break;
            }

            return part;
        }
    }
}
