using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nCall
{
    public class CallInfo : XmlNodeListConfiguration<CallParameter>
    {
        public const string NODE_NAME = "CallInfo";
        private static CallParameter CreateParameter(XmlNode node)
        {
            return node.Name == CallParameter.NODE_NAME ? new CallParameter() : null;
        }

        private readonly XmlAttributeConfiguration uid; //Only for SCL
        private readonly XmlAttributeConfiguration callName;
        private readonly XmlAttributeConfiguration blockType;
        private readonly Instance instance;

        public CallInfo() : base(CallInfo.NODE_NAME, CallInfo.CreateParameter)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");

            callName = this.AddAttribute("Name", required: true);
            blockType = this.AddAttribute("BlockType", required: true);
            instance = this.AddNode(new Instance());
            //==== INIT CONFIGURATION ====
        }

        public Instance GetInstance()
        {
            return instance;
        }

        public CallInfo SetCallName(string callName)
        {
            this.callName.SetValue(callName);
            return this;
        }

        public string GetCallName()
        {
            return this.callName.GetValue();
        }

        public CallInfo SetBlockType(SimaticBlockType blockType)
        {
            this.blockType.SetValue(blockType.GetSimaticMLString());
            return this;
        }

        public SimaticBlockType GetBlockType()
        {
            return Utils.FindEnumByStringMethod<SimaticBlockType>(this.blockType.GetValue(), SimaticBlockTypeExtension.GetSimaticMLString);
        }

    }
}
