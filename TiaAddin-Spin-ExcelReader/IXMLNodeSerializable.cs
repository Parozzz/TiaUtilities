using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinXmlReader
{
    public interface IXMLNodeSerializable
    {
        void ParseNode(XmlNode tagNode);
        XmlNode GenerateNode(XmlDocument document);
    }
}
