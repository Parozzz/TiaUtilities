using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class BlockAttributeList
    {
        private readonly BlockInterface blockInterface;

        private string memoryLayout;
        public string MemoryLayout { get => memoryLayout; set => memoryLayout = value; }

        private string name;
        public string Name { get => name; set => name = value; }

        private int number;
        public int Number { get => number; set => number = value; }

        private string programmingLanguage;
        public string ProgrammingLanguage { get => programmingLanguage; set => programmingLanguage = value; }

        private bool setENOAutomatically;
        public bool SetENOAutomatically { get => setENOAutomatically; set => setENOAutomatically = value; }


        public BlockAttributeList()
        {
            blockInterface = new BlockInterface();
        }

        public BlockInterface GetBlockInterface()
        {
            return blockInterface;
        }

        public void DoXmlNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("AttributeList"), "BlockAttributeList node name is not valid.");

            blockInterface.DoXmlNode(XmlSearchEngine.Of(node).GetFirstNode("Interface"));

            bool parseValid = Util.TryNotNull(XmlSearchEngine.Of(node).GetFirstNode("MemoryLayout")?.InnerText, out this.memoryLayout);
            parseValid &= Util.TryNotNull(XmlSearchEngine.Of(node).GetFirstNode("Name")?.InnerText, out this.name);
            parseValid &= int.TryParse(XmlSearchEngine.Of(node).GetFirstNode("Number")?.InnerText, out this.number);
            parseValid &= Util.TryNotNull(XmlSearchEngine.Of(node).GetFirstNode("ProgrammingLanguage")?.InnerText, out this.programmingLanguage);
            parseValid &= bool.TryParse(XmlSearchEngine.Of(node).GetFirstNode("SetENOAutomatically")?.InnerText, out this.setENOAutomatically);

            if(!parseValid)
            {
                throw new InvalidOperationException("Something is not been parsed correctly while parsing BlockAttributeList.");
            }
        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            var mainNode = document.CreateNode(XmlNodeType.Element, "AttributeList", "");

            mainNode.AppendChild(XmlUtil.CreateElementNode(document, "Name", this.name));
            mainNode.AppendChild(XmlUtil.CreateElementNode(document, "MemoryLayout", this.memoryLayout));
            mainNode.AppendChild(XmlUtil.CreateElementNode(document, "Number", this.number));
            mainNode.AppendChild(XmlUtil.CreateElementNode(document, "ProgrammingLanguage", this.programmingLanguage));
            mainNode.AppendChild(XmlUtil.CreateElementNode(document, "SetENOAutomatically", this.setENOAutomatically));


            mainNode.AppendChild(blockInterface.GenerateXmlNode(document));

            return mainNode;
        }

    }
}
