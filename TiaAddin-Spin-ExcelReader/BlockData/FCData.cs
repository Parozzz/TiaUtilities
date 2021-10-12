using System.Collections.Generic;
using System.Xml;

namespace SpinAddIn.BlockData
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
            this.blockInterface = new BlockInterface();

            this.compileUnitList = new List<CompileUnit>();
        }

        public void ParseXMLDocument()
        {
            var mainNode = xmlDocument.SelectSingleNode(".//SW.Blocks.FC");
            if (mainNode != null)
            {

                foreach (XmlNode mainChildNode in mainNode.ChildNodes)
                {
                    if (mainChildNode.Name == "AttributeList")
                    {
                        var attributeListNode = mainChildNode;
                        foreach (XmlNode attributeNode in attributeListNode.ChildNodes)
                        {
                            if (attributeNode.Name == "Interface")
                            {
                                blockInterface.ParseXmlNode(mainChildNode);
                                break;
                            }
                        }

                    }
                    else if (mainChildNode.Name == "ObjectList")
                    {
                        var objectListNode = mainChildNode;
                        foreach (XmlNode objectNode in objectListNode.ChildNodes)
                        {
                            if (objectNode.Name == "SW.Blocks.CompileUnit")
                            {
                                compileUnitList.Add(new CompileUnit().ParseXmlNode(objectNode));
                            }
                            else if (objectNode.Name == "MultilingualText")
                            {
                                var multilingualText = new MultilingualText().ParseXMLNode(objectNode);
                                switch (objectNode.Attributes["CompositionName"].Value)
                                {
                                    case "Comment":
                                        Comment = multilingualText;
                                        break;
                                    case "Title":
                                        Title = multilingualText;
                                        break;
                                }
                            }
                        }
                    }
                }

            }
        }

    }

}
