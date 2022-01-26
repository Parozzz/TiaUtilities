using Siemens.Engineering.SW.Blocks;
using SpinAddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.BlockData;

namespace TiaAddin_Spin_ExcelReader
{
    public class CompileUnit : XmlNodeSerializable, GlobalIDObject
    {
        public MultilingualText Title { get; private set; }
        public MultilingualText Comment { get; private set; }
        public ProgrammingLanguage ProgrammingLanguage { get; private set; }
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

            var convOK = true;
            convOK &= Enum.TryParse(attributeListNode["ProgrammingLanguage"]?.InnerText, true, out ProgrammingLanguage language);
            if(!convOK)
            {
                return null;
            }

            ProgrammingLanguage = language;
            Title = new MultilingualText().ParseFromParent(objectListNode, "Title");
            Comment = new MultilingualText().ParseFromParent(objectListNode, "Comment");

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

        public uint GetID()
        {
            throw new NotImplementedException();
        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
