using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TiaXmlReader.Utility
{
    public class XmlConfigurator : XmlNodeConfiguration
    {
        public XmlConfigurator(string nodeName, string namespaceURI = "") : base(nodeName, true, namespaceURI, "")
        {
        }

    }

    public class XmlNodeListConfiguration<T> : XmlNodeConfiguration where T : XmlNodeConfiguration
    {
        private readonly Func<XmlNode, T> creationFunction;
        private readonly List<T> items;

        public XmlNodeListConfiguration(string name, Func<XmlNode, T> creationFunction, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required, namespaceURI, defaultInnerText)
        {
            this.creationFunction = creationFunction;
            this.items = new List<T>();
        }

        public override bool Parse(XmlNode xmlNode)
        {
            if (base.Parse(xmlNode))
            {
                foreach (XmlNode childNode in base.xmlElement.ChildNodes)
                {
                    var configuration = creationFunction.Invoke(childNode);
                    if (configuration != null)
                    {
                        items.Add(configuration);

                        if (!configuration.Parse(childNode))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && items.Count == 0;
        }

        public override XmlElement Generate(XmlDocument document)
        {
            var xmlElement = base.Generate(document);

            foreach(var item in items)
            {
                var itemXmlElement = item.Generate(document);
                if(itemXmlElement != null)
                {
                    xmlElement.AppendChild(itemXmlElement);
                }
            }

            return xmlElement;
        }

        public List<T> GetItems()
        {
            return items;
        }
    }

    public class XmlNodeConfiguration
    {
        private readonly string name;
        private bool required;
        private readonly string namespaceURI;

        private string innerText;
        private bool parsed = false;
        protected XmlElement xmlElement;

        private readonly bool main;

        protected readonly List<XmlAttributeConfiguration> attributeConfigurations;
        protected readonly List<XmlNodeConfiguration> childrenNodeConfigurations;

        public XmlNodeConfiguration(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "", bool main = false)
        {
            this.name = name;
            this.required = required;
            this.namespaceURI = namespaceURI;
            this.innerText = defaultInnerText;
            this.main = main;

            attributeConfigurations = new List<XmlAttributeConfiguration>();
            childrenNodeConfigurations = new List<XmlNodeConfiguration>();
        }

        public string GetConfigurationName()
        {
            return this.name;
        }

        public bool IsParsed()
        {
            return parsed;
        }
        public void SetParsed()
        {
            parsed = true;
        }

        public bool IsRequired()
        {
            return this.required;
        }

        public void SetRequired()
        {
            required = true;
        }

        public string GetNamespaceURI()
        {
            return namespaceURI;
        }

        public T AddAttribute<T>(T attributeConfiguration) where T : XmlAttributeConfiguration
        {
            attributeConfigurations.Add(attributeConfiguration);
            return attributeConfiguration;
        }

        public XmlAttributeConfiguration AddAttribute(string name, bool required = false, string requiredValue = "", string value = "")
        {
            return AddAttribute(new XmlAttributeConfiguration(name, required, requiredValue, value));
        }

        public T AddNode<T>(T nodeConfiguration) where T : XmlNodeConfiguration
        {
            childrenNodeConfigurations.Add(nodeConfiguration);
            return nodeConfiguration;
        }

        public XmlNodeConfiguration AddNode(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "")
        {
            return AddNode(new XmlNodeConfiguration(name, required, namespaceURI, defaultInnerText));
        }

        public XmlNodeListConfiguration<T> AddNodeList<T>(string name, Func<XmlNode, T> creationFunction, bool required = false, string namespaceURI = "", string defaultInnerText = "") where T : XmlNodeConfiguration
        {
            return AddNode(new XmlNodeListConfiguration<T>(name, creationFunction, required, namespaceURI, defaultInnerText));
        }

        public void SetInnerText(string innerText)
        {
            this.innerText = innerText;
        }

        public string GetInnerText()
        {
            return innerText;
        }

        public virtual bool Parse(XmlNode xmlNode)
        {
            Validate.NotNull(xmlNode);

            this.xmlElement = (XmlElement)xmlNode;

            this.innerText = xmlElement.InnerText;
            foreach (var attributeConfig in attributeConfigurations)
            {
                attributeConfig.Parse(xmlElement);
            }

            foreach (var childNodeConfig in childrenNodeConfigurations)
            {
                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    if (child.Name == childNodeConfig.GetConfigurationName())
                    {
                        childNodeConfig.Parse((XmlElement)child);
                        break;
                    }
                }
            }

            parsed = true;

            foreach (var attributeConfig in attributeConfigurations)
            {
                if (attributeConfig.IsRequired() && !attributeConfig.IsParsed())
                {
                    throw new Exception("A required Attribute has not been parsed. Name=" + attributeConfig.GetName() + " for " + this.GetConfigurationName());
                }
            }

            foreach (var childNodeConfig in childrenNodeConfigurations)
            {
                if (childNodeConfig.IsRequired() && !childNodeConfig.IsParsed())
                {
                    throw new Exception("A required Node has not been parsed. Name=" + childNodeConfig.GetConfigurationName() + " for " + this.GetConfigurationName());
                }
            }

            return true;
        }

        public virtual bool IsEmpty()
        {
            if(string.IsNullOrEmpty(this.innerText) && attributeConfigurations.Count == 0 && childrenNodeConfigurations.Count == 0)
            {
                return true;
            }

            
            var allChildEmpty = true;
            foreach(var childNodeConfig in childrenNodeConfigurations)
            {
                allChildEmpty &= childNodeConfig.IsEmpty();
            }

            var allAttributesEmpty = true;
            foreach (var attributeConfig in attributeConfigurations)
            {
                allAttributesEmpty &= attributeConfig.IsEmpty();
            }

            return allChildEmpty && allAttributesEmpty;

            return false;
        }

        public virtual XmlElement Generate(XmlDocument document)
        {
            if(this.IsEmpty() && !IsRequired())
            {
                return null;
            }

            var xmlElement = string.IsNullOrWhiteSpace(namespaceURI) ? document.CreateElement(name) : document.CreateElement(name, namespaceURI);
            if(!string.IsNullOrEmpty(this.innerText))
            {
                xmlElement.InnerText = this.innerText;
            }

            foreach (var attributeConfig in attributeConfigurations)
            {
                if(!attributeConfig.IsEmpty())
                {
                    attributeConfig.Set(document, xmlElement);
                }
            }

            foreach (var nodeConfig in childrenNodeConfigurations)
            {
                var childXmlElement = nodeConfig.Generate(document);
                if(childXmlElement != null)
                {
                    xmlElement.AppendChild(childXmlElement);
                }
            }

            return xmlElement;
        }
    }

    public class XmlAttributeConfiguration
    {
        private readonly string name;
        private readonly bool required;
        private readonly string requiredValue;

        protected string value;
        private bool parsed = false;

        public XmlAttributeConfiguration(string name, bool required = false, string requiredValue = "", string value = "")
        {
            this.name = name;
            this.required = required;
            this.requiredValue = requiredValue;
            this.value = value;
        }
        public string GetName()
        {
            return this.name;
        }

        public bool IsParsed()
        {
            return parsed;
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

        public bool GetUIntValue(out uint value)
        {
            return uint.TryParse(this.value, out value);
        }

        public virtual bool Parse(XmlNode xmlNode)
        {
            var attribute = xmlNode.Attributes[name];
            if (attribute != null)
            {
                this.value = attribute.Value;

                this.parsed = !this.IsRequired() || requiredValue == "" || requiredValue == this.value;
            }

            return true;
        }

        public virtual bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.value) && string.IsNullOrEmpty(this.requiredValue);
        }

        public virtual void Set(XmlDocument document, XmlNode xmlNode)
        {
            var attribute = document.CreateAttribute(name);
            attribute.Value = string.IsNullOrEmpty(this.value) ? this.requiredValue : this.value;
            xmlNode.Attributes.Append(attribute);
        }
    }

}
