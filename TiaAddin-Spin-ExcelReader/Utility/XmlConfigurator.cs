using SpinXmlReader;
using SpinXmlReader.SimaticML;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TiaXmlReader.Utility
{
    public abstract class XmlConfiguration
    {
        protected readonly string name;
        protected bool required;
        protected XmlNodeConfiguration parentConfiguration;

        public XmlConfiguration(string name, bool required = false)
        {
            this.name = name;
            this.required = required;
        }

        public string GetConfigurationName()
        {
            return this.name;
        }

        public bool IsRequired()
        {
            return this.required;
        }

        public void SetRequired()
        {
            this.required = true;
        }

        public XmlNodeConfiguration GetParentConfiguration()
        {
            return parentConfiguration;
        }

        public void SetParentConfiguration(XmlNodeConfiguration parentConfiguration)
        {
            if (this.parentConfiguration != null)
            {
                throw new Exception("Setting a Parent Configuration for a XmlConfiguration that already have it (Double add?) for " + name + ".");
            }
            this.parentConfiguration = parentConfiguration;
        }

        public abstract void Parse(XmlNode xmlNode, IDGenerator globalIDGenerator);

        public abstract bool IsEmpty();
    }

    public class XmlNodeConfiguration : XmlConfiguration
    {
        private readonly string namespaceURI;

        private string innerText;
        private bool parsed = false;
        protected XmlElement xmlElement;

        protected readonly List<XmlAttributeConfiguration> attributeConfigurations;
        protected readonly List<XmlNodeConfiguration> childrenNodeConfigurations;

        public XmlNodeConfiguration(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required)
        {
            this.namespaceURI = namespaceURI;
            this.innerText = defaultInnerText;

            attributeConfigurations = new List<XmlAttributeConfiguration>();
            childrenNodeConfigurations = new List<XmlNodeConfiguration>();
        }

        public bool IsParsed()
        {
            return parsed;
        }

        public void SetParsed()
        {
            parsed = true;
        }

        public T AddAttribute<T>(T attributeConfiguration) where T : XmlAttributeConfiguration
        {
            attributeConfiguration.SetParentConfiguration(this);
            attributeConfigurations.Add(attributeConfiguration);
            return attributeConfiguration;
        }

        public XmlAttributeConfiguration AddAttribute(string name, bool required = false, string requiredValue = "", string value = "")
        {
            return AddAttribute(new XmlAttributeConfiguration(name, required, requiredValue, value));
        }

        public T AddNode<T>(T nodeConfiguration) where T : XmlNodeConfiguration
        {
            nodeConfiguration.SetParentConfiguration(this);

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

        public override void Parse(XmlNode xmlNode, IDGenerator globalIDGenerator)
        {
            Validate.NotNull(xmlNode);

            this.xmlElement = (XmlElement)xmlNode;

            //I use the innerText wrongly. In reality, inner text is a text based version of all the childs. So when i parse it, i will check for a child which has a value inside.
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if(child.Value != null)
                {
                    this.innerText = child.Value;
                    break;
                }
            }

            foreach (var attributeConfig in attributeConfigurations)
            {
                attributeConfig.Parse(xmlElement, globalIDGenerator);
            }

            foreach (var childNodeConfig in childrenNodeConfigurations)
            {
                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    if (child.Name == childNodeConfig.GetConfigurationName())
                    {
                        childNodeConfig.Parse((XmlElement)child, globalIDGenerator);
                        break;
                    }
                }
            }

            this.parsed = true;

            foreach (var attributeConfig in attributeConfigurations)
            {
                if (attributeConfig.IsRequired() && !attributeConfig.IsParsed())
                {
                    throw new Exception("A required Attribute has not been parsed. Name=" + attributeConfig.GetConfigurationName() + " for " + this.GetConfigurationName());
                }
            }

            foreach (var childNodeConfig in childrenNodeConfigurations)
            {
                if (childNodeConfig.IsRequired() && !childNodeConfig.IsParsed())
                {
                    throw new Exception("A required Node has not been parsed. Name=" + childNodeConfig.GetConfigurationName() + " for " + this.GetConfigurationName());
                }
            }
            //At the end after everything has been parsed.
            if(this is IGlobalObject globalObject)
            {
                globalIDGenerator.SetHighest(globalObject.GetGlobalObjectData().GetId());
            }
        }

        public override bool IsEmpty()
        {
            if (!string.IsNullOrEmpty(this.innerText))
            {
                return false;
            }

            var allChildEmpty = true;
            foreach (var childNodeConfig in childrenNodeConfigurations)
            {
                allChildEmpty &= childNodeConfig.IsEmpty();
            }

            var allAttributesEmpty = true;
            foreach (var attributeConfig in attributeConfigurations)
            {
                allAttributesEmpty &= attributeConfig.IsEmpty();
            }

            return allChildEmpty && allAttributesEmpty;
        }

        public virtual XmlElement Generate(XmlDocument document, IDGenerator globalIDGenerator)
        {
            if (this.IsEmpty() && !IsRequired())
            {
                return null;
            }

            //This need to be done top first since the XmlNodeAttribute will be added below.
            if (this is IGlobalObject globalObject)
            {
                globalObject.GetGlobalObjectData().SetValue(globalIDGenerator.GetNextHex());
            }

            //If i don't recall the first NamespaceURI all the nodes that are inside a node with a namespace will show xmlns=""
            var namespaceURI = FindFirstNamespaceURI(this);
            xmlElement = string.IsNullOrEmpty(namespaceURI) ? document.CreateElement(name) : document.CreateElement(name, namespaceURI);
            if (!string.IsNullOrEmpty(this.innerText))
            {
                xmlElement.InnerText = this.innerText;
            }

            foreach (var attributeConfig in attributeConfigurations)
            {
                if (!attributeConfig.IsEmpty())
                {
                    attributeConfig.Set(document, xmlElement);
                }
            }

            foreach (var nodeConfig in childrenNodeConfigurations)
            {
                var childXmlElement = nodeConfig.Generate(document, globalIDGenerator);
                if (childXmlElement != null)
                {
                    xmlElement.AppendChild(childXmlElement);
                }
            }

            return xmlElement;
        }

        protected string FindFirstNamespaceURI(XmlNodeConfiguration configuration)
        {
            if (configuration == null)
            {
                return "";
            }

            return string.IsNullOrEmpty(configuration.namespaceURI) ? FindFirstNamespaceURI(configuration.parentConfiguration) : configuration.namespaceURI;
        }
    }
    public class XmlNodeListConfiguration<T> : XmlNodeConfiguration where T : XmlNodeConfiguration
    {
        private readonly Func<XmlNode, T> creationFunction;
        private readonly ObservableCollection<T> items;

        public XmlNodeListConfiguration(string name, Func<XmlNode, T> creationFunction, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required, namespaceURI, defaultInnerText)
        {
            this.creationFunction = creationFunction;
            this.items = new ObservableCollection<T>();
            this.items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (XmlNodeConfiguration newItem in e.NewItems)
                {
                    newItem.SetParentConfiguration(this);
                }
            }
        }

        public override void Parse(XmlNode xmlNode, IDGenerator globalIDGenerator)
        {
            base.Parse(xmlNode, globalIDGenerator);

            foreach (XmlNode childNode in base.xmlElement.ChildNodes)
            {
                var configuration = creationFunction.Invoke(childNode);
                if (configuration != null)
                {
                    items.Add(configuration);
                    configuration.Parse(childNode, globalIDGenerator);
                }
            }
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && items.Count == 0;
        }

        public override XmlElement Generate(XmlDocument document, IDGenerator globalIDGenerator)
        {
            var xmlElement = base.Generate(document, globalIDGenerator);

            foreach (var item in items)
            {
                var itemXmlElement = item.Generate(document, globalIDGenerator);
                if (itemXmlElement != null)
                {
                    xmlElement.AppendChild(itemXmlElement);
                }
            }

            return xmlElement;
        }

        public ObservableCollection<T> GetItems()
        {
            return items;
        }
    }

    public class XmlAttributeConfiguration : XmlConfiguration
    {
        private readonly string requiredValue;

        protected string value;
        private bool parsed = false;
        protected XmlAttribute xmlAttribute;

        public XmlAttributeConfiguration(string name, bool required = false, string requiredValue = "", string value = "") : base(name, required)
        {
            this.requiredValue = requiredValue;
            this.value = value;
        }

        public bool IsParsed()
        {
            return parsed;
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

        public override void Parse(XmlNode xmlNode, IDGenerator globalIDGenerator)
        {
            xmlAttribute = xmlNode.Attributes[name];
            if (xmlAttribute != null)
            {
                this.value = xmlAttribute.Value;
                this.parsed = !this.IsRequired() || requiredValue == "" || requiredValue == this.value;
            }
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.value) && string.IsNullOrEmpty(this.requiredValue);
        }

        public virtual void Set(XmlDocument document, XmlNode xmlNode)
        {
            xmlAttribute = document.CreateAttribute(name);
            xmlAttribute.Value = string.IsNullOrEmpty(this.value) ? this.requiredValue : this.value;
            xmlNode.Attributes.Append(xmlAttribute);
        }
    }

}
