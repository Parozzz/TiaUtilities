using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.TagTable
{
    public class XMLTag : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTag";
        public static XMLTag CreateTag(XmlNode node)
        {
            return node.Name == XMLTag.NODE_NAME ? new XMLTag() : null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration dataTypeName;
        private readonly XmlNodeConfiguration logicalAddress;
        private readonly XmlNodeConfiguration tagName;

        private readonly XmlNodeConfiguration objectList;

        private readonly MultilingualText comment;
        
        public XMLTag() : base(XMLTag.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(Constants.COMPOSITION_NAME_KEY, required: true, requiredValue: "Tags");

            var attributeList = this.AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            dataTypeName = attributeList.AddNode("DataTypeName",           required: true, defaultInnerText: "bool");
            logicalAddress = attributeList.AddNode("LogicalAddress",       required: true, defaultInnerText: "%M0.0");
            tagName = attributeList.AddNode("Name",                        required: true, defaultInnerText: "DefaultTagName");

            objectList = this.AddNode(Constants.OBJECT_LIST_KEY, required: false);
            comment = objectList.AddNode(new MultilingualText(MultilingualTextType.COMMENT));
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public MultilingualText GetComment()
        {
            return comment;
        }

        public string GetDataTypeName()
        {
            return dataTypeName.GetInnerText();
        }

        public void SetDataTypeName(string str)
        {
            dataTypeName.SetInnerText(str);
        }

        public string GetLogicalAddress()
        {
            return logicalAddress.GetInnerText();
        }

        public void SetLogicalAddress(string str)
        {
            logicalAddress.SetInnerText(str);
        }

        public string GetTagName()
        {
            return tagName.GetInnerText();
        }

        public void SetTagName(string str)
        {
            tagName.SetInnerText(str);
        }
    }
}
