using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Blocks.FlagNet
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
        public static Wire? CreateWire(CompileUnit compileUnit, XmlNode node)
        {
            return node.Name == Wire.NODE_NAME ? new Wire(compileUnit) : null;
        }

        private readonly XmlAttributeConfiguration uid;
        private readonly CompileUnit compileUnit;

        public Wire(CompileUnit compileUnit) : base(Wire.NODE_NAME, Con.CreateCon)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");
            //==== INIT CONFIGURATION ====

            this.compileUnit = compileUnit;
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
            this.uid.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.AsUInt;
        }

        public PowerrailCon? GetPowerrail()
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

        public IdentCon? GetIdentCon()
        {
            return this.GetItems().SingleOrDefault(c => c is IdentCon) is IdentCon identCon ? identCon : null;
        }

        public bool IsIdentCon()
        {
            return GetIdentCon() != null;
        }

        public Wire CreateIdentCon(SimaticVariable variable, Part part, string partConnectionName)
        {
            var access = variable.CreateAccess(this.compileUnit);
            if (access == null)
            {
                return this;
            }

            return this.CreateIdentCon(access, part, partConnectionName);
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
            var nameCon = new NameCon() { ConName = partConnectionName };
            nameCon.SetLocalObject(part);
            this.GetItems().Add(nameCon);
            return this;
        }

        public OpenCon? GetOpenCon()
        {
            return this.GetItems().Where(c => c is OpenCon).Cast<OpenCon>().FirstOrDefault();
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

    public class PowerrailWire(Wire wire)
    {
        private readonly Wire wire = wire;

        public PowerrailWire Add(Part part, string partConnection)
        {
            wire.CreateNameCon(part, partConnection);
            return this;
        }

        public PowerrailWire Add(Dictionary<Part, string> partConnectionDict)
        {
            foreach (var entry in partConnectionDict)
            {
                this.Add(entry.Key, entry.Value);
            }
            return this;
        }

        /*
        public T Add<T>(T partData) where T : IPartData
        {
            this.Add(partData.GetPart(), partData.InputConName);
            return partData;
        }

        public static IPartData operator &(PowerrailWire powerrail, IPartData partData) => powerrail.Add(partData);*/
    }

    public class Con : XmlNodeConfiguration
    {
        public static Con CreateCon(XmlNode node)
        {
            return node.Name switch
            {
                PowerrailCon.NODE_NAME => new PowerrailCon(),
                IdentCon.NODE_NAME => new IdentCon(),
                OpenCon.NODE_NAME => new OpenCon(),
                NameCon.NODE_NAME => new NameCon(),
                Openbranch.NODE_NAME => new Openbranch(),
                _ => throw new Exception("Cannot find Wire Connection with name " + node.Name),
            };
        }

        protected readonly XmlAttributeConfiguration uid; //This identify a Part or an Access.

        public Con(string name, bool required = false) : base(name, required: required)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");
            //==== INIT CONFIGURATION ====
        }
    }

    public class PowerrailCon() : Con(PowerrailCon.NODE_NAME, required: true) //Adding required ensure it is added even if empty!
    {
        public const string NODE_NAME = "Powerrail";
    }

    public class IdentCon() : Con(IdentCon.NODE_NAME) //Connection to identify an Access
    {
        public const string NODE_NAME = "IdentCon";

        private ILocalObject? localObject;

        public void UpdateIDFromLocalObject()
        {
            if (this.localObject != null)
            {
                this.uid.AsUInt = localObject.GetUId();
            }
        }

        public void SetLocalObject(ILocalObject? localObject)
        {
            this.localObject = localObject;
        }

        public uint GetLocalObjectUId()
        {
            return localObject == null ? this.uid.AsUInt : localObject.GetUId();
        }
    }

    public class OpenCon() : Con(OpenCon.NODE_NAME), ILocalObject
    {
        public const string NODE_NAME = "OpenCon";

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());
        }

        public void SetUId(uint uid)
        {
            this.uid.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.AsUInt;
        }

    }

    public class Openbranch() : Con(Openbranch.NODE_NAME)
    {
        public const string NODE_NAME = "Openbranch";
    }

    public class NameCon : Con //Connection to connect a Part with an Access
    {
        public const string NODE_NAME = "NameCon";

        public string ConName { get => this.connectionName.AsString; set => this.connectionName.AsString = value; }

        private readonly XmlAttributeConfiguration connectionName;
        private ILocalObject? localObject;

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
                this.uid.AsUInt = localObject.GetUId();
            }
        }

        public void SetLocalObject(ILocalObject? localObject)
        {
            this.localObject = localObject;
        }

        public uint GetLocalObjectUId()
        {
            return localObject == null ? this.uid.AsUInt : localObject.GetUId();
        }
    }
}
