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
        public String MemoryLayout { get { return memoryLayout; } set { memoryLayout = value; } }

        private string name;
        public String Name { get { return name; } set { name = value; } }
        
        private int number;
        public int Number { get { return number; } set { number = value; } }

        private string programmingLanguage;
        public string ProgrammingLanguage { get { return programmingLanguage; } set { programmingLanguage = value; } }

        private bool setENOAutomatically;
        public bool SetENOAutomatically { get { return setENOAutomatically; } set { setENOAutomatically = value; } }


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

            string memoryLayout = XmlSearchEngine.Of(node).GetFirstNode("MemoryLayout")?.InnerText;
            string name = XmlSearchEngine.Of(node).GetFirstNode("Name")?.InnerText;
            string number = XmlSearchEngine.Of(node).GetFirstNode("Number")?.InnerText;
            string programmingLanguage = XmlSearchEngine.Of(node).GetFirstNode("ProgrammingLanguage")?.InnerText;
            string setENOAutomatically = XmlSearchEngine.Of(node).GetFirstNode("SetENOAutomatically")?.InnerText;

            if(memoryLayout == null || name == null || number == null || programmingLanguage == null || setENOAutomatically == null)
            {
                throw new InvalidOperationException("Something is null while parsing BlockAttributeList.");
            }

            bool parseValid = true;

            this.memoryLayout = memoryLayout;
            this.name = name;
            parseValid &= int.TryParse(number, out this.number);
            this.programmingLanguage = programmingLanguage;
            parseValid &= bool.TryParse(setENOAutomatically, out this.setENOAutomatically);

            if(!parseValid)
            {
                throw new InvalidOperationException("Something is not been parsed correctly parsing BlockAttributeList.");
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
