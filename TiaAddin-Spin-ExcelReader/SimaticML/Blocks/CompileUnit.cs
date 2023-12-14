using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.Utility;
using System;
using TiaXmlReader.SimaticML;
using SpinXmlReader.SimaticML;

namespace SpinXmlReader.Block
{
    public class CompileUnit : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.CompileUnit";
        private static XmlNodeConfiguration CreatePart(CompileUnit compileUnit, XmlNode node)
        {
            switch (node.Name)
            {
                case Access.NODE_NAME:
                    return new Access(compileUnit);
                case Part.NODE_NAME:
                    return new Part(compileUnit);
            }

            return null;
        }

        public IDGenerator LocalIDGenerator { get; private set; } = new IDGenerator(20);

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeListConfiguration<MultilingualText> objectList;

        private readonly XmlNodeConfiguration networkSource;
        private readonly XmlNodeConfiguration flgNet;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> parts;
        private readonly XmlNodeListConfiguration<Wire> wires;

        private readonly XmlNodeConfiguration programmingLanguage;

        public CompileUnit() : base(CompileUnit.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute("CompositionName", required: true, requiredValue: "CompileUnits");

            var attributeList = AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            networkSource = attributeList.AddNode("NetworkSource", required: true);
            flgNet = networkSource.AddNode("FlgNet", namespaceURI: Constants.GET_FLAG_NET_NAMESPACE());
            parts = flgNet.AddNodeList("Parts", xmlNode => CompileUnit.CreatePart(this, xmlNode));
            wires = flgNet.AddNodeList("Wires", xmlNode => Wire.CreateWire(this, xmlNode));

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, MultilingualText.CreateMultilingualText, required: true);

            programmingLanguage = attributeList.AddNode("ProgrammingLanguage", required: true, defaultInnerText: SimaticProgrammingLanguage.LADDER.GetSimaticMLString());
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            this.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, "");
            this.ComputeBlockComment().SetText(Constants.DEFAULT_CULTURE, "");
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
        public Access AddAccess(Access access)
        {
            if (parts.GetItems().Contains(access))
            {
                throw new Exception("An Access has been added twice to the same CompileUnit.");
            }

            parts.AddNode(access);
            return access;
        }

        public Part AddPart(Part part)
        {
            if (parts.GetItems().Contains(part))
            {
                throw new Exception("A Part has been added twice to the same CompileUnit.");
            }

            parts.GetItems().Add(part);
            return part;
        }

        public Wire AddWire(Wire wire)
        {
            if (wires.GetItems().Contains(wire))
            {
                throw new Exception("A wire has been added twice to the same CompileUnit.");
            }

            wires.GetItems().Add(wire);
            return wire;
        }

        public CompileUnit AddPowerrailConnections(Dictionary<Part, string> partConnectionDict)
        {
            Wire powerrail = null;
            foreach (var wire in this.wires.GetItems())
            {
                if (wire.IsPowerrail())
                {
                    powerrail = wire;
                }
            }

            if (powerrail == null)
            {
                powerrail = new Wire(this);
                powerrail.SetPowerrail();
            }

            foreach (KeyValuePair<Part, string> entry in partConnectionDict)
            {
                powerrail.AddPowerrailCon(entry.Key, entry.Value);
            }

            return this;
        }

        public CompileUnit AddPowerrailSingleConnection(Part part, string partConnection)
        {
            return AddPowerrailConnections(new Dictionary<Part, string>()
            {
                { part, partConnection }
            });
        }
    }
}
