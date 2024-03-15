using SpinXmlReader;
using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;

namespace TiaXmlReader.SimaticML.nBlockAttributeList
{
    public class BlockAttributeList : XmlNodeConfiguration
    {
        private readonly XmlNodeConfiguration dbAccessibleFromOPCUA;
        private readonly XmlNodeConfiguration dbAccessibleFromWebserver;

        private readonly XmlNodeConfiguration headerAuthor;
        private readonly XmlNodeConfiguration headerFamily;
        private readonly XmlNodeConfiguration headerName;
        private readonly XmlNodeConfiguration headerVersion;

        private readonly XmlNodeConfiguration autoNumber;
        private readonly XmlNodeConfiguration instanceOfName; //ONLY FOR InstanceDB
        private readonly XmlNodeConfiguration instanceOfType; //ONLY FOR InstanceDB

        private readonly XmlNodeConfiguration blockInterface;
        private readonly XmlNodeListConfiguration<Section> blockSections;

        private readonly XmlNodeConfiguration secondaryType; //FOR OBS
        private readonly XmlNodeConfiguration assignedProDiagFB;
        private readonly XmlNodeConfiguration supervisions;
        private readonly XmlNodeConfiguration isOnlyStoredInLoadMemory;
        private readonly XmlNodeConfiguration isWriteProtectedInAS;
        private readonly XmlNodeConfiguration isRetainMemResEnabled;
        private readonly XmlNodeConfiguration isIECCheckEnabled;
        private readonly XmlNodeConfiguration memoryLayout;
        private readonly XmlNodeConfiguration memoryReserve;
        private readonly XmlNodeConfiguration retainMemoryReserve;
        private readonly XmlNodeConfiguration parameterPassing;
        private readonly XmlNodeConfiguration blockName;
        private readonly XmlNodeConfiguration blockNumber;
        private readonly XmlNodeConfiguration programmingLanguage;
        private readonly XmlNodeConfiguration setENOAutomatically;
        private readonly XmlNodeConfiguration libraryConformanceStatus;
        private readonly XmlNodeConfiguration udaBlockProperties;
        private readonly XmlNodeConfiguration udaEnableTagReadback;

        public BlockAttributeList() : base(Constants.ATTRIBUTE_LIST_KEY, required: true)
        {
            //==== INIT CONFIGURATION ====
            dbAccessibleFromOPCUA = this.AddNode("DBAccessibleFromOPCUA");
            dbAccessibleFromWebserver = this.AddNode("DBAccessibleFromWebserver");
            headerAuthor = this.AddNode("HeaderAuthor");
            headerFamily = this.AddNode("HeaderFamily");
            headerName = this.AddNode("HeaderName");
            headerVersion = this.AddNode("HeaderVersion");

            autoNumber = this.AddNode("AutoNumber");
            instanceOfName = this.AddNode("InstanceOfName");
            instanceOfType = this.AddNode("InstanceOfType");

            blockInterface = this.AddNode("Interface", required: true);
            blockSections = blockInterface.AddNodeList("Sections", this.CreateSection, required: true, namespaceURI: Constants.GET_SECTIONS_NAMESPACE());

            secondaryType = this.AddNode("SecondaryType");
            assignedProDiagFB = this.AddNode("AssignedProDiagFB");
            supervisions = this.AddNode("Supervisions");
            isOnlyStoredInLoadMemory = this.AddNode("IsOnlyStoredInLoadMemory");
            isWriteProtectedInAS = this.AddNode("IsWriteProtectedInAS");
            isRetainMemResEnabled = this.AddNode("IsRetainMemResEnabled");
            isIECCheckEnabled = this.AddNode("IsIECCheckEnabled");
            memoryLayout = this.AddNode("MemoryLayout");
            memoryReserve = this.AddNode("MemoryReserve");
            retainMemoryReserve = this.AddNode("RetainMemoryReserve");
            parameterPassing = this.AddNode("ParameterPassing");
            blockName = this.AddNode("Name", required: true, defaultInnerText: "fcTest");
            if (Constants.VERSION >= 18)
            {
                this.AddNode("Namespace", required: true);
            }
            blockNumber = this.AddNode("Number", required: true, defaultInnerText: "1");
            programmingLanguage = this.AddNode("ProgrammingLanguage", required: true, defaultInnerText: "LAD");
            setENOAutomatically = this.AddNode("SetENOAutomatically");
            libraryConformanceStatus = this.AddNode("LibraryConformanceStatus");
            udaBlockProperties = this.AddNode("UDABlockProperties");
            udaEnableTagReadback = this.AddNode("UDAEnableTagReadback");
            //==== INIT CONFIGURATION ====
        }

        private Section CreateSection(XmlNode node)
        {
            return node.Name == Section.NODE_NAME ? new Section() : null;
        }

        public string GetHeaderAuthor()
        {
            return this.headerAuthor.GetInnerText();
        }

        public BlockAttributeList SetHeaderAuthor(string headerAuthor)
        {
            this.headerAuthor.SetInnerText(headerAuthor);
            return this;
        }

        public string GetHeaderFamily()
        {
            return this.headerFamily.GetInnerText();
        }

        public BlockAttributeList SetHeaderFamily(string headerFamily)
        {
            this.headerFamily.SetInnerText(headerFamily);
            return this;
        }

        public string GetHeaderName()
        {
            return this.headerName.GetInnerText();
        }

        public BlockAttributeList SetHeaderName(string headerName)
        {
            this.headerName.SetInnerText(headerName);
            return this;
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

        public BlockAttributeList SetInstanceOfName(string name)
        {
            this.instanceOfName.SetInnerText(name);
            return this;
        }

        public string GetInstanceOfType()
        {
            return instanceOfType.GetInnerText();
        }

        public BlockAttributeList SetInstanceOfType(string type)
        {
            this.instanceOfType.SetInnerText(type);
            return this;
        }

        public string GetBlockMemoryLayout()
        {
            return memoryLayout.GetInnerText();
        }

        public BlockAttributeList SetBlockMemoryLayout(string memorylaout)
        {
            this.memoryLayout.SetInnerText(memorylaout);
            return this;
        }

        public uint GetBlockMemoryReserve()
        {
            return string.IsNullOrEmpty(memoryReserve.GetInnerText()) ? 0 : uint.Parse(memoryReserve.GetInnerText());
        }

        public BlockAttributeList SetBlockMemoryReserve(uint memoryReserve)
        {
            this.memoryReserve.SetInnerText("" + memoryReserve);
            return this;
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

        public SimaticProgrammingLanguage GetBlockProgrammingLanguage()
        {
            return SimaticProgrammingLanguageUtil.GetFromSimaticMLString(programmingLanguage.GetInnerText());
        }

        public BlockAttributeList SetBlockProgrammingLanguage(SimaticProgrammingLanguage programmingLanguage)
        {
            this.programmingLanguage.SetInnerText(programmingLanguage.GetSimaticMLString());
            return this;
        }

        public bool GetBlockSetENOAutomatically()
        {
            return !string.IsNullOrEmpty(setENOAutomatically.GetInnerText()) && bool.Parse(setENOAutomatically.GetInnerText());
        }

        public BlockAttributeList SetBlockSetENOAutomatically(bool setENOAutomatically)
        {
            this.setENOAutomatically.SetInnerText(setENOAutomatically.ToString().ToLower()); //HE WANTS LOWERCASE!
            return this;
        }

        public string GetUDABlockProperties()
        {
            return this.udaBlockProperties.GetInnerText();
        }

        public BlockAttributeList SetUDABlockProperties(string udaBlockProperties)
        {
            this.udaEnableTagReadback.SetInnerText(string.IsNullOrEmpty(udaBlockProperties) ? "false" : "true");
            this.udaBlockProperties.SetInnerText(udaBlockProperties);
            return this;
        }

        public Section ComputeSection(SectionTypeEnum sectionType)
        {
            foreach (var item in blockSections.GetItems())
            {
                if (item.GetSectionType() == sectionType)
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
    }
}
