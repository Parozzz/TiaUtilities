using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinAddIn.BlockData
{
    public enum AccessScopeEnum
    {
        LOCALVARIABLE,
        LOCALCONSTANT,
        GLOBALVARIABLE,
        TYPEDCONSTANT,
        LITERALCONSTANT,
    }

    public static class AccessParser
    {
        public static Access ParseXMLNode(XmlNode node)
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
                    access = new LocalVariableAccess();

                    var symbolNode = node.SelectSingleNode(".//Symbol");
                    var componentNodeList = symbolNode?.SelectNodes(".//Component");
                    if (componentNodeList != null)
                    {
                        StringBuilder symbolBuilder = new StringBuilder();
                        foreach (XmlNode componentNode in componentNodeList)
                        {
                            symbolBuilder.Append('.').Append(componentNode.Attributes["Name"].Value).Append('.');
                        }
                        symbolBuilder.Remove(0, 1).Remove(symbolBuilder.Length - 1, 1); //Remove the initial and final dot

                        ((LocalVariableAccess)access).Symbol = symbolBuilder.ToString();
                    }

                    break;
                case AccessScopeEnum.TYPEDCONSTANT:
                    access = new TypedConstantAccess();

                    var constantNode = node.SelectSingleNode(".//Constant");
                    var constantValueNode = constantNode?.SelectSingleNode(".//ConstantValue");
                    ((TypedConstantAccess)access).ConstantValue = constantValueNode?.Value;

                    break;
            }

            if (access != null)
            {
                access.UId = parseUID;
                access.Scope = parseScope;
            }

            return access;
        }

    }

    public class Access : UIdObject
    {
        public AccessScopeEnum Scope { get; protected internal set; }

        public Access()
        {

        }

    }
}
