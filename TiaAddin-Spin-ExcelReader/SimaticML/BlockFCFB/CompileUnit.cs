using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TiaXmlReader;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class CompileUnit : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.CompileUnit";
        private static XmlNodeConfiguration CreatePart(XmlNode node)
        {
            switch (node.Name)
            {
                case Access.NODE_NAME:
                    return new Access();
                case Part.NODE_NAME:
                    return new Part();
            }

            return null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlAttributeConfiguration compositionName;

        private readonly XmlNodeListConfiguration<MultilingualText> objectList;

        private readonly XmlNodeConfiguration networkSource;
        private readonly XmlNodeConfiguration flgNet;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> parts;
        private readonly XmlNodeListConfiguration<Wire> wires;

        private readonly XmlNodeConfiguration programmingLanguange;

        public CompileUnit() : base(CompileUnit.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            compositionName = this.AddAttribute("CompositionName", required: true, requiredValue: "CompileUnits");

            var attributeList = AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            networkSource = attributeList.AddNode("NetworkSource", required: true);
            flgNet = networkSource.AddNode("FlgNet", namespaceURI: Constants.GET_FLAG_NET_NAMESPACE());
            parts = flgNet.AddNodeList("Parts", CompileUnit.CreatePart);
            wires = flgNet.AddNodeList("Wires", Wire.CreateWire);

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, MultilingualText.CreateMultilingualText, required: true);


            programmingLanguange = attributeList.AddNode("ProgrammingLanguage", required: true, defaultInnerText: "LAD");
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            var title = this.ComputeBlockTitle();
            title.SetText(Constants.DEFAULT_CULTURE, "");

            var comment = this.ComputeBlockComment();
            comment.SetText(Constants.DEFAULT_CULTURE, "");
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

        public string GetBlockProgrammingLanguage()
        {
            return programmingLanguange.GetInnerText();
        }

        public Access AddAccess()
        {
            return parts.AddNode(new Access());
        }

        public Part AddPart(Part.Type partType)
        {
            var part = new Part();
            part.SetPartType(partType);
            return parts.AddNode(part);
        }

        public CompileUnit AddPowerrailConnections(Dictionary<Part, string> partConnectionDict)
        {
            Wire powerrail = null;
            foreach(var wire in this.wires.GetItems())
            {
                if(wire.IsPowerrail())
                {
                    powerrail = wire;
                }
            }

            if(powerrail == null)
            {
                powerrail = this.AddWire();
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

        public void AddIdentWire(Access.Type accessType, string accessSymbol, Part part, string partConnectionName)
        {
            var access = this.AddAccess();
            access.SetAccessType(accessType);
            access.SetSymbol(accessSymbol);

            var identWire = this.AddWire();
            identWire.SetIdentCon(access.GetLocalObjectData().GetUId(), part.GetLocalObjectData().GetUId(), partConnectionName);
        }

        public void AddBoolANDWire(Part startPart, string startPartConnectionName, Part exitPart, string exitPartConnectionName)
        {
            var andWire = this.AddWire();
            andWire.SetWireStart(startPart, startPartConnectionName);
            andWire.SetWireExit(exitPart, exitPartConnectionName);
        }

        public Wire AddWire()
        {
            var wire = new Wire();
            wires.GetItems().Add(wire);
            return wire;
        }
    }
}
