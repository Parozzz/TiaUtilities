using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    internal class FlagNetParser
    {
        private readonly FlagNet flagNet;
        private readonly XmlNamespaceManager netNamespace;
        public FlagNetParser(FlagNet flagNet, XmlNamespaceManager netNamespace)
        {
            this.flagNet = flagNet;
            this.netNamespace = netNamespace;
        }

        public Access ParseAccessNode(XmlNode node)
        {
            bool parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["UId"]?.Value, out var parseUID);
            parseOK &= Enum.TryParse(node.Attributes["Scope"]?.Value, true, out ScopeEnum parseScope);
            if (!parseOK)
            {
                return null;
            }

            Access access = null;
            switch (parseScope)
            {
                case ScopeEnum.LOCALVARIABLE:
                case ScopeEnum.GLOBALVARIABLE:
                    access = new LocalVariableAccess();

                    StringBuilder symbolBuilder = new StringBuilder();
                    foreach (XmlNode componentNode in node.SelectNodes("net:Symbol/net:Component", netNamespace))
                    {
                        symbolBuilder.Append('.').Append(componentNode.Attributes["Name"].Value).Append('.');
                    }
                    symbolBuilder.Remove(0, 1).Remove(symbolBuilder.Length - 1, 1); //Remove the initial and final dot

                    ((LocalVariableAccess)access).Symbol = symbolBuilder.ToString();

                    break;
                case ScopeEnum.TYPEDCONSTANT:
                    access = new TypedConstantAccess();

                    var constantValueNode = node.SelectSingleNode("net:Constant/net:ConstantValue", netNamespace);
                    ((TypedConstantAccess)access).ConstantValue = constantValueNode?.InnerText;

                    break;
                case ScopeEnum.LITERALCONSTANT:
                    access = new LiteralConstantAccess();

                    ((LiteralConstantAccess)access).ConstantType = node.SelectSingleNode("net:Constant/net:ConstantType", netNamespace)?.InnerText ?? "";
                    ((LiteralConstantAccess)access).ConstantValue = node.SelectSingleNode("net:Constant/net:ConstantValue", netNamespace)?.InnerText ?? "";

                    break;
            }

            if (access != null)
            {
                access.UId = parseUID;
                access.Scope = parseScope;
            }

            return access;
        }

        public Part ParsePartNode(XmlNode node)
        {
            bool parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["UId"]?.Value, out var parsedUID);
            if (!parseOK)
            {
                return null;
            }

            var name = node.Attributes["Name"].Value;

            return new Part()
            {
                UId = parsedUID,
                Name = name
            };
        }

        public Wire ParseWireNode(FlagNet flagNet, XmlNode node)
        {
            var parsedOk = true;
            parsedOk &= uint.TryParse(node.Attributes["UId"]?.Value, out uint parsedUID);
            if(!parsedOk)
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
                    UId = parsedUID
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
                    UId = parsedUID
                };
            }
        }

        private WirePart ParseWirePartNode(FlagNet flagNet, XmlNode node)
        {
            if (!Enum.TryParse(node.Name.ToUpper(), out WirePartType type))
            {
                MessageBox.Show("Unknown WirePart node named " + node.Name);
                return null;
            }

            if (type == WirePartType.POWERRAIL)
            {
                return new WirePart(WirePartType.POWERRAIL, null, "Powerrail");
            }

            bool parsedOK = true;
            parsedOK &= uint.TryParse(node.Attributes["UId"]?.Value, out uint parsedUId);

            var uidObject = flagNet.GetUIdObject(parsedUId);
            parsedOK &= uidObject != null;
            if (!parsedOK)
            {
                return null;
            }

            var name = node.Attributes["Name"]?.Value ?? "";
            return new WirePart(type, uidObject, name);
        }
    }
}
