using SimaticML.API;
using SimaticML.XMLClasses;

namespace SimaticML.TagTable
{
    public class XMLTagTable : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTagTable";

        public string TableName { get => this.tagTableName.AsString; set => this.tagTableName.AsString = value; }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration tagTableName;
        private readonly XmlNodeListConfiguration<XMLTag> objectList; //Tags

        public XMLTagTable() : base(XMLTagTable.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            attributeList = this.AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            tagTableName = attributeList.AddNode("Name", required: true, defaultInnerText: "DefaultTagTable");

            objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, XMLTag.CreateTag);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
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

            var dict = tagsList.ToDictionary(tag => tag.TagName);
            return dict;
        }

    }
}
