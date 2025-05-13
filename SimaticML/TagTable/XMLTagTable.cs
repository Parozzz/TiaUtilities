using SimaticML.API;
using SimaticML.Blocks.FlagNet;
using SimaticML.Enums;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.TagTable
{
    public class XMLTagTable : XmlNodeConfiguration, IGlobalObject, ISimaticVariableCollection
    {
        public const string NODE_NAME = "SW.Tags.PlcTagTable";
        public static XmlNodeConfiguration? CreateXmlTagTableObject(XmlNode node)
        {
            return node.Name switch
            {
                XMLTag.NODE_NAME => new XMLTag(),
                XMLUserConstant.NODE_NAME => new XMLUserConstant(),
                _ => null,
            };
        }

        public string TableName { get => this.tagTableName.AsString; set => this.tagTableName.AsString = value; }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration tagTableName;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList; //Tags

        public XMLTagTable() : base(XMLTagTable.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            attributeList = this.AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            tagTableName = attributeList.AddNode("Name", required: true, defaultInnerText: "DefaultTagTable");

            objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, XMLTagTable.CreateXmlTagTableObject);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public SimaticVariable AddVariableTag(string name, SimaticDataType dataType)
        {
            var tag = this.AddTag();
            tag.TagName = name;
            tag.DataType = dataType;

            return new SimaticGlobalVariable(this, tag);
        }

        public SimaticVariable AddVariableConstant(string constantName, SimaticDataType dataType, string constantValue)
        {
            var userConstant = this.AddConstant();
            userConstant.ConstantName = constantName;
            userConstant.ConstantValue = constantValue;
            userConstant.DataType = dataType;

            return new SimaticGlobalConstant(this, userConstant);
        }

        public XMLTag AddTag()
        {
            var tag = new XMLTag();
            tag.Init();
            objectList.GetItems().Add(tag);
            return tag;
        }

        public XMLUserConstant AddConstant()
        {
            var userConstant = new XMLUserConstant();
            userConstant.Init();
            objectList.GetItems().Add(userConstant);
            return userConstant;
        }

        public Dictionary<string, XMLTag> GetTags()
        {
            var tagsList = objectList.GetItems();

            var dict = tagsList.Where(node => node is XMLTag)
                .Cast<XMLTag>()
                .ToDictionary(tag => tag.TagName);
            return dict;
        }

        public Dictionary<string, XMLUserConstant> GetConstants()
        {
            var tagsList = objectList.GetItems();

            var dict = tagsList.Where(node => node is XMLUserConstant)
                .Cast<XMLUserConstant>()
                .ToDictionary(userConstant => userConstant.ConstantName);
            return dict;
        }

        public SimaticDataType? FetchDataTypeOf(string variableName)
        {
            foreach (var node in this.objectList.GetItems())
            {
                if(node is XMLTag tag 
                    && tag.TagName.Equals(variableName, StringComparison.OrdinalIgnoreCase))
                {
                    return tag.DataType;
                }
                else if(node is XMLUserConstant userConstant 
                    && userConstant.ConfigurationName.Equals(variableName, StringComparison.OrdinalIgnoreCase))
                {
                    return userConstant.DataType;
                }
            }

            return null;
        }
    }
}
