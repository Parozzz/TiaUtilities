using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class FlagNet
    {
        private readonly FCData fcData;
        private readonly Dictionary<uint, UIdObject> completeUIdDictionary;
        private readonly Dictionary<uint, Part> partUIdDictionary;
        private readonly Dictionary<uint, Access> accessUIdDictionary;
        private readonly Dictionary<uint, Wire> wireUIdDictionary;

        internal FlagNet(FCData fcData)
        {
            this.fcData = fcData;
            completeUIdDictionary = new Dictionary<uint, UIdObject>();
            partUIdDictionary = new Dictionary<uint, Part>();
            accessUIdDictionary = new Dictionary<uint, Access>();
            wireUIdDictionary = new Dictionary<uint, Wire>();
        }

        public UIdObject GetUIdObject(uint uid)
        {
            return completeUIdDictionary.TryGetValue(uid, out UIdObject value) ? value : null;
        }


        public FlagNet ParseXMLNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("FlgNet"), "FlagNet XmlNode has wrong name.");

            var parser = new FlagNetParser(this, netNamespace);
            foreach (XmlNode partNode in XmlSearchEngine.Of(node).AddSearch("Parts/Part").GetAllNodes())
            {
                var part = parser.ParsePartNode(partNode);
                if (part != null)
                {
                    completeUIdDictionary.Add(part.UId, part);
                    partUIdDictionary.Add(part.UId, part);
                }
            }
            
            foreach (XmlNode accessNode in XmlSearchEngine.Of(node).AddSearch("Parts/Access").GetAllNodes())
            {
                var access = parser.ParseAccessNode(accessNode);
                if (access != null)
                {
                    completeUIdDictionary.Add(access.UId, access);
                    accessUIdDictionary.Add(access.UId, access);
                }
            }
            
            //Call is used when calling FCs or FBs
            foreach (XmlNode callNode in XmlSearchEngine.Of(node).AddSearch("Parts/Call").GetAllNodes())
            {
            }

            
            foreach (XmlNode wireNode in XmlSearchEngine.Of(node).AddSearch("Wires/Wire").GetAllNodes())
            {
                var wire = parser.ParseWireNode(this, wireNode);
                if(wire != null)
                {
                    completeUIdDictionary[wire.UId] = wire;
                    wireUIdDictionary[wire.UId] = wire;
                }
            }

            return this;
        }
    }
}
