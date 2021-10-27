using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class CompileUnit
    {
        public MultilingualText Title { get; private set; }
        public MultilingualText Comment { get; private set; }
        public string ProgrammingLanguage { get; private set; }
        public FlagNet Net { get; private set; }

        private readonly FCData fcData;

        internal CompileUnit(FCData fcData)
        {
            this.fcData = fcData;
        }

        internal CompileUnit ParseXmlNode(XmlNode node)
        {
            var attributeListNode = node.SelectSingleNode("AttributeList");
            var objectListNode = node.SelectSingleNode("ObjectList");
            if(attributeListNode == null || objectListNode == null)
            {
                return null;
            }

            Title = new MultilingualText().ParseXMLNode(objectListNode.SelectSingleNode("MultilingualText[@CompositionName = \"Title\"]"));
            Comment = new MultilingualText().ParseXMLNode(objectListNode.SelectSingleNode("MultilingualText[@CompositionName = \"Comment\"]"));

            ProgrammingLanguage = attributeListNode.SelectSingleNode("ProgrammingLanguage")?.InnerText ?? "";

            var networkSourceNode = attributeListNode.SelectSingleNode("NetworkSource");
            if (networkSourceNode != null)
            {
                foreach (XmlNode networkSourceChildNode in networkSourceNode.ChildNodes)
                {
                    switch (networkSourceChildNode.Name)
                    {
                        case "StructuredText":
                            break;
                        case "FlgNet":
                            Net = new FlagNet(fcData).ParseXMLNode(networkSourceChildNode);
                            break;
                    }
                }
            }

            return this;
        }
    }
}
