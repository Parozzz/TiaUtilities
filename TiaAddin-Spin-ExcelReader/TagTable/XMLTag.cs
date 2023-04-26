using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.TagTable
{
    public class XMLTag : IXMLNodeSerializable, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTag";
        public const string COMPOSITION_NAME = "Tags";

        public const string DATATYPE_NAME = "DataTypeName";
        public const string LOGICAL_ADDRESS = "LogicalAddress";
        public const string NAME = "Name";

        private readonly XmlNodeConfiguration dataTypeName;
        private readonly XmlNodeConfiguration logicalAddress;
        private readonly XmlNodeConfiguration name;

        private readonly MultilingualText comment;
        private readonly GlobalObjectData globalObjectData;

        private readonly XmlConfigurator configurator;

        public XMLTag()
        {
            comment = new MultilingualText(MultilingualTextType.COMMENT);
            globalObjectData = new GlobalObjectData();

            configurator = new XmlConfigurator(NODE_NAME);

            var attributeList = configurator.AddNode(Constants.ATTRIBUTE_LIST_NAME, required: true);
            dataTypeName = attributeList.AddNode(XMLTag.DATATYPE_NAME, defaultInnerText: "bool");
            logicalAddress = attributeList.AddNode(XMLTag.LOGICAL_ADDRESS, defaultInnerText: "%M0.0");
            name = attributeList.AddNode(XMLTag.NAME, defaultInnerText: "DefaultTagName");

            var objectList = configurator.AddNode(Constants.OBJECT_LIST_NAME, required: false);
            objectList.AddNode(comment);
        }

        public MultilingualText GetComment()
        {
            return comment;
        }

        public GlobalObjectData GetGlobalObjectIdata()
        {
            return globalObjectData;
        }

        public void ParseNode(XmlNode tagNode)
        {
            globalObjectData.ParseNode(tagNode);
            configurator.Parse(tagNode);
            /*
            var attributeList = XMLUtils.GetFirstChild(tagNode, "AttributeList");
            if (attributeList != null)
            {
                //Each TAG has all this values. If non, a null pointer exception need to happen.
                DataTypeName = attributeList["DataTypeName"].InnerText;
                LogicalAddress = attributeList["LogicalAddress"].InnerText;
                Name = attributeList["Name"].InnerText;
            }

            var objectList = XMLUtils.GetFirstChild(tagNode, "ObjectList");
            if (objectList != null)
            {
                comment.ParseFirstFrom(objectList);
            }
            */
        }

        public XmlNode GenerateNode(XmlDocument document)
        {
            var xmlNode = configurator.Generate(document);
            new GlobalObjectData(COMPOSITION_NAME).GenerateNextID().SetToNode(xmlNode);
            return xmlNode;

            /*
            var attributeList = xmlNode.AppendChild(document.CreateElement(Constants.ATTRIBUTE_LIST_NAME));
            attributeList.AppendChild(document.CreateElement("DataTypeName")).InnerText = DataTypeName;
            attributeList.AppendChild(document.CreateElement("LogicalAddress")).InnerText = LogicalAddress;
            attributeList.AppendChild(document.CreateElement("Name")).InnerText = Name;

            var objectList = xmlNode.AppendChild(document.CreateElement(Constants.OBJECT_LIST_NAME));
            if(comment != null)
            {
                objectList.AppendChild(comment.GenerateNode(document));
            }

            return xmlNode;*/
        }
    }
}
