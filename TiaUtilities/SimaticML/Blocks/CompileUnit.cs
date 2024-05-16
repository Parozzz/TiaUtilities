using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.Blocks.FlagNet;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nPart;
using TiaXmlReader.XMLClasses;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nCall;
using TiaXmlReader.Languages;

namespace TiaXmlReader.SimaticML.Blocks
{
    public class CompileUnit : XmlNodeConfiguration, ILocalObjectMaster, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.CompileUnit";
        private static XmlNodeConfiguration CreatePart(CompileUnit compileUnit, XmlNode node)
        {
            switch (node.Name)
            {
                case Access.NODE_NAME:
                    return new Access();
                case Part.NODE_NAME:
                    return new Part();
                case Call.NODE_NAME:
                    return new Call();
            }

            return null;
        }

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

            var attributeList = AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            networkSource = attributeList.AddNode("NetworkSource", required: true);
            flgNet = networkSource.AddNode("FlgNet", namespaceURI: Constants.GET_FLAG_NET_NAMESPACE());
            labels = flgNet.AddNodeList("Labels", xmlNode => LabelDeclaration.CreateLabelDeclaration(xmlNode)); //FIRST! It will not work otherwise.
            parts = flgNet.AddNodeList("Parts", xmlNode => CompileUnit.CreatePart(this, xmlNode));
            wires = flgNet.AddNodeList("Wires", xmlNode => Wire.CreateWire(xmlNode));
            

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, MultilingualText.CreateMultilingualText, required: true);

            programmingLanguage = attributeList.AddNode("ProgrammingLanguage", required: true, defaultInnerText: SimaticProgrammingLanguage.LADDER.GetSimaticMLString());
            //==== INIT CONFIGURATION ====
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

        public ILocalObject FindPartsLocalObjectByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is ILocalObject).Cast<ILocalObject>().Where(o => o.GetUId() == uid).FirstOrDefault();
        }

        public Part FindPartByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is Part).Cast<Part>().Where(p => p.GetUId() == uid).FirstOrDefault();
        }

        public Access FindAccessByUId(uint uid)
        {
            return this.parts.GetItems().Where(o => o is Access).Cast<Access>().Where(a => a.GetUId() == uid).FirstOrDefault();
        }


        public void UpdateLocalObjects()
        {
            this.LocalIDGenerator.Reset();

            var objectList = new List<object>();
            objectList.AddRange(base.childrenConfigurationDict.Values);
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
            this.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, "");
            this.ComputeBlockComment().SetText(LocalizationVariables.CULTURE, "");
        }

        public MultilingualText ComputeBlockTitle()
        {
            foreach (var item in objectList.GetItems())
            {
                if (item.GetMultilingualTextType() == MultilingualTextType.TITLE)
                {
                    return item;
                }
            }

            var title = new MultilingualText(MultilingualTextType.TITLE);
            objectList.GetItems().Add(title);
            return title;
        }

        public MultilingualText ComputeBlockComment()
        {
            foreach (var item in objectList.GetItems())
            {
                if (item.GetMultilingualTextType() == MultilingualTextType.COMMENT)
                {
                    return item;
                }
            }

            var comment = new MultilingualText(MultilingualTextType.COMMENT);
            objectList.GetItems().Add(comment);
            return comment;
        }

        public SimaticProgrammingLanguage GetBlockProgrammingLanguage()
        {
            return SimaticProgrammingLanguageUtil.GetFromSimaticMLString(programmingLanguage.GetInnerText());
        }

        public CompileUnit SetBlockProgrammingLanguage(SimaticProgrammingLanguage programmingLanguage)
        {
            this.programmingLanguage.SetInnerText(programmingLanguage.GetSimaticMLString());
            return this;
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

        public CompileUnit AddPowerrailConnections(Dictionary<Part, string> partConnectionDict)
        {
            var powerrail = this.wires.GetItems().SingleOrDefault(wire => wire.IsPowerrail());
            if (powerrail == null)
            {
                powerrail = this.CreateWire().SetPowerrail();
            }

            foreach (var entry in partConnectionDict)
            {
                powerrail.CreateNameCon(entry.Key, entry.Value);
            }

            return this;
        }

        public CompileUnit AddPowerrailConnections(Part part, string partConnection)
        {
            var dict = new Dictionary<Part, string>() { { part, partConnection } };
            return AddPowerrailConnections(dict);
        }
    }
}
