using System.Xml;
using SimaticML.Enums;
using SimaticML.Enums.Utility;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Blocks.FlagNet.nCall;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks
{
    public class CompileUnit : XmlNodeConfiguration, ILocalObjectMaster, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.CompileUnit";
        private static XmlNodeConfiguration? CreatePart(XmlNode node)
        {
            return node.Name switch
            {
                Access.NODE_NAME => new Access(),
                Part.NODE_NAME => new Part(),
                Call.NODE_NAME => new Call(),
                _ => null,
            };
        }

        public SimaticProgrammingLanguage ProgrammingLanguage { get => this.programmingLanguage.AsEnum<SimaticProgrammingLanguage>(); set => this.programmingLanguage.AsEnum(value); }
        public MultilingualText Title { get => this.ComputeMultilingualText(MultilingualTextType.TITLE); }
        public MultilingualText Comment { get => this.ComputeMultilingualText(MultilingualTextType.COMMENT); }
        public AccessFactory AccessFactory { get; init; }
        public PowerrailWire Powerrail { get => new(this.ComputePowerrail()); }

        //Start from a high number in case there are nodes not included! Since siemens starts from 20, i SHOULD avoid most conflicts.
        //Is not nice but works ¯\_(ツ)_/¯
        public IDGenerator LocalIDGenerator { get; private set; } = new IDGenerator(10000);

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeListConfiguration<MultilingualText> objectList;

        private readonly XmlNodeConfiguration networkSource;
        private readonly XmlNodeConfiguration flgNet;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> parts;
        private readonly XmlNodeListConfiguration<Wire> wires;
        private readonly XmlNodeListConfiguration<LabelDeclaration> labels;

        private readonly XmlNodeConfiguration programmingLanguage;

        public CompileUnit() : base(CompileUnit.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute("CompositionName", required: true, requiredValue: "CompileUnits");

            var attributeList = AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            networkSource = attributeList.AddNode("NetworkSource", required: true);
            flgNet = networkSource.AddNode("FlgNet", namespaceURI: SimaticMLAPI.GET_FLAG_NET_NAMESPACE());
            labels = flgNet.AddNodeList("Labels", LabelDeclaration.CreateLabelDeclaration); //FIRST! It will not work otherwise.
            parts = flgNet.AddNodeList("Parts", CompileUnit.CreatePart);
            wires = flgNet.AddNodeList("Wires", Wire.CreateWire);

            objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, MultilingualText.CreateMultilingualText, required: true);

            programmingLanguage = attributeList.AddNode("ProgrammingLanguage", required: true, defaultInnerText: SimaticProgrammingLanguage.LADDER.GetSimaticMLString());
            //==== INIT CONFIGURATION ====

            this.AccessFactory = new(this);
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            base.Load(xmlNode, parseUnknown);

            foreach (Wire wire in this.wires.GetItems())
            {
                var identCon = wire.GetIdentCon();
                if (identCon != null)
                {
                    var uid = identCon.GetLocalObjectUId();
                    identCon.SetLocalObject(FindPartsLocalObjectByUId(uid));
                }

                foreach (var nameCon in wire.GetNameCons())
                {
                    var uid = nameCon.GetLocalObjectUId();
                    nameCon.SetLocalObject(FindPartsLocalObjectByUId(uid));
                }
            }
        }

        public ILocalObject? FindPartsLocalObjectByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is ILocalObject).Cast<ILocalObject>().Where(o => o.GetUId() == uid).FirstOrDefault();
        }

        public Part? FindPartByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is Part).Cast<Part>().Where(p => p.GetUId() == uid).FirstOrDefault();
        }

        public Access? FindAccessByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is Access).Cast<Access>().Where(a => a.GetUId() == uid).FirstOrDefault();
        }


        public void UpdateLocalObjects()
        {
            this.LocalIDGenerator.Reset();

            var objectList = new List<object>();
            objectList.AddRange(base.childrenDict.Values);
            objectList.AddRange(this.labels.GetItems());
            objectList.AddRange(this.parts.GetItems());
            objectList.AddRange(this.wires.GetItems()); //UPDATE WIRES AFTER PARTS! OTHERWISE PART LOCAL UID ARE NOT UPDATED AND IT WON'T WORK!
            foreach (XmlNodeConfiguration nodeConfig in objectList)
            {
                if (nodeConfig == null)
                {
                    continue;
                }

                if (nodeConfig is ILocalObject localObject)
                {
                    localObject.UpdateLocalUId(this.LocalIDGenerator);
                }
            }
        }

        public IDGenerator GetLocalIDGenerator()
        {
            return this.LocalIDGenerator;
        }

        public void Init()
        {
            this.Title[SimaticMLAPI.CULTURE] = "";
            this.Comment[SimaticMLAPI.CULTURE] = "";
        }

        private MultilingualText ComputeMultilingualText(MultilingualTextType type)
        {
            foreach (var item in objectList.GetItems())
            {
                if (item.TextType == type)
                {
                    return item;
                }
            }

            var comment = new MultilingualText(type);
            objectList.GetItems().Add(comment);
            return comment;
        }


        public Access CreateAccess()
        {
            var access = new Access();
            parts.GetItems().Add(access);
            return access;
        }

        public Part CreatePart()
        {
            var part = new Part();
            parts.GetItems().Add(part);
            return part;
        }

        public Wire CreateWire()
        {
            var wire = new Wire();
            wires.GetItems().Add(wire);
            return wire;
        }

        public Wire ComputePowerrail()
        {
            return this.wires.GetItems().SingleOrDefault(wire => wire.IsPowerrail()) ?? this.CreateWire().SetPowerrail();
        }
        /*
        public CompileUnit AddPowerrailConnections(Dictionary<Part, string> partConnectionDict)
        {
            var powerrailWire = this.wires.GetItems().SingleOrDefault(wire => wire.IsPowerrail());
            if (powerrailWire == null)
            {
                powerrailWire = this.CreateWire().SetPowerrail();
            }

            foreach (var entry in partConnectionDict)
            {
                powerrailWire.CreateNameCon(entry.Key, entry.Value);
            }

            return this;
        }

        public CompileUnit AddPowerrailConnections(Part part, string partConnection)
        {
            var dict = new Dictionary<Part, string>() { { part, partConnection } };
            return AddPowerrailConnections(dict);
        }*/
    }
}
