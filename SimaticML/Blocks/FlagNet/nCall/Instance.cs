using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using SimaticML.XMLClasses;
using System.Data;
using System.Xml;

namespace SimaticML.Blocks.FlagNet.nCall
{
    public class Instance : XmlNodeListConfiguration<Component>, ILocalObject
    {
        public const string NODE_NAME = "Instance";
        private static Component? CreateComponent(XmlNode xmlNode)
        {
            return xmlNode.Name == Component.NODE_NAME ? new Component() : null;
        }

        public SimaticVariableScope VariableScope { get => this.scope.AsEnum<SimaticVariableScope>(); set => this.scope.AsEnum(value); }

        private readonly XmlAttributeConfiguration scope;
        private readonly XmlAttributeConfiguration uid;

        public Instance() : base(Instance.NODE_NAME, Instance.CreateComponent)
        {
            //==== INIT CONFIGURATION ====
            scope = this.AddAttribute("Scope", required: true);
            uid = this.AddAttribute("UId", required: true);
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
            var componentList = this.GetItems()
                                    .Select(x => x.GetComponentName())
                                    .ToList();
            return SimaticMLUtil.JoinComponentsIntoAddress(componentList);
        }

        public Instance SetAddress(string address)
        {
            this.GetItems().Clear();

            var addressComponentList = SimaticMLUtil.SplitFullAddressIntoComponents(address);
            foreach (var component in Access.ParseAddressComponents(addressComponentList))
            {
                this.GetItems().Add(component);
            }

            return this;
        }
    }
}
