using System;
using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.TagTable
{
    public class XMLTagTable : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_KEY = "SW.Tags.PlcTagTable";

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration name;
        private readonly XmlNodeListConfiguration<XMLTag> objectList; //Tags

        public XMLTagTable() : base(XMLTagTable.NODE_KEY, main: true)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            attributeList = this.AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            name = attributeList.AddNode("Name", required: true, defaultInnerText: "DefaultTagTable");

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, XMLTag.CreateTag);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void SetTagTableName(string name)
        {
            this.name.SetInnerText(name);
        }

        public string GetTagTableName()
        {
            return name.GetInnerText();
        }

        public XMLTag AddTag()
        {
            var tag = new XMLTag();
            objectList.GetItems().Add(tag);
            return tag;
        }

        public Dictionary<string, XMLTag> GetTags()
        {
            var tagsList = objectList.GetItems();

            var dict = new Dictionary<string, XMLTag>();
            foreach(var tag in tagsList)
            {
                dict.Add(tag.GetTagName(), tag);
            }
            return dict;
        }

    }
}
