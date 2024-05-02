using System.Globalization;
using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.TagTable
{
    public class XMLTag : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Tags.PlcTag";
        public static XMLTag CreateTag(XmlNode node)
        {
            return node.Name == XMLTag.NODE_NAME ? new XMLTag() : null;
        }

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

            this.AddAttribute(Constants.COMPOSITION_NAME_KEY, required: true, requiredValue: "Tags");

            var attributeList = this.AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            dataTypeName = attributeList.AddNode("DataTypeName",           required: true, defaultInnerText: "bool");
            logicalAddress = attributeList.AddNode("LogicalAddress",       required: true, defaultInnerText: "%M0.0");
            tagName = attributeList.AddNode("Name",                        required: true, defaultInnerText: "DefaultTagName");

            objectList = this.AddNode(Constants.OBJECT_LIST_KEY, required: false);
            comment = objectList.AddNode(new MultilingualText(MultilingualTextType.COMMENT));
            //==== INIT CONFIGURATION ====
        }

        public void Init()
        {
            comment.SetText(LocalizationVariables.CULTURE, "");
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public MultilingualText GetComment()
        {
            return comment;
        }

        public SimaticDataType GetDataType()
        {
            return SimaticDataType.FromSimaticMLString(dataTypeName.GetInnerText());
        }

        public XMLTag SetDataType(SimaticDataType dataType)
        {
            dataTypeName.SetInnerText(dataType.GetSimaticMLString());
            return this;
        }

        public string GetLogicalAddress()
        {
            return logicalAddress.GetInnerText();
        }

        public XMLTag SetLogicalAddress(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            logicalAddress.SetInnerText("%" + memoryArea.GetTIAMnemonic() + memoryByte + "." + memoryBit);
            return this;
        }

        public XMLTag SetBoolean(SimaticMemoryArea memoryArea, uint memoryByte, uint memoryBit)
        {
            this.SetDataType(SimaticDataType.BOOLEAN);
            logicalAddress.SetInnerText("%" + memoryArea.GetTIAMnemonic() + memoryByte + "." + memoryBit);
            return this;
        }

        public XMLTag SetComplex(SimaticMemoryArea memoryArea, SimaticDataType dataType, uint memoryByte)
        {
            this.SetDataType(dataType);
            logicalAddress.SetInnerText("%" + memoryArea.GetTIAMnemonic() + dataType.GetSimaticLengthIdentifier() + memoryByte);
            return this;
        }

        public string GetTagName()
        {
            return tagName.GetInnerText();
        }

        public XMLTag SetTagName(string str)
        {
            tagName.SetInnerText(str);
            return this;
        }

        public XMLTag SetCommentText(CultureInfo cultureInfo, string text)
        {
            if(!string.IsNullOrEmpty(text))
            {
                comment.SetText(cultureInfo, text);
            }
            return this;
        }
    }
}
