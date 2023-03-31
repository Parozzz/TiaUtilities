 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SpinXmlReader.Block
{
    public abstract class Wire : UIdObject
    {
    }

    public class PowerrailWire : Wire
    {
        private readonly List<WirePart> wireParts;

        public PowerrailWire()
        {
            wireParts = new List<WirePart>();
        }

        public void AddWirePart(WirePart part)
        {
            wireParts.Add(part);
        }

        public ICollection<WirePart> GetWireParts()
        {
            return wireParts;
        }
    }

    public class NormalWire : Wire
    {

        private readonly WirePart leftWirePart;
        private readonly WirePart rightWirePart;

        public NormalWire(WirePart leftWirePart, WirePart rightWirePart)
        {
            this.leftWirePart = leftWirePart;
            this.rightWirePart = rightWirePart;
        }

        public WirePart GetLeftWirePart()
        {
            return leftWirePart;
        }

        public WirePart GetRightWirePart()
        {
            return rightWirePart;
        }
    }

    public enum WirePartType
    {
        POWERRAIL,
        NAMECON,
        IDENTCON,
        OPENCON
    }

    public class WirePart
    {
        public string Name { get; protected internal set; }

        private readonly WirePartType type;
        private readonly UIdObject partUIdObject;
        private readonly string name;

        public WirePart(WirePartType type, UIdObject partUIdObject, string name)
        {
            this.type = type;
            this.partUIdObject = partUIdObject;
            this.name = name;
        }

        public WirePartType GetWirePart()
        {
            return type;
        }

        public UIdObject GetPartUIdObject()
        {
            return partUIdObject;
        }

        public bool HasName()
        {
            return Name != null && Name.Length > 0;
        }

        public string GetName()
        {
            return name;
        }
    }
}
