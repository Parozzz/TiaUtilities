 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public enum WirePartType
    {
        POWERRAIL,
        NAMECON,
        IDENTCON,
        OPENCON
    }

    public class Wire : XmlNodeListConfiguration<NameCon>, ILocalObject
    {
        public const string NODE_NAME = "Wire";
        public static Wire CreateWire(XmlNode node)
        {
            return node.Name == Wire.NODE_NAME ? new Wire() : null;
        }

        private readonly LocalObjectData localObjectData;

        private readonly XmlNodeConfiguration powerrail;

        private readonly XmlNodeConfiguration identCon; //If this is preset, it means this wire identify a connection between an Access and a Part.
        private readonly XmlAttributeConfiguration identConUId;

        private readonly XmlNodeConfiguration openCon;

        public Wire() : base(Wire.NODE_NAME, NameCon.CreateNameCon)
        {
            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData());

            powerrail = this.AddNode("Powerrail");

            identCon = this.AddCon("IdentCon");
            identConUId = identCon.AddAttribute("UId"); //This UId refers to an Access

            openCon = this.AddCon("OpenCon");
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

        public bool IsPowerrail()
        {
            return powerrail.IsParsed();
        }

        public void SetPowerrail()
        {
            powerrail.SetParsed();
            powerrail.SetRequired();
        }

        public void AddPowerrailCon(Part part, string partConnectionName)
        {
            if(IsPowerrail())
            {
                var nameCon = this.AddNode(new NameCon());
                nameCon.SetConUId(part.GetLocalObjectData().GetUId());
                nameCon.SetConName(partConnectionName);
            }
        }

        public bool IsIdentCon()
        {
            return identCon.IsParsed();
        }

        public void SetIdentCon(uint accessUId, uint partUId, string partConnectionName)
        {
            if(!IsPowerrail() && !IsIdentCon())
            {
                var con = this.AddCon("IdentCon");
                con.SetConUId(accessUId);

                var nameCon = this.AddNode(new NameCon());
                nameCon.SetConUId(partUId);
                nameCon.SetConName(partConnectionName);
            }
        }

        public uint GetIdentAccessUId()
        {
            if(IsIdentCon() && identConUId.GetUIntValue(out uint uid))
            {
                return uid; 
            }

            return 0;
        }

        public uint GetIdentPartUId()
        {
            return IsIdentCon() && this.GetItems().Count == 1 ? this.GetItems()[0].GetConUId() : 0;
        }

        public string GetIdentPartName()
        {
            return IsIdentCon() && this.GetItems().Count == 1 ? this.GetItems()[0].GetConName() : "";
        }


        public uint GetWireStartUId()
        {
            return this.GetItems().Count == 2 ? this.GetItems()[0].GetConUId() : 0;
        }

        public string GetWireStartName()
        {
            return this.GetItems().Count == 2 ? this.GetItems()[0].GetConName() : "";
        }
        public void SetWireStart(Part part, string partConnectionName)
        {
            if (this.GetItems().Count < 2)
            {
                var nameCon = this.AddNode(new NameCon());
                nameCon.SetConUId(part.GetLocalObjectData().GetUId());
                nameCon.SetConName(partConnectionName);
            }
        }

        public bool IsExitOpenCon()
        {
            return openCon.IsParsed();
        }

        public uint GetWireExitUId()
        {
            return this.GetItems().Count == 2 ? this.GetItems()[1].GetConUId() : 0;
        }

        public string GetWireExitName()
        {
            return this.GetItems().Count == 2 ? this.GetItems()[1].GetConName() : "";
        }

        public void SetWireExit(Part part, string partConnectionName)
        {
            if (this.GetItems().Count < 2)
            {
                var nameCon = this.AddNode(new NameCon());
                nameCon.SetConUId(part.GetLocalObjectData().GetUId());
                nameCon.SetConName(partConnectionName);
            }
        }
    }

    public class Con : XmlNodeConfiguration
    {
        private readonly XmlAttributeConfiguration uid; //This identify a Part or an Access.

        public Con(string name) : base(name)
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

    public class NameCon : Con
    {
        public const string NODE_NAME = "NameCon";
        public static NameCon CreateNameCon(XmlNode node)
        {
            return node.Name == NameCon.NODE_NAME ? new NameCon() : null;
        }

        private readonly XmlAttributeConfiguration name;

        public NameCon() : base(NameCon.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            name = this.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }

        public void SetConName(string name)
        {
            this.name.SetValue(name);
        }

        public string GetConName()
        {
            return name.GetValue();
        }
    }
}
