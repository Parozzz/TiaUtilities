using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.TagTable;
using TiaXmlReader.XMLClasses;
using System.IO;

namespace TiaXmlReader.SimaticML
{
    public static class SimaticMLParser
    {

        public static XmlNodeConfiguration ParseFile(string filePath)
        {
            if(!File.Exists(filePath) || Path.GetExtension(filePath) != ".xml")
            {
                return null;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            return SimaticMLParser.ParseXML(xmlDocument);
        }

        public static XmlNodeConfiguration ParseXML(XmlDocument document)
        {
            var tagTableNode = XMLUtils.GetFirstChild(document.DocumentElement, XMLTagTable.NODE_NAME);
            if (tagTableNode != null)
            {
                var tagTable = new XMLTagTable();
                tagTable.Load(tagTableNode);
                return tagTable;
            }

            var blockFCNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockFC.NODE_NAME);
            if (blockFCNode != null)
            {
                var blockFC  = new BlockFC();
                blockFC.Load(blockFCNode);
                return blockFC;
            }

            var blockFBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockFB.NODE_NAME);
            if (blockFBNode != null)
            {
                var blockFB = new BlockFB();
                blockFB.Load(blockFBNode);
                return blockFB;
            }

            var globalDBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockGlobalDB.NODE_NAME);
            if (globalDBNode != null)
            {
                var blockGlobalDB = new BlockGlobalDB();
                blockGlobalDB.Load(globalDBNode);
                return blockGlobalDB;
            }

            var instanceDBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockInstanceDB.NODE_NAME);
            if (instanceDBNode != null)
            {
                var blockInstanceDB = new BlockInstanceDB();
                blockInstanceDB.Load(instanceDBNode);
                return blockInstanceDB;
            }

            return null;
        }

        public static XmlDocument CreateDocument()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateProcessingInstruction("xml", "version=\"1.0\" encoding =\"utf-8\""));

            XmlElement root = xmlDocument.CreateElement("Document");
            xmlDocument.AppendChild(root);

            var engineering = xmlDocument.CreateElement("Engineering");

            var versionAttribute = xmlDocument.CreateAttribute("version");
            versionAttribute.Value = Constants.GET_STRING_VERSION();
            engineering.Attributes.Append(versionAttribute);

            root.AppendChild(engineering);

            return xmlDocument;
        }

        public static XMLTagTable CreateEmptyTagTable()
        {
            return new XMLTagTable();
        }

    }
}
