using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Siemens.Engineering.SW.Blocks;

namespace SpinAddIn.BlockData
{
    public class CompileUnit
    {
        public MultilingualText Title { get; private set; }
        public MultilingualText Comment { get; private set; }
        public ProgrammingLanguage ProgrammingLanguage { get; private set; }
        public FlagNet Net { get; private set; }

        internal CompileUnit()
        {

        }

        internal CompileUnit ParseXmlNode(XmlNode node)
        {
            var attributeListNode = node.SelectSingleNode(".//AttributeList");

            var networkSourceNode = attributeListNode?.SelectSingleNode(".//NetworkSource");
            if (networkSourceNode != null)
            {
                foreach (XmlNode networkSourceChildNode in networkSourceNode.ChildNodes)
                {
                    switch (networkSourceChildNode.Name)
                    {
                        case "StructuredText":
                            break;
                        case "FlgNet":
                            Net = new FlagNet();
                            break;
                    }
                }
            }

            var programmingLanguangeNode = attributeListNode?.SelectSingleNode(".//ProgrammingLanguage");
            if (programmingLanguangeNode != null && Enum.TryParse(programmingLanguangeNode.Value, true, out ProgrammingLanguage language))
            {
                ProgrammingLanguage = language;
            }

            var multilingualTextList = node?.SelectNodes(".//ObjectList[./MultilingualText]");
            if (multilingualTextList != null)
            {
                foreach (XmlNode multilingualTextNode in multilingualTextList)
                {
                    var multilingualText = new MultilingualText().ParseXMLNode(multilingualTextNode);
                    switch (multilingualText.CompositionName)
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

            return this;
        }
    }
}
