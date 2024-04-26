using System;
using System.Xml;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet
{
    public class LabelDeclaration : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "LabelDeclaration";
        public static LabelDeclaration CreateLabelDeclaration(XmlNode node)
        {
            return node.Name == LabelDeclaration.NODE_NAME ? new LabelDeclaration() : null;
        }

        private readonly XmlAttributeConfiguration uid;

        private readonly XmlNodeConfiguration label;
        private readonly XmlAttributeConfiguration labelName;

        public LabelDeclaration() : base(NODE_NAME) 
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId", required: true);

            label = this.AddNode("Label", required: true);
            labelName = label.AddAttribute("Name", required: true);
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

        public string GetLabelName()
        {
            return this.labelName.GetValue();
        }

    }
}
