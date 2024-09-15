using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Blocks.FlagNet
{
    public class LabelDeclaration : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "LabelDeclaration";
        public static LabelDeclaration? CreateLabelDeclaration(XmlNode node)
        {
            return node.Name == LabelDeclaration.NODE_NAME ? new LabelDeclaration() : null;
        }

        public string LabelName { get => this.labelName.AsString; set => this.labelName.AsString = value; }

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
            this.uid.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.AsUInt;
        }
    }
}
