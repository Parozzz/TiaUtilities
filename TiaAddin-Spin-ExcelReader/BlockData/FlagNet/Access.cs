using SpinXmlReader;
using System;
using System.Xml;
using TiaXmlReader.Utility;

namespace TiaXmlReader
{
    public class Access : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Access";

        public enum Type
        {
            LOCAL_VARIABLE,
            LOCAL_CONSTANT,
            LITERAL_CONSTANT,
            GLOBAL_CONSTANT,
            GLOBAL_VARIABLE,
        }

        private readonly LocalObjectData localObjectData;

        private readonly XmlAttributeConfiguration scope;

        private readonly XmlNodeListConfiguration<Component> symbol; //FOR GLOBAL AND LOCAL VARIABLES

        private readonly XmlNodeConfiguration constant;
        private readonly XmlAttributeConfiguration constantName;     //FOR LOCAL AND GLOBAL CONSTANT
        private readonly XmlNodeConfiguration constantType;          //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY)
        private readonly XmlNodeConfiguration constantValue;         //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY)

        public Access() : base(Access.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData());
            scope = this.AddAttribute("Scope", required: true);

            symbol = this.AddNodeList("Symbol", Component.CreateComponent);

            constant = this.AddNode("Constant");
            constantName = constant.AddAttribute("Name");
            constantType = constant.AddNode("ConstantType");
            constantValue = constant.AddNode("ConstantValue");
            //==== INIT CONFIGURATION ====
        }
        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }

        public void SetAccessType(Type type)
        {
            switch (type)
            {
                case Type.LOCAL_VARIABLE:
                    scope.SetValue("LocalVariable");
                    break;
                case Type.LOCAL_CONSTANT:
                    scope.SetValue("LocalConstant");
                    break;
                case Type.LITERAL_CONSTANT:
                    scope.SetValue("LiteralConstant");
                    break;
                case Type.GLOBAL_CONSTANT:
                    scope.SetValue("GlobalConstant");
                    break;
                case Type.GLOBAL_VARIABLE:
                    scope.SetValue("GlobalVariable");
                    break;
            }
        }

        public Type GetAccessType()
        {
            switch (scope.GetValue())
            {
                case "LocalVariable":
                    return Type.LOCAL_VARIABLE;
                case "LocalConstant":
                    return Type.LOCAL_CONSTANT;
                case "LiteralConstant":
                    return Type.LITERAL_CONSTANT;
                case "GlobalConstant":
                    return Type.GLOBAL_CONSTANT;
                case "GlobalVariable":
                    return Type.GLOBAL_VARIABLE;
                default:
                    throw new Exception("Invalid Access Type for " + scope.GetValue());
            }
        }

        public string GetLiteralConstantType()
        {
            var type = this.GetAccessType();
            if (type == Type.LITERAL_CONSTANT)
            {
                return constantType.GetInnerText();
            }

            return "";
        }

        public string GetLiteralConstantValue()
        {
            var type = this.GetAccessType();
            if (type == Type.LITERAL_CONSTANT)
            {
                return constantValue.GetInnerText();
            }

            return "";
        }

        public string GetSymbol()
        {
            var type = this.GetAccessType();
            switch (type)
            {
                case Type.GLOBAL_CONSTANT:
                case Type.LOCAL_CONSTANT:
                    return constantName.GetValue();
                case Type.GLOBAL_VARIABLE:
                case Type.LOCAL_VARIABLE:
                    string joinedSymbol = "";
                    foreach (var component in symbol.GetItems())
                    {
                        joinedSymbol += component.GetComponentName() + ".";
                    }
                    return joinedSymbol.Substring(0, joinedSymbol.Length - 1); //Get the joined component names without the final dot.
            }

            return "";
        }

        public void SetSymbol(string address) //DB.Struct.TestInt
        {
            var type = this.GetAccessType();
            switch (type)
            {
                case Type.GLOBAL_CONSTANT:
                case Type.LOCAL_CONSTANT:
                    constantName.SetValue(address);
                    break;
                case Type.GLOBAL_VARIABLE:
                case Type.LOCAL_VARIABLE:
                    symbol.GetItems().Clear();

                    var splitAddress = address.Split('.');
                    foreach (var str in splitAddress)
                    {
                        var component = new Component();
                        component.SetComponentName(str);
                        symbol.GetItems().Add(component);
                    }
                    break;
            }
        }
    }

    public class Component : XmlNodeConfiguration
    {
        public const string NODE_NAME = "Component";
        public static Component CreateComponent(XmlNode node)
        {
            return node.Name == Component.NODE_NAME ? new Component() : null;
        }

        private readonly XmlAttributeConfiguration name;

        public Component(string value = "") : base(Component.NODE_NAME, required: true)
        {
            //==== INIT CONFIGURATION ====
            name = this.AddAttribute("Name", required: true, value: value);
            //==== INIT CONFIGURATION ====
        }

        public void SetComponentName(string name)
        {
            this.name.SetValue(name);
        }

        public string GetComponentName()
        {
            return name.GetValue();
        }
    }
}
