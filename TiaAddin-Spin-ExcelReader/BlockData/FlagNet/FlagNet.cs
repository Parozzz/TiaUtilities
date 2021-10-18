using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class FlagNet
    {
        private readonly FCData fcData;
        private readonly Dictionary<uint, Part> partUIdDictionary;
        private readonly Dictionary<uint, Access> accessUIdDictionary;
        internal FlagNet(FCData fcData)
        {
            this.fcData = fcData;
            partUIdDictionary = new Dictionary<uint, Part>();
            accessUIdDictionary = new Dictionary<uint, Access>();
        }


        public FlagNet ParseXMLNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("FlgNet"), "FlagNet XmlNode has wrong name.");

            var netNamespace = XmlUtil.GetNamespace(fcData.GetXmlDocument(), "net", node.NamespaceURI); //The section has different namespace.nsmgr
            foreach (XmlNode partNode in node.SelectNodes("net:Parts/net:Part", netNamespace))
            {
                var part = PartParser.ParseXMLNode(partNode);
                partUIdDictionary.Add(part.UId, part);
            }

            foreach (XmlNode accessNode in node.SelectNodes("net:Parts/net:Access", netNamespace))
            {
                var access = AccessParser.ParseXMLNode(accessNode, netNamespace);
                accessUIdDictionary.Add(access.UId, access);
            }

            foreach(XmlNode wireNode in node.SelectNodes("net:Wires/net:Wire", netNamespace))
            {

            }

            return this;
        }
    }
}
