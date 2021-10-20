using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            parseOK &= uint.TryParse(node.Attributes["UId"].Value, out var parseUID);
            parseOK &= Enum.TryParse(node.Attributes["Scope"].Value, true, out AccessScopeEnum parseScope);
            if (!parseOK)
            {
                return null;
            }

            Access access = null;
            switch (parseScope)
            {
                case AccessScopeEnum.LOCALVARIABLE:
                case AccessScopeEnum.GLOBALVARIABLE:
                    access = new LocalVariableAccess();

                    StringBuilder symbolBuilder = new StringBuilder();
                    foreach (XmlNode componentNode in node.SelectNodes("net:Symbol/net:Component", netNamespace))
                    {
                        symbolBuilder.Append('.').Append(componentNode.Attributes["Name"].Value).Append('.');
                    }
                    symbolBuilder.Remove(0, 1).Remove(symbolBuilder.Length - 1, 1); //Remove the initial and final dot

                    ((LocalVariableAccess)access).Symbol = symbolBuilder.ToString();

                    break;
                case AccessScopeEnum.TYPEDCONSTANT:
                    access = new TypedConstantAccess();

                    var constantValueNode = node.SelectSingleNode("net:Constant/net:ConstantValue", netNamespace);
                    ((TypedConstantAccess)access).ConstantValue = constantValueNode?.InnerText;

                    break;
                case AccessScopeEnum.LITERALCONSTANT:
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
            parseOK &= uint.TryParse(node.Attributes["UId"].Value, out var parseUID);
            if (!parseOK)
            {
                return null;
            }

            var name = node.Attributes["Name"].Value;

            return new Part()
            {
                UId = parseUID,
                Name = name
            };
        }
    }
}
