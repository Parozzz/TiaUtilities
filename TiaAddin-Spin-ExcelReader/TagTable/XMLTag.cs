using System.Xml;

namespace SpinXmlReader.TagTable
{
    public class XMLTag : IXMLNodeSerializable, IGlobalObject
    {
        public static string NAME = "SW.Tags.PlcTag";
        public static string COMPOSITION_NAME = "Tags";

        public string DataTypeName { get; set; }
        public string LogicalAddress { get; set; }
        public string Name { get; set; }

        private readonly MultilingualText comment;
        private readonly GlobalObjectData globalObjectData;

        public XMLTag()
        {
            DataTypeName = "bool";
            LogicalAddress = "%M0.0";
            Name = "DefaultTagName";

            comment = new MultilingualText(MultilingualTextType.COMMENT);
            globalObjectData = new GlobalObjectData();
        }

        public MultilingualText GetComment()
        {
            return comment;
        }

        public GlobalObjectData GetGlobalObjectIdata()
        {
            return globalObjectData;
        }

        public void ParseXMLNode(XmlNode tagNode)
        {
            globalObjectData.ParseXMLNode(tagNode);

            var attributeList = XMLUtils.GetFirstChild(tagNode, "AttributeList");
            if (attributeList != null)
            {
                DataTypeName = attributeList["DataTypeName"]?.InnerText;
                LogicalAddress = attributeList["LogicalAddress"]?.InnerText;
                Name = attributeList["Name"]?.InnerText;
            }

            var objectList = XMLUtils.GetFirstChild(tagNode, "ObjectList");
            if (objectList != null)
            {
                comment = MultilingualText.FindInParent(objectList, "Comment");
            }

        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            var xmlNode = document.CreateElement(NAME);
            new GlobalObjectData(COMPOSITION_NAME).SetToXMLNode(xmlNode);

            var attributeList = xmlNode.AppendChild(document.CreateElement(Constants.ATTRIBUTE_LIST_NAME));
            attributeList.AppendChild(document.CreateElement("DataTypeName")).InnerText = DataTypeName;
            attributeList.AppendChild(document.CreateElement("LogicalAddress")).InnerText = LogicalAddress;
            attributeList.AppendChild(document.CreateElement("Name")).InnerText = Name;

            var objectList = xmlNode.AppendChild(document.CreateElement(Constants.OBJECT_LIST_NAME));
            if(comment != null)
            {
                objectList.AppendChild(comment.GenerateXmlNode(document));
            }

            return xmlNode;
        }
    }
}
