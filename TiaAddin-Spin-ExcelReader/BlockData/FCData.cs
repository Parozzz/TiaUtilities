using System.Collections.Generic;
using System.Xml;

namespace SpinXmlReader.Block
{
    public class FCData
    {
        private readonly XmlDocument xmlDocument;
        private readonly BlockAttributeList blockAttributeList;

        private readonly MultilingualText title;
        private readonly MultilingualText comment;

        private readonly List<CompileUnit> compileUnitList;

        public FCData(XmlDocument xmlDocument)
        {
            this.xmlDocument = xmlDocument;
            this.blockAttributeList = new BlockAttributeList(true);

            this.compileUnitList = new List<CompileUnit>();

            this.title = new MultilingualText(MultilingualTextType.TITLE);
            this.comment = new MultilingualText(MultilingualTextType.COMMENT);
        }

        public void ParseXMLDocument()
        {
            var documentElement = xmlDocument.DocumentElement;

            var interfaceNode = XmlSearchEngine.Of(documentElement).AddSearch("SW.Blocks.FC/AttributeList/Interface").GetFirstNode(); //search the whole document for the first FC Block AttributeList Interface (Because .//)
            var objectListNode = XmlSearchEngine.Of(documentElement).AddSearch("SW.Blocks.FC/ObjectList").GetFirstNode(); //search the whole document for the first FC Block ObjectList (Because .//)
            if (interfaceNode == null || objectListNode == null)
            {
                return;
            }

            // ==============================
            // BLOCK INTERFACE
            //==============================

            //blockInterface.DoXmlNode(interfaceNode);

            // ==============================
            // OBJECT LIST
            //==============================
            title.ParseFirstFrom(objectListNode);
            comment.ParseFirstFrom(objectListNode);

            foreach (XmlNode compileUnitNode in XmlSearchEngine.Of(objectListNode).AddSearch("SW.Blocks.CompileUnit").GetAllNodes())
            {
                compileUnitList.Add(new CompileUnit(this).ParseXmlNode(compileUnitNode));
            }
        }

        public XmlDocument GetXmlDocument()
        {
            return xmlDocument;
        }
    }

}
