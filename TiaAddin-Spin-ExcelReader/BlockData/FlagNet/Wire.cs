using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class Wire : UIdObject
    {
        public bool HasPowerrail { get; internal set; }

        private List<WirePart> wirePartList;

        public Wire()
        {
            wirePartList = new List<WirePart>();
        }

        public void AddWirePart(WirePart wirePart)
        {
            wirePartList.Add(wirePart);
        }
    }

    public enum WirePartType
    {
        POWERRAIL,
        NAMECON,
        IDENTCON,
        OPENCON
    }

    public class WirePart : UIdObject
    {
        public string Name { get; protected internal set; }

        public WirePartType Type { get; protected internal set; }

        public WirePart()
        {

        }

        public bool HasName()
        {
            return Name != null && Name.Length > 0;
        }

        public void ParseXMLNode(XmlNode node)
        {
            if(uint.TryParse(node.Attributes["UId"].Value, out uint parsedUId))
            {
                this.UId = parsedUId;
            }

            var nameAttribute = node.Attributes["Name"];
            if (nameAttribute != null)
            {
                Name = nameAttribute.Value;
            }
        }
    }
}
