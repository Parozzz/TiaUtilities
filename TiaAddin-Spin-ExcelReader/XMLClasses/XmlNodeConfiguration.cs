using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;

namespace TiaXmlReader.XMLClasses
{
    public class XmlNodeConfiguration : XmlConfiguration
    {
        private readonly string namespaceURI;

        private string innerText;
        private bool parsed = false;
        protected XmlElement xmlElement;

        protected readonly Dictionary<string, XmlAttributeConfiguration> attributeConfigurationDict;
        protected readonly Dictionary<string, XmlNodeConfiguration> childrenConfigurationDict;

        protected readonly List<XmlNodeConfiguration> unkownChilds;
        protected readonly List<XmlAttributeConfiguration> unknownAttributes;

        protected readonly List<XmlNode> parseUnkownChilds;
        protected readonly List<XmlAttribute> parseUnkownAttributes;

        public XmlNodeConfiguration(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required)
        {
            this.namespaceURI = namespaceURI;
            this.innerText = defaultInnerText;

            this.attributeConfigurationDict = new Dictionary<string, XmlAttributeConfiguration>();
            this.childrenConfigurationDict = new Dictionary<string, XmlNodeConfiguration>();

            this.unkownChilds = new List<XmlNodeConfiguration>();
            this.unknownAttributes = new List<XmlAttributeConfiguration>();

            this.parseUnkownChilds = new List<XmlNode>();
            this.parseUnkownAttributes = new List<XmlAttribute>();
        }

        public bool IsParsed()
        {
            return parsed;
        }

        public void SetParsed()
        {
            parsed = true;
        }

        public T AddAttribute<T>(T attributeConfig) where T : XmlAttributeConfiguration
        {
            attributeConfig.SetParentConfiguration(this);

            var attributeName = attributeConfig.GetConfigurationName();
            attributeConfigurationDict.Compute(attributeName, attributeConfig);
            return attributeConfig;
        }

        public XmlAttributeConfiguration AddAttribute(string name, bool required = false, string requiredValue = "", string value = "")
        {
            return AddAttribute(new XmlAttributeConfiguration(name, required, requiredValue, value));
        }

        public T AddNode<T>(T childNodeConfig) where T : XmlNodeConfiguration
        {
            childNodeConfig.SetParentConfiguration(this);

            var childNodeName = childNodeConfig.GetConfigurationName();
            childrenConfigurationDict.Compute(childNodeName, childNodeConfig);
            return childNodeConfig;
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

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            Validate.NotNull(xmlNode);

            this.parseUnkownChilds.Clear();
            this.parseUnkownAttributes.Clear();

            this.xmlElement = (XmlElement)xmlNode;
            foreach (XmlAttribute attribute in this.xmlElement.Attributes)
            {
                var attributeConfig = attributeConfigurationDict.GetOrDefault(attribute.Name);
                if (attributeConfig == null)
                {
                    this.parseUnkownAttributes.Add(attribute);
                    continue;
                }

                attributeConfig.Load(attribute);
            }

            foreach (XmlNode child in this.xmlElement.ChildNodes)
            {
                if(child.NodeType == XmlNodeType.Text) //The text inside childs IS the inner text!
                {
                    this.innerText = child.Value;
                    continue;
                }
                else if (child.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                var childConfig = childrenConfigurationDict.GetOrDefault(child.Name);
                if (childConfig == null)
                {
                    this.parseUnkownChilds.Add(child);
                    continue;
                }

                childConfig.Load(child);
            }

            this.parsed = true;

            foreach (var attributeConfig in attributeConfigurationDict.Values)
            {
                if (attributeConfig.IsRequired() && !attributeConfig.IsParsed())
                {
                    throw new Exception("A required Attribute has not been parsed. Name=" + attributeConfig.GetConfigurationName() + " for " + this.GetConfigurationName());
                }
            }

            foreach (var childNodeConfig in childrenConfigurationDict.Values)
            {
                if (childNodeConfig.IsRequired() && !childNodeConfig.IsParsed())
                {
                    throw new Exception("A required Node has not been parsed. Name=" + childNodeConfig.GetConfigurationName() + " for " + this.GetConfigurationName());
                }
            }

            if(parseUnknown)
            {
                this.ParseUnkown();
            }
        }

        protected void ParseUnkown()
        {
            foreach (var attribute in this.parseUnkownAttributes)
            {
                var unknowAttributeConfig = new XmlAttributeConfiguration(attribute.Name);
                unknowAttributeConfig.Load(attribute);
                this.unknownAttributes.Add(unknowAttributeConfig);
            }
            this.parseUnkownAttributes.Clear();

            foreach (var child in this.parseUnkownChilds)
            {
                var unknowNodeConfig = child.Attributes["ID"] == null 
                                                ? new XmlNodeConfiguration(child.Name, namespaceURI: child.NamespaceURI)
                                                : new UnknownXmlGlobalObject(child.Name, namespaceURI: child.NamespaceURI);
                unknowNodeConfig.Load(child);
                this.unkownChilds.Add(unknowNodeConfig);
            }
            this.parseUnkownChilds.Clear();
        }

        public virtual void UpdateID_UId(IDGenerator globalIDGeneration)
        {
            if(this is IGlobalObject globalObject)
            {
                globalObject.GetGlobalObjectData().SetValue(globalIDGeneration.GetNextHex());
            }

            if (this is ILocalObjectMaster localObjectMaster)
            {
                localObjectMaster.UpdateLocalObjects();
            }

            foreach (var nodeConfig in this.childrenConfigurationDict.Values)
            {
                nodeConfig.UpdateID_UId(globalIDGeneration);
            }
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.innerText) 
                        && childrenConfigurationDict.Values.All(c => c.IsEmpty())
                        && attributeConfigurationDict.Values.All(c => c.IsEmpty())
                        && unknownAttributes.Count == 0 
                        && unkownChilds.Count == 0;
        }

        public virtual XmlElement Generate(XmlDocument document)
        {
            if (this.IsEmpty() && !IsRequired())
            {
                return null;
            }

            //If i don't recall the first NamespaceURI all the nodes that are inside a node with a namespace will show xmlns=""
            var namespaceURI = FindFirstNamespaceURI(this);
            xmlElement = string.IsNullOrEmpty(namespaceURI) ? document.CreateElement(name) : document.CreateElement(name, namespaceURI);
            if (!string.IsNullOrEmpty(this.innerText))
            {
                xmlElement.InnerText = this.innerText;
            }

            var attributeConfigList = new List<XmlAttributeConfiguration>();
            attributeConfigList.AddRange(attributeConfigurationDict.Values);
            attributeConfigList.AddRange(unknownAttributes);
            foreach (var attributeConfig in attributeConfigList)
            {
                if (!attributeConfig.IsEmpty())
                {
                    attributeConfig.Set(document, xmlElement);
                }
            }

            var childConfigList = new List<XmlNodeConfiguration>();
            childConfigList.AddRange(childrenConfigurationDict.Values);
            childConfigList.AddRange(unkownChilds);
            foreach (var childConfig in childConfigList)
            {
                var childXmlElement = childConfig.Generate(document);
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
}
