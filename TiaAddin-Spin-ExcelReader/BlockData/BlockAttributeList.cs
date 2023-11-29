using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class BlockAttributeList : XmlNodeConfiguration
    {
        //private readonly BlockInterface blockInterface;
        private static Section CreateSection(XmlNode node)
        {
            return node.Name == Section.NODE_NAME ? new Section() : null;
        }

        private readonly XmlNodeConfiguration blockInterface;
        private readonly XmlNodeListConfiguration<Section> blockSections;

        private readonly XmlNodeConfiguration memoryLayout;
        private readonly XmlNodeConfiguration name;
        private readonly XmlNodeConfiguration number;
        private readonly XmlNodeConfiguration programmingLanguage;
        private readonly XmlNodeConfiguration setENOAutomatically;

        public BlockAttributeList() : base(Constants.ATTRIBUTE_LIST_KEY, required: true)
        {
            //==== INIT CONFIGURATION ====
            blockInterface = this.AddNode("Interface", required: true);
            blockSections = blockInterface.AddNodeList("Sections", BlockAttributeList.CreateSection, required: true, namespaceURI: Constants.GET_SECTIONS_NAMESPACE());

            memoryLayout = this.AddNode("MemoryLayout",               required: true, defaultInnerText: "Optimized");
            name = this.AddNode("Name",                               required: true, defaultInnerText: "fcTest");
            if(Constants.VERSION == "V18")
            {
                this.AddNode("Namespace",                             required: true);
            }
            number = this.AddNode("Number",                           required: true, defaultInnerText: "1");
            programmingLanguage = this.AddNode("ProgrammingLanguage", required: true, defaultInnerText: "LAD");
            setENOAutomatically = this.AddNode("SetENOAutomatically", required: true, defaultInnerText: "false");
            //==== INIT CONFIGURATION ====
        }

        public string GetBlockMemoryLayout()
        {
            return memoryLayout.GetInnerText();
        }

        public string GetBlockName()
        {
            return name.GetInnerText();
        }

        public string GetBlockNumber()
        {
            return number.GetInnerText();
        }

        public string GetBlockProgrammingLanguage()
        {
            return programmingLanguage.GetInnerText();
        }

        public string GetBlockSetENOAutomatically()
        {
            return setENOAutomatically.GetInnerText();
        }
        
        public Section ComputeSection(SectionTypeEnum sectionType)
        {
            foreach(var item in blockSections.GetItems())
            {
                if(item.GetSectionType() == sectionType)
                {
                    return item;
                }
            }

            var section = new Section(sectionType);
            blockSections.GetItems().Add(section);
            return section;
        }

        public ICollection<Section> GetBlockSections()
        {
            return blockSections.GetItems();
        }

        public Section GetBlockSection(SectionTypeEnum sectionType) //Nullable
        {
            foreach(var item in blockSections.GetItems())
            {
                if(item.GetSectionType() == sectionType)
                {
                    return item;
                }
            }

            return null;
        }

    }
}
