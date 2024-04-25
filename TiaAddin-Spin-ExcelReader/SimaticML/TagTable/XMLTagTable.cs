using System.Collections.Generic;
using TiaXmlReader.Utility;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.TagTable
{
    public class XMLTagTable : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTagTable";

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration tagTableName;
        private readonly XmlNodeListConfiguration<XMLTag> objectList; //Tags

        public XMLTagTable() : base(XMLTagTable.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            attributeList = this.AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            tagTableName = attributeList.AddNode("Name", required: true, defaultInnerText: "DefaultTagTable");

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, XMLTag.CreateTag);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void SetTagTableName(string name)
        {
            this.tagTableName.SetInnerText(name);
        }

        public string GetTagTableName()
        {
            return tagTableName.GetInnerText();
        }

        public XMLTag AddTag()
        {
            var tag = new XMLTag();
            tag.Init();
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
