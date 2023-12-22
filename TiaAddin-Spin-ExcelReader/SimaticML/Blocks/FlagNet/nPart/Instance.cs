using DocumentFormat.OpenXml.Drawing.Charts;
using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nPart
{
    public class Instance : XmlNodeListConfiguration<Component>
    {
        public const string NODE_NAME = "Instance";
        private static Component CreateComponent(XmlNode xmlNode)
        {
            return xmlNode.Name == Component.NODE_NAME ? new Component() : null;
        }

        private readonly CompileUnit compileUnit;

        private readonly XmlAttributeConfiguration scope;
        private readonly XmlAttributeConfiguration uid;

        public Instance(CompileUnit compileUnit) : base(NODE_NAME, Instance.CreateComponent)
        {
            this.compileUnit = compileUnit;

            //==== INIT CONFIGURATION ====
            scope = this.AddAttribute("Scope", required: true);
            uid = this.AddAttribute("UId", required: true);
            //==== INIT CONFIGURATION ====
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }

        private void SetUId()
        {
            if (string.IsNullOrEmpty(uid.GetValue()))
            {//Set the uid only if a value has been added, otherwise it will not be empty anymore and will be added even if not needed.
                uid.SetValue("" + compileUnit.LocalIDGenerator.GetNextString());
            }
        }

        public Instance SetVariableScope(SimaticVariableScope scope)
        {
            SetUId();

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
            SetUId();

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
