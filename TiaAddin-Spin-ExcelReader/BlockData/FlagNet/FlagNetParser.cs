using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace SpinXmlReader.Block
{
    internal static class FlagNetParser
    {
        public static Access ParseAccessNode(XmlNode node)
        {
            bool parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["UId"]?.Value, out var uid);
            parseOK &= Enum.TryParse(node.Attributes["Scope"]?.Value, true, out ScopeEnum scope);
            if (!parseOK)
            {
                return null;
            }

            Access access = null;
            switch (scope)
            {
                case ScopeEnum.LOCALVARIABLE:
                case ScopeEnum.GLOBALVARIABLE:
                    access = VariableAccess.Parse(node);
                    break;
                case ScopeEnum.TYPEDCONSTANT:
                    access = TypedConstantAccess.Parse(node);
                    break;
                case ScopeEnum.LITERALCONSTANT:
                    access = LiteralConstantAccess.Parse(node);
                    break;
            }

            if (access != null)
            {
                access.UId = uid;
                access.Scope = scope;
            }

            return access;
        }

        public static Part ParsePartNode(XmlNode node)
        {
            bool parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["UId"]?.Value, out var uid);
            if (!parseOK)
            {
                return null;
            }

            var part = Part.Parse(node);
            if(part == null)
            {
                return null;
            }

            part.UId = uid;
            return part;
        }

        public static Wire ParseWireNode(FlagNet flagNet, XmlNode node)
        {
            var parseOK = uint.TryParse(node.Attributes["UId"]?.Value, out uint uid);
            if(!parseOK)
            {
                return null;
            }

            var firstWirePart = ParseWirePartNode(flagNet, node.ChildNodes[0]);
            if (firstWirePart == null)
            {
                return null;
            }

            if (firstWirePart.GetWirePart() == WirePartType.POWERRAIL)
            {
                var powerrrailWire = new PowerrailWire()
                {
                    UId = uid
                };

                for (int x = 1; x < node.ChildNodes.Count; x++)
                {
                    var partNode = ParseWirePartNode(flagNet, node.ChildNodes[x]);
                    if (partNode != null)
                    {
                        powerrrailWire.AddWirePart(partNode);
                    }
                }
                return powerrrailWire;
            }
            else
            {
                var secondWirePart = ParseWirePartNode(flagNet, node.ChildNodes[1]);
                if (secondWirePart == null)
                {
                    return null;
                }

                return new NormalWire(firstWirePart, secondWirePart)
                {
                    UId = uid
                };
            }
        }

        private static WirePart ParseWirePartNode(FlagNet flagNet, XmlNode node)
        {
            bool parseOK = Enum.TryParse(node.Name, true, out WirePartType type);
            parseOK &= uint.TryParse(node.Attributes["UId"]?.Value, out uint uid);
            parseOK &= Util.TryNotNull(node.Attributes["Name"]?.Value, out string name);
            if (!parseOK)
            {
                return null;
            }

            if (type == WirePartType.POWERRAIL)
            {
                return new WirePart(WirePartType.POWERRAIL, null, "Powerrail");
            }

            var uidObject = flagNet.GetUIdObject(uid);
            if (uidObject == null)
            {
                return null;
            }

            return new WirePart(type, uidObject, name);
        }
    }
}
