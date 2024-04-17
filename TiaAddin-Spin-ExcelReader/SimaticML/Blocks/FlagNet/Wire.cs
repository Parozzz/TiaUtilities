using System.Linq;
using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.Blocks.FlagNet;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nPart;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet
{
    public enum WirePartType
    {
        POWERRAIL,
        NAMECON,
        IDENTCON,
        OPENCON
    }

    public class Wire : XmlNodeListConfiguration<Con>, ILocalObject
    {
        public const string NODE_NAME = "Wire";
        public static Wire CreateWire(CompileUnit compileUnit, XmlNode node)
        {
            return node.Name == Wire.NODE_NAME ? new Wire(compileUnit) : null;
        }

        private readonly CompileUnit compileUnit;
        private readonly LocalObjectData localObjectData;

        public Wire(CompileUnit compileUnit) : base(Wire.NODE_NAME, xmlNode => Con.CreateCon(xmlNode, compileUnit))
        {
            this.compileUnit = compileUnit;
            compileUnit.AddWire(this);

            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData(compileUnit.LocalIDGenerator));
            //==== INIT CONFIGURATION ====
        }

        private Con AddCon(string name)
        {
            return this.AddNode(new Con(name));
        }

        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }

        public PowerrailCon GetPowerrail()
        {
            return this.GetItems().SingleOrDefault(con => con.GetConfigurationName() == PowerrailCon.NODE_NAME) is PowerrailCon powerrailCon ? powerrailCon : null;
        }

        public bool IsPowerrail()
        {
            return GetPowerrail() != null;
        }

        public void SetPowerrail()
        {
            if (!IsPowerrail() && !IsIdentCon())
            {
                this.GetItems().Add(new PowerrailCon());
            }
        }

        public void AddPowerrailCon(Part part, string partConnectionName)
        {
            if (IsPowerrail())
            {
                var nameCon = new NameCon();
                nameCon.SetConUId(part.GetLocalObjectData().GetUId());
                nameCon.SetConName(partConnectionName);
                this.GetItems().Add(nameCon);
            }
        }

        public IdentCon GetIdentCon()
        {
            return this.GetItems().SingleOrDefault(con => con.GetConfigurationName() == IdentCon.NODE_NAME) is IdentCon identCon ? identCon : null;
        }

        public bool IsIdentCon()
        {
            return GetIdentCon() != null;
        }

        public Wire AddIdentCon(Access access, uint partUId, string partConnectionName)
        {
            if (!IsPowerrail() && !IsIdentCon())
            {
                var identCon = new IdentCon();
                identCon.SetConUId(access.GetUId());
                this.GetItems().Add(identCon);

                var nameCon = new NameCon();
                nameCon.SetConUId(partUId);
                nameCon.SetConName(partConnectionName);
                this.GetItems().Add(nameCon);
            }

            return this;
        }

        public uint GetIdentAccessUId()
        {
            var identCon = this.GetIdentCon();
            return identCon != null ? identCon.GetConUId() : 0;
        }

        public Wire AddNameCon(Part part, string partConnectionName)
        {
            var nameCon = new NameCon();
            nameCon.SetConUId(part.GetLocalObjectData().GetUId());
            nameCon.SetConName(partConnectionName);
            this.GetItems().Add(nameCon);

            return this;
        }

        public bool HasOpenCon()
        {
            return this.GetItems().Select(con => con.GetConfigurationName() == OpenCon.NODE_NAME).Any();
        }

        public Wire AddOpenCon()
        {
            var openCon = new OpenCon();
            openCon.SetConUId(compileUnit.LocalIDGenerator.GetNext());
            this.GetItems().Add(openCon);

            return this;
        }
    }

    public class Con : XmlNodeConfiguration 
    {
        public static Con CreateCon(XmlNode node, CompileUnit compileUnit)
        {
            switch (node.Name)
            {
                case PowerrailCon.NODE_NAME:
                    return new PowerrailCon();
                case IdentCon.NODE_NAME:
                    return new IdentCon();
                case OpenCon.NODE_NAME:
                    return new OpenCon();
                case NameCon.NODE_NAME:
                    return new NameCon();
                case Openbranch.NODE_NAME:
                    return new Openbranch();
                default:
                    throw new System.Exception("Cannot find Wire Connection with name " + node.Name);
            }
        }

        private readonly XmlAttributeConfiguration uid; //This identify a Part or an Access.

        public Con(string name, bool required = false) : base(name, required: required)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId", required: true);
            //==== INIT CONFIGURATION ====
        }

        public void SetConUId(uint uId)
        {
            uid.SetValue(uId.ToString());
        }

        public uint GetConUId()
        {
            return uid.GetUIntValue(out uint value) ? value : 0;
        }
    }

    public class PowerrailCon : Con
    {
        public const string NODE_NAME = "Powerrail";
        public PowerrailCon() : base(NODE_NAME, required: true) { } //Adding required ensure it is added even if empty!
    }

    public class IdentCon : Con
    {
        public const string NODE_NAME = "IdentCon";
        public IdentCon() : base(NODE_NAME) { }
    }

    public class OpenCon : Con
    {
        public const string NODE_NAME = "OpenCon";
        public OpenCon() : base(NODE_NAME)  { }

    }

    public class Openbranch : Con
    {
        public const string NODE_NAME = "Openbranch";
        public Openbranch() : base(NODE_NAME) { }

    }

    public class NameCon : Con
    {
        public const string NODE_NAME = "NameCon";

        private readonly XmlAttributeConfiguration connectionName;

        public NameCon() : base(NameCon.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            connectionName = this.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }

        public void SetConName(string name)
        {
            this.connectionName.SetValue(name);
        }

        public string GetConName()
        {
            return connectionName.GetValue();
        }
    }
}
