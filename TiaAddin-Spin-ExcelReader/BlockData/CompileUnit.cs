using Siemens.Engineering.SW.Blocks;
using System;
using System.Xml;

namespace SpinXmlReader.Block
{
    public class CompileUnit : IXMLNodeSerializable, IGlobalObject
    {
        public MultilingualText Title { get; private set; }
        public MultilingualText Comment { get; private set; }
        public ProgrammingLanguage ProgrammingLanguage { get; private set; }
        public FlagNet Net { get; private set; }

        private readonly FCData fcData;

        internal CompileUnit(FCData fcData)
        {
            this.fcData = fcData;
        }

        internal CompileUnit ParseXmlNode(XmlNode node)
        {
            var attributeListNode = node.SelectSingleNode("AttributeList");
            var objectListNode = node.SelectSingleNode("ObjectList");
            if(attributeListNode == null || objectListNode == null)
            {
                return null;
            }

            var convOK = true;
            convOK &= Enum.TryParse(attributeListNode["ProgrammingLanguage"]?.InnerText, true, out ProgrammingLanguage language);
            if(!convOK)
            {
                return null;
            }

            ProgrammingLanguage = language;
            Title = MultilingualText.FindInParent(objectListNode, "Title");
            Comment = MultilingualText.FindInParent(objectListNode, "Comment");

            var networkSourceNode = attributeListNode.SelectSingleNode("NetworkSource");
            if (networkSourceNode != null)
            {
                foreach (XmlNode networkSourceChildNode in networkSourceNode.ChildNodes)
                {
                    switch (networkSourceChildNode.Name)
                    {
                        case "StructuredText":
                            break;
                        case "FlgNet":
                            //Net = new FlagNet(fcData).ParseXMLNode(networkSourceChildNode);
                            break;
                    }
                }
            }

            return this;
        }

        public uint GetID()
        {
            throw new NotImplementedException();
        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
