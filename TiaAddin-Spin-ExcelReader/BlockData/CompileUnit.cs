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
            var networkSourceNode = node.SelectSingleNode("./AttributeList/NetworkSource");
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

            ProgrammingLanguage = node.SelectSingleNode("./AttributeList/ProgrammingLanguage")?.InnerText ?? "";

            var titleNode = node?.SelectSingleNode("./ObjectList/MultilingualText[@CompositionName = \"Title\"]");
            Title = new MultilingualText().ParseXMLNode(titleNode);

            var commentNode = node?.SelectSingleNode("./ObjectList/MultilingualText[@CompositionName = \"Comment\"]");
            Comment = new MultilingualText().ParseXMLNode(commentNode);

            return this;
        }
    }
}
