using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SpinAddIn.BlockData
{
    public class FlagNet
    {
        private readonly Dictionary<uint, Part> partUIdDictionary;
        private readonly Dictionary<uint, Access> accessUIdDictionary;
        internal FlagNet()
        {
            partUIdDictionary = new Dictionary<uint, Part>();
            accessUIdDictionary = new Dictionary<uint, Access>();
        }


        public FlagNet ParseXMLNode(XmlNode xmlNode)
        {
            Validate.NotNull(xmlNode);
            Validate.IsTrue(xmlNode.Name.Equals("FlgNet"));

            var partsNode = xmlNode.SelectSingleNode("//Parts");
            foreach(XmlNode partNode in partsNode.SelectNodes("./Part"))
            {
                var part = PartParser.ParseXMLNode(partNode);
                partUIdDictionary.Add(part.UId, part);
            }

            foreach (XmlNode accessNode in partsNode.SelectNodes("./Access"))
            {
                var access = AccessParser.ParseXMLNode(accessNode);
                accessUIdDictionary.Add(access.UId, access);
            }

            return this;
        }
    }
}
