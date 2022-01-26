using System.Collections.Generic;
using System.Xml;
using TiaAddin_Spin_ExcelReader.BlockData;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader
{
    public class FCData
    {
        private readonly XmlDocument xmlDocument;
        public readonly GlobalIDGenerator GlobalIDGenerator;
        private readonly BlockAttributeList blockAttributeList;

        public MultilingualText Comment { get; private set; }
        public MultilingualText Title { get; private set; }

        private readonly List<CompileUnit> compileUnitList;

        public FCData(XmlDocument xmlDocument)
        {
            this.xmlDocument = xmlDocument;
            this.GlobalIDGenerator = new GlobalIDGenerator();
            this.blockAttributeList = new BlockAttributeList(true);

            this.compileUnitList = new List<CompileUnit>();
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

            blockInterface.DoXmlNode(interfaceNode);

            // ==============================
            // OBJECT LIST
            //==============================
            Title = new MultilingualText().ParseFromParent(objectListNode, "Title");
            Comment = new MultilingualText().ParseFromParent(objectListNode, "Comment");

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
