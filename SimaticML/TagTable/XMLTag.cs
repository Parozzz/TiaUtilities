using System.Globalization;
using System.Xml;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;

namespace SimaticML.TagTable
{
    public class XMLTag : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTag";
        public static XMLTag? CreateTag(XmlNode node)
        {
            return node.Name == XMLTag.NODE_NAME ? new XMLTag() : null;
        }
        public MultilingualText Comment { get => comment; }
        public string TagName { get => tagName.AsString; set => tagName.AsString = value; }
        public SimaticDataType DataType { get => SimaticDataType.FromSimaticMLString(dataTypeName.AsString); set => dataTypeName.AsString = value.SimaticMLString; }


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
            return logicalAddress.AsString;
        }

        public XMLTag SetLogicalAddress(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            logicalAddress.AsString = "%" + memoryArea.GetSimaticMLString() + memoryByte + "." + memoryBit;
            return this;
        }

        public XMLTag SetBoolean(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            this.DataType = SimaticDataType.BOOLEAN;
            logicalAddress.AsString = "%" + memoryArea.GetSimaticMLString() + memoryByte + "." + memoryBit;
            return this;
        }

        public XMLTag SetComplex(SimaticMemoryArea memoryArea, SimaticDataType dataType, uint memoryByte)
        {
            this.DataType = dataType;
            logicalAddress.AsString = "%" + memoryArea.GetSimaticMLString() + dataType.GetSimaticLengthIdentifier() + memoryByte;
            return this;
        }
    }
}
