using SimaticML.API;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;
using System.Globalization;
using System.Xml;

namespace SimaticML.TagTable
{
    public class XMLUserConstant : XmlNodeConfiguration, IGlobalObject, ISimaticVariableDataHolder
    {
        public const string NODE_NAME = "SW.Tags.PlcUserConstant";

        public MultilingualText Comment { get => comment; }
        public string ConstantName { get => constantName.AsString; set => constantName.AsString = value; }
        public string ConstantValue { get => constantValue.AsString; set => constantValue.AsString = value; }
        public SimaticDataType DataType { get => SimaticDataType.FromSimaticMLString(dataTypeName.AsString); set => dataTypeName.AsString = value.SimaticMLString; }


        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration dataTypeName;
        private readonly XmlNodeConfiguration constantName;
        private readonly XmlNodeConfiguration constantValue;

        private readonly XmlNodeConfiguration objectList;
        private readonly MultilingualText comment;

        public XMLUserConstant() : base(XMLTag.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(SimaticMLAPI.COMPOSITION_NAME_KEY, required: true, requiredValue: "Tags");

            var attributeList = this.AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            dataTypeName = attributeList.AddNode("DataTypeName", required: true, defaultInnerText: "bool");
            constantName = attributeList.AddNode("Name", required: true, defaultInnerText: "DEFAULT_CONSTANT");
            constantValue = attributeList.AddNode("Value", required: true, defaultInnerText: "false");

            objectList = this.AddNode(SimaticMLAPI.OBJECT_LIST_KEY, required: false);
            comment = objectList.AddNode(new MultilingualText(MultilingualTextType.COMMENT));
            //==== INIT CONFIGURATION ====
        }

        public void Init()
        {
            comment[SimaticMLAPI.CULTURE] = "";
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
        public string GetName()
        {
            return this.ConstantName;
        }

        public void AddComment(CultureInfo cultureInfo, string commentText)
        {
            this.Comment[cultureInfo] = commentText;
        }
    }
}
