using SimaticML.API;
using SimaticML.Enums;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.nBlockAttributeList
{
    public class BlockAttributeList : XmlNodeConfiguration
    {

        public Section INPUT { get => this.ComputeSection(SectionTypeEnum.INPUT); }
        public Section OUTPUT { get => this.ComputeSection(SectionTypeEnum.OUTPUT); }
        public Section INOUT { get => this.ComputeSection(SectionTypeEnum.INOUT); }
        public Section STATIC { get => this.ComputeSection(SectionTypeEnum.STATIC); }
        public Section TEMP { get => this.ComputeSection(SectionTypeEnum.TEMP); }
        public Section CONSTANT { get => this.ComputeSection(SectionTypeEnum.CONSTANT); }
        public Section RETURN { get => this.ComputeSection(SectionTypeEnum.RETURN); }
        public Section NONE { get => this.ComputeSection(SectionTypeEnum.NONE); }

        public string HeaderAuthor { get => this.headerAuthor.AsString; set => this.headerAuthor.AsString = value; }
        public string HeaderFamily { get => this.headerFamily.AsString; set => this.headerFamily.AsString = value; }
        public string HeaderName { get => this.headerName.AsString; set => this.headerName.AsString = value; }

        public string MemoryLayout { get => this.memoryLayout.AsString; set => this.memoryLayout.AsString = value; }
        public uint MemoryReserve { get => this.memoryReserve.AsUInt; set => this.memoryReserve.AsUInt = value; }
        public bool AutoNumber { get => this.autoNumber.AsBool; set => this.autoNumber.AsBool = value; } //HE WANTS LOWERCASE!
        public string BlockName { get => this.blockName.AsString; set => this.blockName.AsString = value; }
        public uint BlockNumber { get => this.blockNumber.AsUInt; set => this.blockNumber.AsUInt = value; }
        public bool SetENOAutomatically { get => this.setENOAutomatically.AsBool; set => this.setENOAutomatically.AsBool = value; }
        public SimaticProgrammingLanguage ProgrammingLanguage { get => this.programmingLanguage.AsEnum<SimaticProgrammingLanguage>(); set => this.programmingLanguage.AsEnum(value); }

        public string UDABlockProperties
        {
            get => this.udaBlockProperties.AsString;
            set
            {
                this.UDAEnableTagReadback = true;
                this.udaBlockProperties.AsString = value;
            }
        }
        public bool UDAEnableTagReadback { get => this.udaEnableTagReadback.AsBool; private set => this.udaEnableTagReadback.AsBool = value; }

        public string InstanceOfName { get => this.instanceOfName.AsString; set => this.instanceOfName.AsString = value; }
        public string InstanceOfType { get => this.instanceOfType.AsString; set => this.instanceOfType.AsString = value; }



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

        public BlockAttributeList(bool isUDT = false) : base(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true)
        {
            //==== INIT CONFIGURATION ====
            dbAccessibleFromOPCUA = this.AddNode("DBAccessibleFromOPCUA");
            dbAccessibleFromWebserver = this.AddNode("DBAccessibleFromWebserver");
            headerAuthor = this.AddNode("HeaderAuthor");
            headerFamily = this.AddNode("HeaderFamily");
            headerName = this.AddNode("HeaderName");
            headerVersion = this.AddNode("HeaderVersion");
            autoNumber = this.AddNodeWithFunc("AutoNumber", () => !isUDT);
            instanceOfName = this.AddNodeWithFunc("InstanceOfName", () => !isUDT);
            instanceOfType = this.AddNodeWithFunc("InstanceOfType", () => !isUDT);
            blockInterface = this.AddNode("Interface", required: true);
            blockSections = blockInterface.AddNodeList("Sections", this.CreateSection, required: true, namespaceURI: SimaticMLAPI.GET_SECTIONS_NAMESPACE());

            secondaryType = this.AddNodeWithFunc("SecondaryType", () => !isUDT);
            assignedProDiagFB = this.AddNodeWithFunc("AssignedProDiagFB", () => !isUDT);
            supervisions = this.AddNodeWithFunc("Supervisions", () => !isUDT);
            isOnlyStoredInLoadMemory = this.AddNodeWithFunc("IsOnlyStoredInLoadMemory", () => !isUDT);
            isWriteProtectedInAS = this.AddNodeWithFunc("IsWriteProtectedInAS", () => !isUDT);
            isRetainMemResEnabled = this.AddNodeWithFunc("IsRetainMemResEnabled", () => !isUDT);
            isIECCheckEnabled = this.AddNodeWithFunc("IsIECCheckEnabled", () => !isUDT);
            memoryLayout = this.AddNodeWithFunc("MemoryLayout", () => !isUDT);
            memoryReserve = this.AddNodeWithFunc("MemoryReserve", () => !isUDT && !IsSafe());
            retainMemoryReserve = this.AddNodeWithFunc("RetainMemoryReserve", () => !isUDT && !IsSafe());
            parameterPassing = this.AddNodeWithFunc("ParameterPassing", () => !isUDT);

            blockName = this.AddNode("Name", required: true, defaultInnerText: "fcTest");
            if (SimaticMLAPI.TIA_VERSION >= 18)
            {
                this.AddNode("Namespace", required: true);
            }

            blockNumber = this.AddNodeWithFunc("Number", () => !isUDT, required: true, defaultInnerText: "1");
            programmingLanguage = this.AddNodeWithFunc("ProgrammingLanguage", () => !isUDT, required: true, defaultInnerText: "LAD");
            setENOAutomatically = this.AddNodeWithFunc("SetENOAutomatically", () => !isUDT && !IsSafe());
            libraryConformanceStatus = this.AddNode("LibraryConformanceStatus");
            udaBlockProperties = this.AddNode("UDABlockProperties");
            udaEnableTagReadback = this.AddNode("UDAEnableTagReadback");
            //==== INIT CONFIGURATION ====
        }

        private Section? CreateSection(XmlNode node)
        {
            return node.Name == Section.NODE_NAME ? new Section() : null;
        }

        private Section ComputeSection(SectionTypeEnum sectionType)
        {
            foreach (var item in blockSections.GetItems())
            {
                if (item.SectionType == sectionType)
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

        private bool IsSafe()
        {
            return this.ProgrammingLanguage == SimaticProgrammingLanguage.SAFE_LADDER
            || this.ProgrammingLanguage == SimaticProgrammingLanguage.SAFE_FBD
            || this.ProgrammingLanguage == SimaticProgrammingLanguage.SAFE_DB;
        }
    }
}
