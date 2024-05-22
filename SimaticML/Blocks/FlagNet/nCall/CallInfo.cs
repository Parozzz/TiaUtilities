using SimaticML.Enums;
using SimaticML.Enums.Utility;
using System.Xml;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks.FlagNet.nCall
{
    public class CallInfo : XmlNodeListConfiguration<CallParameter>
    {
        public const string NODE_NAME = "CallInfo";
        private static CallParameter? CreateParameter(XmlNode node)
        {
            return node.Name == CallParameter.NODE_NAME ? new CallParameter() : null;
        }

        public string CallName { get => this.callName.AsString; set => this.callName.AsString = value; }
        public SimaticBlockType BlockType { get => this.blockType.AsEnum<SimaticBlockType>(); set => this.blockType.AsEnum(value); }
        public Instance Instance { get => this.instance; }


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
    }
}
