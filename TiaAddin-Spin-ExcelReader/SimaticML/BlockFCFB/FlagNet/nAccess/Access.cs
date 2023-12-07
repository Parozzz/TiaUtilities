using SpinXmlReader;
using SpinXmlReader.Block;
using System;
using System.Linq;
using System.Xml;
using TiaXmlReader.Utility;

namespace TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace
{
    public enum AccessScope
    {
        LOCAL_VARIABLE,
        LOCAL_CONSTANT,
        LITERAL_CONSTANT,
        TYPED_CONSTANT,
        GLOBAL_CONSTANT,
        GLOBAL_VARIABLE,
    }

    public static class AccessScopeExtension
    {
        public static string GetSimaticMLString(this AccessScope type)
        {
            switch (type)
            {
                case AccessScope.LOCAL_VARIABLE: return "LocalVariable";
                case AccessScope.LOCAL_CONSTANT: return "LocalConstant";
                case AccessScope.LITERAL_CONSTANT: return "LiteralConstant";
                case AccessScope.TYPED_CONSTANT: return "TypedConstant";
                case AccessScope.GLOBAL_CONSTANT: return "GlobalConstant";
                case AccessScope.GLOBAL_VARIABLE: return "GlobalVariable";
                default:
                    throw new Exception("AccessScope " +type.ToString() + "  not yet implemented");
            }
        }

        public static IAccessData CreateAccessData(this AccessScope type, CompileUnit compileUnit)
        {
            switch (type)
            {
                case AccessScope.LOCAL_VARIABLE: return new LocalVariableAccessData(compileUnit);
                case AccessScope.LOCAL_CONSTANT: return new LocalConstantAccessData(compileUnit);
                case AccessScope.LITERAL_CONSTANT: return new LiteralConstantAccessData(compileUnit);
                case AccessScope.TYPED_CONSTANT: return new TypedConstantAccessData(compileUnit);
                case AccessScope.GLOBAL_CONSTANT: return new GlobalConstantAccessData(compileUnit);
                case AccessScope.GLOBAL_VARIABLE: return new GlobalVariableAccessData(compileUnit);
                default:
                    throw new Exception("AccessScope " + type.ToString() + "  not yet implemented");
            }
        }
    }

    public class Access : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Access";

        private readonly LocalObjectData localObjectData;

        private readonly XmlAttributeConfiguration scope;

        private readonly XmlNodeListConfiguration<Component> symbol; //FOR GLOBAL AND LOCAL VARIABLES

        private readonly XmlNodeConfiguration constant;
        private readonly XmlAttributeConfiguration constantName;     //FOR LOCAL AND GLOBAL CONSTANT
        private readonly XmlNodeConfiguration constantType;          //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY)
        private readonly XmlNodeConfiguration constantValue;         //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY) AND TYPED_CONSTANT

        public Access(CompileUnit compileUnit) : base(Access.NODE_NAME)
        {
            compileUnit.AddAccess(this);

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

        public Access SetAccessScope(AccessScope scope)
        {
            this.scope.SetValue(scope.GetSimaticMLString());
            return this;
        }

        public AccessScope GetAccessScope()
        {
            foreach (AccessScope accessType in Enum.GetValues(typeof(AccessScope)))
            {
                if (scope.GetValue() == accessType.GetSimaticMLString())
                {
                    return accessType;
                }
            }

            throw new Exception("Invalid Access Type for " + scope.GetValue());
        }

        public string GetAddress()
        {
            var componentList = symbol.GetItems()
                                    .Select(x => x.GetComponentName())
                                    .ToList();
            return SimaticMLUtil.JoinComponentsIntoAddress(componentList);
        }

        public void SetAddress(string address)
        {
            symbol.GetItems().Clear();

            var componentList = SimaticMLUtil.SplitAddressIntoComponents(address);
            foreach (var component in componentList)
            {
                symbol.GetItems().Add(new Component().SetComponentName(component));
            }
        }

        public string GetConstantName()
        {
            return constantName.GetValue();
        }

        public Access SetConstantName(string constantName)
        {
            this.constantName.SetValue(constantName);
            return this;
        }

        public SimaticDataType GetConstantType()
        {
            return SimaticDataTypeUtil.GetFromSimaticMLString(this.constantType.GetInnerText());
        }

        public Access SetConstantType(SimaticDataType dataType)
        {
            this.constantType.SetInnerText(dataType.GetSimaticMLString());
            return this;
        }

        public string GetConstantValue()
        {
            return constantValue.GetInnerText();
        }

        public void SetConstantValue(string constantValue)
        {
            this.constantValue.SetInnerText(constantValue);
        }
    }

    public class Component : XmlNodeConfiguration
    {
        public const string NODE_NAME = "Component";
        public static Component CreateComponent(XmlNode node)
        {
            return node.Name == Component.NODE_NAME ? new Component() : null;
        }

        private readonly XmlAttributeConfiguration componentName;

        public Component(string value = "") : base(Component.NODE_NAME, required: true)
        {
            //==== INIT CONFIGURATION ====
            componentName = this.AddAttribute("Name", required: true, value: value);
            //==== INIT CONFIGURATION ====
        }

        public Component SetComponentName(string name)
        {
            this.componentName.SetValue(name);
            return this;
        }

        public string GetComponentName()
        {
            return componentName.GetValue();
        }
    }
}
