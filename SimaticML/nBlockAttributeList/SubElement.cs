using SimaticML.API;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.nBlockAttributeList
{
    public class SubElement : XmlNodeConfiguration
    {
        public const string NODE_NAME = "Subelement";
        public static SubElement? CreateSubElement(XmlNode node)
        {
            return node.Name == SubElement.NODE_NAME ? new SubElement() : null;
        }

        public string Path { get => this.path.Value.AsString; set => this.path.Value.AsString = value; }
        public string StartValue { get => this.startValue.Value.AsString; set => this.startValue.Value.AsString = value; }
        public Comment Comment { get => comment; }

        private readonly XmlAttributeConfiguration path;  //Not implemented yet
        private readonly XmlNodeConfiguration startValue; //Not implemented yet
        private readonly Comment comment;

        public SubElement() : base(SubElement.NODE_NAME, namespaceURI: SimaticMLAPI.GET_SECTIONS_NAMESPACE())
        {
            this.path = this.AddAttribute("Path", required: true);
            this.startValue = this.AddNode("StartValue");

            this.comment = this.AddNode(new Comment());
        }
    }
}
