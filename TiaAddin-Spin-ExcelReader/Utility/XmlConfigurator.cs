using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TiaXmlReader.Utility
{
    public class XmlConfigurator : XmlNodeConfiguration
    {
        public XmlConfigurator(string nodeName, string namespaceURI = "") : base(nodeName, true, namespaceURI, "")
        {
        }

    }

    public class XmlNodeConfiguration
    {
        private readonly string name;
        private readonly bool required;
        private readonly string namespaceURI;

        private string innerText;
        private XmlElement xmlElement;

        private readonly List<XmlAttributeConfiguration> attributeConfigurations;
        private readonly List<XmlNodeConfiguration> childrenConfigurations;

        
        public XmlNodeConfiguration(string name, bool required, string namespaceURI, string defaultInnerText)
        {
            this.name = name;
            this.required = required;
            this.namespaceURI = namespaceURI;
            innerText = defaultInnerText;
            attributeConfigurations = new List<XmlAttributeConfiguration>();
            childrenConfigurations = new List<XmlNodeConfiguration>();
        }

        public string GetName()
        {
            return this.name;
        }

        public bool IsRequired()
        {
            return this.required;
        }

        public string GetNamespaceURI()
        {
            return namespaceURI;
        }

        public XmlAttributeConfiguration AddAttribute(XmlAttributeConfiguration attributeConfiguration)
        {
            attributeConfigurations.Add(attributeConfiguration);
            return attributeConfiguration;
        }

        public XmlAttributeConfiguration AddAttribute(string name, bool required = false, string defaultValue = "")
        {
            return AddAttribute(new XmlAttributeConfiguration(name, required, defaultValue));
        }

        public XmlNodeConfiguration AddNode(XmlNodeConfiguration nodeConfiguration)
        {
            childrenConfigurations.Add(nodeConfiguration);
            return nodeConfiguration;
        }

        public XmlNodeConfiguration AddNode(string name, bool required = true, string namespaceURI = "", string defaultInnerText = "")
        {
            return AddNode(new XmlNodeConfiguration(name, required, namespaceURI, defaultInnerText));
        }

        public void SetInnerText(string innerText)
        {
            this.innerText = innerText;
        }

        public string GetInnerText()
        {
            return innerText;
        }

        public bool Parse(XmlNode xmlNode)
        {
            Validate.NotNull(xmlNode);
            Validate.IsTrue(xmlNode.Name.Equals(name), "Node name is not valid for " + name);

            if (namespaceURI != null && namespaceURI.Length > 0)
            {
                xmlElement = xmlNode[name, namespaceURI];
            }
            else
            {
                xmlElement = xmlNode[name];
            }

            var childrenOK = true;
            if (xmlElement != null)
            {
                foreach (var attributeConfig in attributeConfigurations)
                {
                    childrenOK &= attributeConfig.Parse(xmlElement);
                }

                foreach (var childConfig in childrenConfigurations)
                {
                    childrenOK &= childConfig.Parse(xmlElement);
                }
            }


            return childrenOK && (xmlElement != null || !required);
        }

        public XmlNode Generate(XmlDocument document)
        {
            var xmlElement = string.IsNullOrEmpty(namespaceURI) ? document.CreateElement(name) : document.CreateElement(name, namespaceURI);
            foreach(var config in attributeConfigurations)
            {
                config.Set(document, xmlElement);
            }

            foreach(var config in childrenConfigurations)
            {
                var childXmlElement = config.Generate(document);
                xmlElement.AppendChild(childXmlElement);
            }

            return xmlElement;
        }
    }

    public class XmlAttributeConfiguration
    {
        private readonly string name;
        private readonly bool required;
        private string value;

        public XmlAttributeConfiguration(string name, bool required, string defaultValue)
        {
            this.name = name;
            this.required = required;
            this.value = defaultValue;
        }
        public string GetName()
        {
            return this.name;
        }

        public bool IsRequired()
        {
            return this.required;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        public string GetValue()
        {
            return this.value;
        }

        public bool Parse(XmlNode xmlNode)
        {
            var attribute = xmlNode.Attributes[name];
            if (attribute != null)
            {
                this.value = attribute.Value;
            }

            return attribute != null || !required;
        }

        public void Set(XmlDocument document, XmlNode xmlNode)
        {
            xmlNode.Attributes.Append(document.CreateAttribute(name)).Value = this.value;
        }
    }

}
