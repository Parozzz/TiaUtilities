using System.Collections.Generic;
using System.Xml;

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
            // Here i search for the whole document for the first FC Block AttributeList Interface (Because .//)
            var interfaceNode = xmlDocument.SelectSingleNode(".//SW.Blocks.FC/AttributeList/Interface");
            if (interfaceNode != null)
            {
                blockInterface.ParseXmlNode(interfaceNode);
            }

            // Here i search for the whole document for the first FC Block ObjectList (Because .//)
            var objectListNode = xmlDocument.SelectSingleNode(".//SW.Blocks.FC/ObjectList"); 
            Title = new MultilingualText()
                .ParseXMLNode(objectListNode.SelectSingleNode("./MultilingualText[@CompositionName = \"Title\"]"));
            Comment = new MultilingualText()
                .ParseXMLNode(objectListNode.SelectSingleNode("./MultilingualText[@CompositionName = \"Comment\"]"));

            foreach(XmlNode compileUnitNode in objectListNode.SelectNodes("./SW.Blocks.CompileUnit"))
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
