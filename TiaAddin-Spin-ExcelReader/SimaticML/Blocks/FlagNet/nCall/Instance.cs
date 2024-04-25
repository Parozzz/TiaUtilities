using System;
using System.Linq;
using System.Xml;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.XMLClasses;
using System.Data;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nCall
{
    public class Instance : XmlNodeListConfiguration<Component>, ILocalObject
    {
        public const string NODE_NAME = "Instance";
        private static Component CreateComponent(XmlNode xmlNode)
        {
            return xmlNode.Name == Component.NODE_NAME ? new Component() : null;
        }

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
            this.uid.SetValue("" + uid);
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }

        public Instance SetVariableScope(SimaticVariableScope scope)
        {
            this.scope.SetValue(scope.GetSimaticMLString());
            return this;
        }

        public SimaticVariableScope GetVariableScope()
        {
            foreach (SimaticVariableScope variableScope in Enum.GetValues(typeof(SimaticVariableScope)))
            {
                if (scope.GetValue() == variableScope.GetSimaticMLString())
                {
                    return variableScope;
                }
            }

            throw new Exception("Invalid VariableScope for " + scope.GetValue());
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
