using SpinAddin.Utility;
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


        public FlagNet ParseXMLNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("FlgNet"), "FlagNet XmlNode has wrong name.");

            var netNamespace = XmlUtil.GetNamespace(fcData.GetXmlDocument(), "net", node.NamespaceURI); //The section has different namespace.nsmgr

            var parser = new FlagNetParser(this, netNamespace);
            foreach (XmlNode partNode in node.SelectNodes("net:Parts/net:Part", netNamespace))
            {
                var part = parser.ParsePartNode(partNode);
                if (part != null)
                {
                    completeUIdDictionary.Add(part.UId, part);
                    partUIdDictionary.Add(part.UId, part);
                }
            }

            foreach (XmlNode accessNode in node.SelectNodes("net:Parts/net:Access", netNamespace))
            {
                var access = parser.ParseAccessNode(accessNode);
                if (access != null)
                {
                    completeUIdDictionary.Add(access.UId, access);
                    accessUIdDictionary.Add(access.UId, access);
                }
            }

            //Call is used when calling FCs or FBs
            foreach (XmlNode callNode in node.SelectNodes("net:Parts/net:Call", netNamespace))
            {
            }


            foreach (XmlNode wireNode in node.SelectNodes("net:Wires/net:Wire", netNamespace))
            {
                var wire = parser.ParseWireNode(wireNode);
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
