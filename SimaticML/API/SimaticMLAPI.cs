using SimaticML.Blocks;
using SimaticML.TagTable;
using SimaticML.XMLClasses;
using System.Globalization;
using System.Xml;

namespace SimaticML.API
{
    public static class SimaticMLAPI
    {

        public const string SECTIONS_NAMESPACE_V16 = "http://www.siemens.com/automation/Openness/SW/Interface/v4";
        public const string SECTIONS_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/Interface/v5";

        public const string FLG_NET_NAMESPACE_V16 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3";
        public const string FLG_NET_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4";
        public const string FLG_NET_NAMESPACE_V19 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v5";

        public const string STRUCTURED_TEXT_NAMESPACE_V16 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3";
        public const string STRUCTURED_TEXT_NAMESPACE_V19 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v4";

        public const string GLOBAL_ID_KEY = "ID";
        public const string COMPOSITION_NAME_KEY = "CompositionName";
        public const string ATTRIBUTE_LIST_KEY = "AttributeList";
        public const string OBJECT_LIST_KEY = "ObjectList";

        public const string DEFAULT_EMPTY_MEMBER_NAME = "_";

        public const string HEADER_AUTHOR = "Giacomo P.";
        public const string HEADER_FAMILY = "SPIN_SRL"; //DO NOT USE SPACE
        public const string UDA_BLOCK_PROPERTIES = "GENERATED_BY_XMLREADER";

        public static CultureInfo CULTURE { get; set; } = CultureInfo.CurrentCulture;
        public static uint TIA_VERSION { get; set; } = 17;

        public static string GET_SECTIONS_NAMESPACE()
        {
            if (TIA_VERSION >= 17)
            {
                return SECTIONS_NAMESPACE_V17;
            }
            else if (TIA_VERSION >= 16)
            {
                return SECTIONS_NAMESPACE_V16;
            }

            return null;
        }

        public static string GET_FLAG_NET_NAMESPACE()
        {
            if (TIA_VERSION >= 19)
            {
                return FLG_NET_NAMESPACE_V19;
            }
            else if (TIA_VERSION >= 17)
            {
                return FLG_NET_NAMESPACE_V17;
            }
            else if (TIA_VERSION >= 16)
            {
                return FLG_NET_NAMESPACE_V16;
            }

            return null;
        }

        public static string GET_STRUCTRED_TEXT_NAMESPACE()
        {
            if (TIA_VERSION >= 19)
            {
                return STRUCTURED_TEXT_NAMESPACE_V19;
            }
            else if (TIA_VERSION >= 16)
            {
                return STRUCTURED_TEXT_NAMESPACE_V16;
            }

            return null;
        }

        public static XmlNodeConfiguration? ParseFile(string filePath)
        {
            if (!File.Exists(filePath) || Path.GetExtension(filePath) != ".xml")
            {
                return null;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            return ParseXML(xmlDocument);
        }

        public static XmlNodeConfiguration? ParseXML(XmlDocument document)
        {
            var element = document.DocumentElement;
            if (element == null)
            {
                return null;
            }

            var tagTableNode = XMLUtils.GetFirstChild(element, XMLTagTable.NODE_NAME);
            if (tagTableNode != null)
            {
                var tagTable = new XMLTagTable();
                tagTable.Load(tagTableNode);
                return tagTable;
            }

            var blockFCNode = XMLUtils.GetFirstChild(element, BlockFC.NODE_NAME);
            if (blockFCNode != null)
            {
                var blockFC = new BlockFC();
                blockFC.Load(blockFCNode);
                return blockFC;
            }

            var blockFBNode = XMLUtils.GetFirstChild(element, BlockFB.NODE_NAME);
            if (blockFBNode != null)
            {
                var blockFB = new BlockFB();
                blockFB.Load(blockFBNode);
                return blockFB;
            }

            var globalDBNode = XMLUtils.GetFirstChild(element, BlockGlobalDB.NODE_NAME);
            if (globalDBNode != null)
            {
                var blockGlobalDB = new BlockGlobalDB();
                blockGlobalDB.Load(globalDBNode);
                return blockGlobalDB;
            }

            var instanceDBNode = XMLUtils.GetFirstChild(element, BlockInstanceDB.NODE_NAME);
            if (instanceDBNode != null)
            {
                var blockInstanceDB = new BlockInstanceDB();
                blockInstanceDB.Load(instanceDBNode);
                return blockInstanceDB;
            }

            return null;
        }

        public static XmlDocument CreateDocument(XmlNodeConfiguration mainNode)
        {
            mainNode.UpdateID_UId(new IDGenerator());

            var document = CreateDocument();

            var xml = mainNode.Generate(document);
            (document.DocumentElement ?? throw new InvalidProgramException()).AppendChild(xml);
            return document;
        }

        public static XmlDocument CreateDocument()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateProcessingInstruction("xml", "version=\"1.0\" encoding =\"utf-8\""));

            XmlElement root = xmlDocument.CreateElement("Document");
            xmlDocument.AppendChild(root);

            var engineering = xmlDocument.CreateElement("Engineering");

            var versionAttribute = xmlDocument.CreateAttribute("version");
            versionAttribute.Value = "V" + TIA_VERSION;
            engineering.Attributes.Append(versionAttribute);

            root.AppendChild(engineering);

            return xmlDocument;
        }
    }
}
