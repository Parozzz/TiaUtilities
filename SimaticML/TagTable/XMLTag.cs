using SimaticML.API;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;
using System.Globalization;

namespace SimaticML.TagTable
{
    public class XMLTag : XmlNodeConfiguration, IGlobalObject, ISimaticVariableDataHolder
    {
        public const string NODE_NAME = "SW.Tags.PlcTag";

        public MultilingualText Comment { get => comment; }
        public string TagName { get => tagName.Value.AsString; set => tagName.Value.AsString = value; }
        public SimaticDataType DataType { get => SimaticDataType.FromSimaticMLString(dataTypeName.Value.AsString); set => dataTypeName.Value.AsString = value.SimaticMLString; }


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

            this.AddAttribute(SimaticMLAPI.COMPOSITION_NAME_KEY, required: true, requiredValue: "Tags");

            var attributeList = this.AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            dataTypeName = attributeList.AddNode("DataTypeName", required: true, defaultInnerText: "bool");
            logicalAddress = attributeList.AddNode("LogicalAddress", required: true, defaultInnerText: "%M0.0");
            tagName = attributeList.AddNode("Name", required: true, defaultInnerText: "DefaultTagName");

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

        public string GetLogicalAddress()
        {
            return logicalAddress.Value.AsString;
        }

        public XMLTag SetLogicalAddress(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            logicalAddress.Value.AsString = "%" + memoryArea.GetSimaticMLString() + memoryByte + "." + memoryBit;
            return this;
        }

        public XMLTag SetBoolean(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            this.DataType = SimaticDataType.BOOLEAN;
            logicalAddress.Value.AsString = "%" + memoryArea.GetSimaticMLString() + memoryByte + "." + memoryBit;
            return this;
        }

        public XMLTag SetComplex(SimaticMemoryArea memoryArea, SimaticDataType dataType, uint memoryByte)
        {
            this.DataType = dataType;
            logicalAddress.Value.AsString = "%" + memoryArea.GetSimaticMLString() + dataType.GetSimaticLengthIdentifier() + memoryByte;
            return this;
        }

        public string GetName()
        {
            return this.TagName;
        }

        public void AddComment(CultureInfo cultureInfo, string commentText)
        {
            this.Comment[cultureInfo] = commentText;
        }
    }
}
