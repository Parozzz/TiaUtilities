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
    public class PartInstance : XmlNodeListConfiguration<Component>
    {
        public const string NODE_NAME = "Instance";
        private static Component CreateComponent(XmlNode xmlNode)
        {
            return xmlNode.Name == Component.NODE_NAME ? new Component() : null;
        }

        private readonly XmlAttributeConfiguration scope;
        private readonly XmlAttributeConfiguration uid;

        public PartInstance(CompileUnit compileUnit) : base(NODE_NAME, PartInstance.CreateComponent)
        {
            //==== INIT CONFIGURATION ====
            scope = this.AddAttribute("Scope", required: true);
            uid = this.AddAttribute("UId", required: true, value: compileUnit.LocalIDGenerator.GetNextString());

            //==== INIT CONFIGURATION ====
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }

        public PartInstance SetVariableScope(SimaticVariableScope scope)
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

        public void SetAddress(string address)
        {
            this.GetItems().Clear();

            var componentList = SimaticMLUtil.SplitFullAddressIntoComponents(address);
            foreach (var component in componentList)
            {
                this.GetItems().Add(new Component().SetComponentName(component));
            }
        }
    }
}
