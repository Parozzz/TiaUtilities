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

                    var loopAddress = address;
                    while (true)
                    {
                        var indexOfDot = loopAddress.IndexOf('.');

                        var str = indexOfDot == -1 ? loopAddress : loopAddress.Substring(0, indexOfDot);
                        while (true)
                        {
                            //If the string does not start with a Double Quote, it means that there will be no point in the middle of it (TIA Won't allow it).
                            //If the string start with a double quote it must end with another one. If it doesn't do it, i will skip the dot since i am in the middle of a string
                            //(Like "DB.TEST".Var1, "DB.TEST" is in itself a component followed by Var1)
                            if (!str.StartsWith("\"") || str.EndsWith("\""))
                            {
                                break;
                            }

                            indexOfDot += loopAddress.IndexOf('.', indexOfDot + 1); //Need to increment the old index since IndexOf return a value starting from zero.
                            if(indexOfDot == -1)
                            {
                                throw new Exception("Error while parsing address " + address + ". Cannot parse " + loopAddress);
                            }

                            str = loopAddress.Substring(0, indexOfDot);
                        }

                        var name = str.Replace("\"", "");  //There cannot be any double quote in the address.
                        symbol.GetItems().Add(new Component().SetComponentName(name));

                        if (indexOfDot == -1)
                        {
                            break;
                        }

                        loopAddress = loopAddress.Substring(indexOfDot + 1);
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

        public Component SetComponentName(string name)
        {
            this.name.SetValue(name);
            return this;
        }

        public string GetComponentName()
        {
            return name.GetValue();
        }
    }
}
