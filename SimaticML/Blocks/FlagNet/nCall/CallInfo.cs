using SimaticML.Enums;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Blocks.FlagNet.nCall
{
    public class CallInfo : XmlNodeListConfiguration<CallParameter>
    {
        public const string NODE_NAME = "CallInfo";
        private static CallParameter? CreateParameter(XmlNode node)
        {
            return node.Name == CallParameter.NODE_NAME ? new CallParameter() : null;
        }

        public string CallName { get => this.callName.Value.AsString; set => this.callName.Value.AsString = value; }
        public SimaticBlockType BlockType { get => this.blockType.Value.AsEnum<SimaticBlockType>(); set => this.blockType.Value.AsEnum(value); }
        public CallInstance Instance { get => this.instance; }


        private readonly XmlAttributeConfiguration uid; //Only for SCL
        private readonly XmlAttributeConfiguration callName;
        private readonly XmlAttributeConfiguration blockType;
        private readonly CallInstance instance;

        public CallInfo() : base(CallInfo.NODE_NAME, CallInfo.CreateParameter)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");

            callName = this.AddAttribute("Name", required: true);
            blockType = this.AddAttribute("BlockType", required: true);
            instance = this.AddNode(new CallInstance());
            //==== INIT CONFIGURATION ====
        }
    }
}
