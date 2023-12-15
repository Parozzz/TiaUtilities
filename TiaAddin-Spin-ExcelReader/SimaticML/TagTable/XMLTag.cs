using DocumentFormat.OpenXml.Wordprocessing;
using SpinXmlReader.SimaticML;
using System.Globalization;
using System.Xml;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;

namespace SpinXmlReader.TagTable
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
            comment.SetText(Constants.DEFAULT_CULTURE, "");
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public MultilingualText GetComment()
        {
            return comment;
        }

        public string GetDataTypeName()
        {
            return dataTypeName.GetInnerText();
        }

        public XMLTag SetDataTypeName(string str)
        {
            dataTypeName.SetInnerText(str);
            return this;
        }

        public string GetLogicalAddress()
        {
            return logicalAddress.GetInnerText();
        }

        public XMLTag SetLogicalAddress(string str)
        {
            logicalAddress.SetInnerText(str);
            return this;
        }

        public XMLTag SetBoolean(SimaticMemoryArea memoryArea, int memoryByte, int memoryBit)
        {
            this.SetDataTypeName(SimaticDataType.BOOLEAN.GetSimaticMLString());
            logicalAddress.SetInnerText("%" + memoryArea.GetTIAMnemonic() + memoryByte + "." + memoryBit);
            return this;
        }

        public XMLTag SetComplex(SimaticMemoryArea memoryArea, SimaticDataType dataType, int memoryByte)
        {
            this.SetDataTypeName(dataType.GetSimaticMLString());
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
