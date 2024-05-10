using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nPart;
using TiaXmlReader.XMLClasses;

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
        public static Wire CreateWire(XmlNode node)
        {
            return node.Name == Wire.NODE_NAME ? new Wire() : null;
        }

        private readonly XmlAttributeConfiguration uid;

        public Wire() : base(Wire.NODE_NAME, xmlNode => Con.CreateCon(xmlNode))
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");
            //==== INIT CONFIGURATION ====
        }

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());

            foreach (var con in this.GetItems())
            {
                if (con is OpenCon openCon)
                {
                    openCon.UpdateLocalUId(localIDGeneration);
                }
                else if (con is NameCon nameCon)
                {
                    nameCon.UpdateIDFromLocalObject();
                }
                else if (con is IdentCon identCon)
                {
                    identCon.UpdateIDFromLocalObject();
                }
            }
        }

        public void SetUId(uint uid)
        {
            this.uid.SetValue("" + uid);
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }

        public PowerrailCon GetPowerrail()
        {
            return this.GetItems().SingleOrDefault(c => c is PowerrailCon) is PowerrailCon powerrailCon ? powerrailCon : null;
        }

        public bool IsPowerrail()
        {
            return GetPowerrail() != null;
        }

        public Wire SetPowerrail()
        {
            if (!IsPowerrail() && !IsIdentCon())
            {
                this.GetItems().Add(new PowerrailCon());
            }

            return this;
        }

        public IdentCon GetIdentCon()
        {
            return this.GetItems().SingleOrDefault(c => c is IdentCon) is IdentCon identCon ? identCon : null;
        }

        public bool IsIdentCon()
        {
            return GetIdentCon() != null;
        }

        public Wire CreateIdentCon(Access access, Part part, string partConnectionName)
        {
            if (!IsPowerrail() && !IsIdentCon())
            {
                var identCon = new IdentCon();
                identCon.SetLocalObject(access);
                this.GetItems().Add(identCon);

                this.CreateNameCon(part, partConnectionName);
            }

            return this;
        }

        public List<NameCon> GetNameCons()
        {
            return this.GetItems().Where(c => c is NameCon).Cast<NameCon>().ToList();
        }

        public Wire CreateNameCon(Part part, string partConnectionName)
        {
            var nameCon = new NameCon();
            nameCon.SetLocalObject(part);
            nameCon.SetConName(partConnectionName);
            this.GetItems().Add(nameCon);

            return this;
        }

        public OpenCon GetOpenCon()
        {
            var con = this.GetItems().Where(c => c is OpenCon).FirstOrDefault();
            return con is OpenCon openCon ? openCon : null;
        }

        public bool HasOpenCon()
        {
            return this.GetItems().Select(c => c is OpenCon).Any();
        }

        public Wire CreateOpenCon()
        {
            this.GetItems().Add(new OpenCon());
            return this;
        }
    }

    public class Con : XmlNodeConfiguration
    {
        public static Con CreateCon(XmlNode node)
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

        protected readonly XmlAttributeConfiguration uid; //This identify a Part or an Access.

        public Con(string name, bool required = false) : base(name, required: required)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");
            //==== INIT CONFIGURATION ====
        }
    }

    public class PowerrailCon : Con
    {
        public const string NODE_NAME = "Powerrail";
        public PowerrailCon() : base(NODE_NAME, required: true) { } //Adding required ensure it is added even if empty!
    }

    public class IdentCon : Con //Connection to identify an Access
    {
        public const string NODE_NAME = "IdentCon";

        private ILocalObject localObject;
        public IdentCon() : base(NODE_NAME) { }

        public void UpdateIDFromLocalObject()
        {
            if (this.localObject != null)
            {
                this.uid.SetValue("" + localObject.GetUId());
            }
        }

        public void SetLocalObject(ILocalObject localObject)
        {
            this.localObject = localObject;
        }

        public uint GetLocalObjectUId()
        {
            if (localObject != null)
            {
                return localObject.GetUId();
            }

            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }
    }

    public class OpenCon : Con, ILocalObject
    {
        public const string NODE_NAME = "OpenCon";
        public OpenCon() : base(NODE_NAME) { }

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());
        }

        public void SetUId(uint uid)
        {
            this.uid.SetValue("" + uid);
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }

    }

    public class Openbranch : Con
    {
        public const string NODE_NAME = "Openbranch";
        public Openbranch() : base(NODE_NAME) { }

    }

    public class NameCon : Con //Connection to connect a Part with an Access
    {
        public const string NODE_NAME = "NameCon";

        private readonly XmlAttributeConfiguration connectionName;

        private ILocalObject localObject;

        public NameCon() : base(NameCon.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            connectionName = this.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }

        public void UpdateIDFromLocalObject()
        {
            if (this.localObject != null)
            {
                this.uid.SetValue("" + localObject.GetUId());
            }
        }

        public void SetLocalObject(ILocalObject localObject)
        {
            this.localObject = localObject;
        }

        public uint GetLocalObjectUId()
        {
            if(localObject != null)
            {
                return localObject.GetUId();
            }
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
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
