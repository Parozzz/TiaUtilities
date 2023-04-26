using System;
using System.Collections.Generic;
using System.Xml;

namespace SpinXmlReader.TagTable
{
    public class XMLTagTable : IXMLNodeSerializable, IGlobalObject
    {
        public static string NAME = "SW.Tags.PlcTagTable";

        private readonly XmlDocument document;

        public string Name { get; set; }

        private readonly GlobalObjectData globalObjectData;
        private readonly Dictionary<string, XMLTag> tags;
        public XMLTagTable(XmlDocument document)
        {
            this.document = document;

            this.Name = "DefaultTagTable";

            globalObjectData = new GlobalObjectData();
            tags = new Dictionary<string, XMLTag>();
        }

        public Dictionary<string, XMLTag> GetTags()
        {
            return tags;
        }

        public GlobalObjectData GetGlobalObjectIdata()
        {
            return globalObjectData;
        }

        public void ParseNode(XmlNode tagTableNode)
        {
            globalObjectData.ParseNode(tagTableNode);

            var attributeList = XMLUtils.GetFirstChild(tagTableNode, "AttributeList");
            if (attributeList != null)
            {
                Name = attributeList["Name"]?.InnerText;
            }

            var objectList = XMLUtils.GetFirstChild(tagTableNode, "ObjectList");
            if (objectList == null)
            {
                return;
            }

            var tagNodeList = XMLUtils.GetAllChild(objectList, "SW.Tags.PlcTag");
            if (tagNodeList == null || tagNodeList.Count == 0)
            {
                return;
            }

            foreach (var tagNode in tagNodeList)
            {
                var tag = new XMLTag();
                tag.ParseNode(tagNode);

                if (!tags.ContainsKey(tag.LogicalAddress))
                {
                    tags.Add(tag.LogicalAddress, tag);
                }
            }

        }

        public XmlNode GenerateNode(XmlDocument document)
        {
            var xmlNode = document.CreateNode(XmlNodeType.Element, NAME, "");

            var globalObjectData = new GlobalObjectData();
            globalObjectData.SetToNode(xmlNode);


            // ========== ATTRIBUTE LIST ==========
            var attributeList = xmlNode.AppendChild(document.CreateElement(Constants.ATTRIBUTE_LIST_NAME));
            attributeList.AppendChild(document.CreateElement("Name")).InnerText = Name;
            // ========== ATTRIBUTE LIST ==========

            // ========== OBJECT LIST ==========
            var objectList = xmlNode.AppendChild(document.CreateElement(Constants.OBJECT_LIST_NAME));
            foreach (var tag in tags.Values)
            {
                objectList.AppendChild(tag.GenerateNode(document));
            }
            // ========== OBJECT LIST ==========

            return xmlNode;
        }
    }
}
