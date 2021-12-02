using System.Collections.Generic;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class FCData
    {
        private readonly XmlDocument xmlDocument;
        private readonly BlockInterface blockInterface;

        public MultilingualText Comment { get; private set; }
        public MultilingualText Title { get; private set; }

        private readonly List<CompileUnit> compileUnitList;

        public FCData(XmlDocument xmlDocument)
        {
            this.xmlDocument = xmlDocument;
            this.blockInterface = new BlockInterface(this);

            this.compileUnitList = new List<CompileUnit>();
        }

        public void ParseXMLDocument()
        {
            var documentElement = xmlDocument.DocumentElement;

            var interfaceNode = XmlSearchEngine.Of(documentElement).AddMultipleSearch("SW.Blocks.FC/AttributeList/Interface").Search(); //search the whole document for the first FC Block AttributeList Interface (Because .//)
            var objectListNode = XmlSearchEngine.Of(documentElement).AddMultipleSearch("SW.Blocks.FC/ObjectList").Search(); //search the whole document for the first FC Block ObjectList (Because .//)
            if (interfaceNode == null || objectListNode == null)
            {
                return;
            }

            // ==============================
            // BLOCK INTERFACE
            //==============================

            blockInterface.ParseXmlNode(interfaceNode);

            // ==============================
            // OBJECT LIST
            //==============================
            Title = new MultilingualText()
                .ParseXMLNode(XmlSearchEngine.Of(objectListNode).AddSearch("MultilingualText").AttributeRequired("CompositionName", "Title").Search());
            Comment = new MultilingualText()
                .ParseXMLNode(XmlSearchEngine.Of(objectListNode).AddSearch("MultilingualText").AttributeRequired("CompositionName", "Comment").Search());

       
            foreach (XmlNode compileUnitNode in XmlUtil.GetAllChild(objectListNode, "SW.Blocks.CompileUnit"))
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
