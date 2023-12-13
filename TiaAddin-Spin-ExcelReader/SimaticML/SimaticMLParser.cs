using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.SimaticML
{
    public static class SimaticMLParser
    {

        public static XmlNodeConfiguration ParseXML(XmlDocument document)
        {
            var tagTableNode = XMLUtils.GetFirstChild(document.DocumentElement, XMLTagTable.NODE_NAME);
            if (tagTableNode != null)
            {
                var tagTable = new XMLTagTable();
                tagTable.Parse(tagTableNode, new IDGenerator());
                return tagTable;
            }

            var blockFCNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockFC.NODE_NAME);
            if (blockFCNode != null)
            {
                var blockFC  = new BlockFC();
                blockFC.Parse(blockFCNode, new IDGenerator());
                return blockFC;
            }

            var blockFBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockFB.NODE_NAME);
            if (blockFBNode != null)
            {
                var blockFB = new BlockFB();
                blockFB.Parse(blockFBNode, new IDGenerator());
                return blockFB;
            }

            var globalDBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockGlobalDB.NODE_NAME);
            if (globalDBNode != null)
            {
                var blockGlobalDB = new BlockGlobalDB();
                blockGlobalDB.Parse(globalDBNode, new IDGenerator());
                return blockGlobalDB;
            }

            var instanceDBNode = XMLUtils.GetFirstChild(document.DocumentElement, BlockInstanceDB.NODE_NAME);
            if (instanceDBNode != null)
            {
                var blockInstanceDB = new BlockInstanceDB();
                blockInstanceDB.Parse(instanceDBNode, new IDGenerator());
                return blockInstanceDB;
            }

            /*
            var fcNode = XmlUtil.GetFirstChild(document, "SW.Blocks.FC");
            if (fcNode != null)
            {
                var fcData = new FCData(document);
                fcData.ParseXMLDocument();
                return;
            }

            var fbNode = XmlUtil.GetFirstChild(document, "SW.Blocks.FB");
            if (fcNode != null)
            {
                return;
            }*/

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
