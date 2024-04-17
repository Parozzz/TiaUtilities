using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet
{
    public class LabelDeclaration : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "LabelDeclaration";
        public static LabelDeclaration CreateLabelDeclaration(XmlNode node, IDGenerator idGenerator)
        {
            return node.Name == LabelDeclaration.NODE_NAME ? new LabelDeclaration(idGenerator) : null;
        }

        private readonly LocalObjectData localObjectData;

        private readonly XmlNodeConfiguration label;
        private readonly XmlAttributeConfiguration labelName;

        public LabelDeclaration(IDGenerator idGenerator) : base(NODE_NAME) 
        {
            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData(idGenerator));

            label = this.AddNode("Label", required: true);
            labelName = label.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }
        public LocalObjectData GetLocalObjectData()
        {
            throw new NotImplementedException();
        }

        public string GetLabelName()
        {
            return this.labelName.GetValue();
        }

    }
}
