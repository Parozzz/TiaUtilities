using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace SpinXmlReader.Block
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


        public BlockAttributeList(XmlNode xmlNode)
        {
            blockInterface = new BlockInterface(xmlNode);
            this.DoXmlNode(xmlNode);
        }

        public BlockAttributeList(bool isFC)
        {
            blockInterface = new BlockInterface(isFC);
        }

        public BlockInterface GetBlockInterface()
        {
            return blockInterface;
        }

        private void DoXmlNode(XmlNode node)
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
            return XmlNodeBuilder.CreateNew(document, "AttributeList")
                .AppendChild("Name", this.name)
                .AppendChild("MemoryLayout", this.memoryLayout)
                .AppendChild("Number", this.number)
                .AppendChild("ProgrammingLanguage", this.programmingLanguage)
                .AppendChild("SetENOAutomatically", this.setENOAutomatically)
                .AppendSerializableAsChild(blockInterface)
                .GetNode();
        }

    }
}
