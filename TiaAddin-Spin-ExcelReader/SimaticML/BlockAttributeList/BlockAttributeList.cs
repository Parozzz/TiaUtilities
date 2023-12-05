using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class BlockAttributeList : XmlNodeConfiguration
    {
        private readonly XmlNodeConfiguration autoNumber;
        private readonly XmlNodeConfiguration instanceOfName; //ONLY FOR InstanceDB
        private readonly XmlNodeConfiguration instanceOfType; //ONLY FOR InstanceDB

        private readonly XmlNodeConfiguration blockInterface;
        private readonly XmlNodeListConfiguration<Section> blockSections;

        private readonly XmlNodeConfiguration memoryLayout;
        private readonly XmlNodeConfiguration memoryReserve;
        private readonly XmlNodeConfiguration blockName;
        private readonly XmlNodeConfiguration blockNumber;
        private readonly XmlNodeConfiguration programmingLanguage;
        private readonly XmlNodeConfiguration setENOAutomatically;

        private readonly XmlNodeConfiguration parentConfiguration;

        public BlockAttributeList(XmlNodeConfiguration parentConfiguration) : base(Constants.ATTRIBUTE_LIST_KEY, required: true)
        {
            this.parentConfiguration = parentConfiguration;

            //==== INIT CONFIGURATION ====
            autoNumber = this.AddNode("AutoNumber");
            instanceOfName = this.AddNode("InstanceOfName");
            instanceOfType = this.AddNode("InstanceOfType");


            blockInterface = this.AddNode("Interface",                                               required: true);
            blockSections = blockInterface.AddNodeList("Sections", this.CreateSection, required: true, namespaceURI: Constants.GET_SECTIONS_NAMESPACE());

            memoryLayout = this.AddNode("MemoryLayout",                                              required: true,  defaultInnerText: "Optimized");
            memoryReserve = this.AddNode("MemoryReserve",                                            required: false);
            blockName = this.AddNode("Name",                                                         required: true, defaultInnerText: "fcTest");
            if(Constants.VERSION >= 18)                                                              
            {                                                                                        
                this.AddNode("Namespace",                                                            required: true);
            }                                                                                        
            blockNumber = this.AddNode("Number",                                                     required: true, defaultInnerText: "1");
            programmingLanguage = this.AddNode("ProgrammingLanguage",                                required: true, defaultInnerText: "LAD");
            setENOAutomatically = this.AddNode("SetENOAutomatically",                                required: false);
            //==== INIT CONFIGURATION ====
        }
        private Section CreateSection(XmlNode node)
        {
            return node.Name == Section.NODE_NAME ? new Section(this) : null;
        }

        public XmlNodeConfiguration GetParentConfiguration()
        {
            return parentConfiguration;
        }

        public bool GetAutoNumber()
        {
            return string.IsNullOrEmpty(autoNumber.GetInnerText()) && bool.Parse(autoNumber.GetInnerText());
        }

        public BlockAttributeList SetAutoNumber(bool autoNumber)
        {
            this.autoNumber.SetInnerText(autoNumber.ToString().ToLower()); //HE WANTS LOWERCASE!
            return this;
        }

        public string GetInstanceOfName()
        {
            return instanceOfName.GetInnerText();
        }

        public void SetInstanceOfName(string name)
        {
            this.instanceOfName.SetInnerText(name);
        }

        public string GetInstanceOfType()
        {
            return instanceOfType.GetInnerText();
        }

        public void SetInstanceOfType(string type)
        {
            this.instanceOfType.SetInnerText(type);
        }

        public string GetBlockMemoryLayout()
        {
            return memoryLayout.GetInnerText();
        }

        public void SetBlockMemoryLayout(string memorylaout)
        {
            this.memoryLayout.SetInnerText(memorylaout);
        }

        public uint GetBlockMemoryReserve()
        {
            return string.IsNullOrEmpty(memoryReserve.GetInnerText()) ? 0 : uint.Parse(memoryReserve.GetInnerText());
        }

        public void SetBlockMemoryReserve(uint memoryReserve)
        {
            this.memoryReserve.SetInnerText("" + memoryReserve);
        }

        public string GetBlockName()
        {
            return blockName.GetInnerText();
        }

        public BlockAttributeList SetBlockName(string name)
        {
            this.blockName.SetInnerText(name);
            return this;
        }

        public uint GetBlockNumber()
        {
            return uint.Parse(blockNumber.GetInnerText());
        }

        public BlockAttributeList SetBlockNumber(uint number)
        {
            this.blockNumber.SetInnerText(number.ToString());
            return this;
        }

        public string GetBlockProgrammingLanguage()
        {
            return programmingLanguage.GetInnerText();
        }

        public void SetBlockProgrammingLanguage(string programmingLanguage)
        {
            this.programmingLanguage.SetInnerText(programmingLanguage);
        }

        public bool GetBlockSetENOAutomatically()
        {
            return !string.IsNullOrEmpty(setENOAutomatically.GetInnerText()) && bool.Parse(setENOAutomatically.GetInnerText());
        }
        
        public void SetBlockSetENOAutomatically(bool setENOAutomatically)
        {
            this.setENOAutomatically.SetInnerText(setENOAutomatically.ToString().ToLower()); //HE WANTS LOWERCASE!
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

            var section = new Section(this, sectionType);
            blockSections.GetItems().Add(section);
            return section;
        }

        public ICollection<Section> GetBlockSections()
        {
            return blockSections.GetItems();
        }
    }
}
