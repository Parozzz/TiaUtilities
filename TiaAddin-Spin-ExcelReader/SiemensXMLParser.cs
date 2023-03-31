using SpinXmlReader.TagTable;
using System.Xml;

namespace SpinXmlReader
{
    public static class SiemensXMLParser
    {

        public static IXMLNodeSerializable ParseXML(XmlDocument document)
        {

            var tagTableNode = XMLUtils.GetFirstChild(document.DocumentElement, "SW.Tags.PlcTagTable");
            if (tagTableNode != null)
            {
                var tagTable = new XMLTagTable(document);
                tagTable.ParseNode(tagTableNode);
                return tagTable;
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
    }
}
