using SimaticML.Enums;
using SimaticML.Enums.Utility;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Blocks.FlagNet.nAccess
{
    public class Access : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Access";

        public SimaticVariableScope VariableScope { get => SimaticEnumUtils.FindByString<SimaticVariableScope>(scope.AsString); set => scope.AsString = value.GetSimaticMLString(); }

        public string ConstantName { get => this.constantName.AsString; set => this.constantName.AsString = value; }
        public string ConstantValue { get => this.constantValue.AsString; set => this.constantValue.AsString = value; }
        public SimaticDataType ConstantType { get => SimaticDataType.FromSimaticMLString(this.constantType.AsString); set => this.constantType.AsString = value.SimaticMLString; }


        private readonly XmlAttributeConfiguration uid;
        private readonly XmlAttributeConfiguration scope;

        private readonly XmlNodeListConfiguration<Component> symbol; //FOR GLOBAL AND LOCAL VARIABLES

        private readonly XmlNodeConfiguration constant;
        private readonly XmlAttributeConfiguration constantName;     //FOR LOCAL AND GLOBAL CONSTANT
        private readonly XmlNodeConfiguration constantType;          //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY)
        private readonly XmlNodeConfiguration constantValue;         //FOR LITERAL CONSTANT (NUMBER WRITTEN DIRECTLY) AND TYPED_CONSTANT

        private readonly XmlNodeConfiguration label;          //Not implemented
        private readonly XmlAttributeConfiguration labelName; //Not implemented

        public Access() : base(Access.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            scope = this.AddAttribute("Scope", required: true);
            uid = this.AddAttribute("UId"); //In case the access is innested inside a Component (For array access modifier) UId is not required.         

            symbol = this.AddNodeList("Symbol", Component.CreateComponent);

            constant = this.AddNode("Constant");
            constantName = constant.AddAttribute("Name");
            constantType = constant.AddNode("ConstantType");
            constantValue = constant.AddNode("ConstantValue");

            label = this.AddNode("Label");
            labelName = this.label.AddAttribute("Name");
            //==== INIT CONFIGURATION ====
        }

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());
        }

        public void SetUId(uint uid)
        {
            this.uid.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.AsUInt;
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
            var addressComponentList = SimaticMLUtil.SplitFullAddressIntoComponents(address);

            symbol.GetItems().Clear();
            foreach (var component in Access.ParseAddressComponents(addressComponentList))
            {
                symbol.GetItems().Add(component);
            }
        }

        public static List<Component> ParseAddressComponents(List<SimaticAddressComponent> addressComponentList)
        {
            var componentList = new List<Component>();
            foreach (var addressComponent in addressComponentList)
            {
                var component = new Component().SetComponentName(addressComponent.Name);
                //If there is any, it will be set to be an array and add an access for the index.
                Access.ParseArrayIndexes(addressComponent.ArrayIndexes).ForEach(access =>
                {
                    component.SetAccessModifier("Array")
                            .GetItems().Add(access);
                });
                componentList.Add(component);
            }
            return componentList;
        }

        public static List<Access> ParseArrayIndexes(List<SimaticAddressArrayIndex> arrayIndexList)
        {
            var accessList = new List<Access>();
            foreach (var arrayIndex in arrayIndexList)
            {
                var access = new Access();

                if (arrayIndex.Components.Count == 1 && int.TryParse(arrayIndex.Components[0].Name, out int constant))
                {
                    access.VariableScope = SimaticVariableScope.LITERAL_CONSTANT;
                    access.ConstantType = SimaticDataType.DINT;
                    access.ConstantValue = "" + constant;
                }
                else
                {
                    access.VariableScope = SimaticVariableScope.GLOBAL_VARIABLE;

                    var arrayIndexComponents = Access.ParseAddressComponents(arrayIndex.Components);
                    arrayIndexComponents.ForEach(access.symbol.GetItems().Add);
                }

                accessList.Add(access);
            }

            return accessList;
        }
    }

    //In case of accessing a multi depth array (Like a Matrix) can have multiple access inside for the Array sliceAccessModifier.
    public class Component : XmlNodeListConfiguration<Access>
    {
        public const string NODE_NAME = "Component";
        public static Component? CreateComponent(XmlNode node)
        {
            return node.Name == Component.NODE_NAME ? new Component() : null;
        }

        public static Access? CreateAccess(XmlNode node)
        {
            return node.Name == Access.NODE_NAME ? new Access() : null;
        }

        private readonly XmlAttributeConfiguration componentName;
        private readonly XmlAttributeConfiguration simpleAccessModifier; //Periphery|QualityInformation
        private readonly XmlAttributeConfiguration sliceAccessModifier;  //[xbwdXBWD]\d+
        private readonly XmlAttributeConfiguration accessModifier; //Only for arrays

        public Component(string value = "") : base(Component.NODE_NAME, Component.CreateAccess, required: true)
        {
            //==== INIT CONFIGURATION ====
            componentName = this.AddAttribute("Name", required: true, value: value);
            sliceAccessModifier = this.AddAttribute("SliceAccessModifier");
            simpleAccessModifier = this.AddAttribute("SliceAccessModifier");
            accessModifier = this.AddAttribute("AccessModifier");
            //==== INIT CONFIGURATION ====
        }

        public string GetComponentName()
        {
            return componentName.AsString;
        }

        public Component SetComponentName(string name)
        {
            this.componentName.AsString = name;
            return this;
        }

        public string GetSliceAccessModifier()
        {
            return sliceAccessModifier.AsString;
        }

        public Component SetSliceAccessModifier(string sliceAccessModifier)
        {
            this.sliceAccessModifier.AsString = sliceAccessModifier;
            return this;
        }

        public string GetSimpleAccessModifier()
        {
            return simpleAccessModifier.AsString;
        }

        public Component SetSimpleAccessModifier(string simpleAccessModifier)
        {
            this.simpleAccessModifier.AsString = simpleAccessModifier;
            return this;
        }

        public string GetAccessModifier()
        {
            return accessModifier.AsString;
        }

        public Component SetAccessModifier(string accessModifier)
        {
            this.accessModifier.AsString = accessModifier;
            return this;
        }
    }
}
