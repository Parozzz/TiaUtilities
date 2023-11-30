using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader
{
    public static class SiemensMLParser
    {

        public static XmlNodeConfiguration ParseXML(XmlDocument document)
        {

            var tagTableNode = XMLUtils.GetFirstChild(document.DocumentElement, "SW.Tags.PlcTagTable");
            if (tagTableNode != null)
            {
                var tagTable = new XMLTagTable();
                tagTable.Parse(tagTableNode);
                return tagTable;
            }

            var blockFCNode = XMLUtils.GetFirstChild(document.DocumentElement, "SW.Blocks.FC");
            if (blockFCNode != null)
            {
                var blockFC  = new BlockFC();
                blockFC.Parse(blockFCNode);
                return blockFC;
            }

            var blockFBNode = XMLUtils.GetFirstChild(document.DocumentElement, "SW.Blocks.FB");
            if (blockFBNode != null)
            {
                var blockFB = new BlockFB();
                blockFB.Parse(blockFBNode);
                return blockFB;
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
