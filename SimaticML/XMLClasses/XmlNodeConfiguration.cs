using System.Xml;
using SimaticML;
using SimaticML.Enums;
using SimaticML.Enums.Utility;
using SimaticML.XMLClasses;

namespace SimaticML.XMLClasses
{
    public class XmlNodeConfiguration : XmlConfiguration
    {
        public string NamespaceURI { get; init; }
        public bool Parsed { get; private set; }
        protected override string XmlValue { get => this.innerText; set => this.innerText = value; }

        protected XmlElement? xmlElement;

        protected readonly Dictionary<string, XmlAttributeConfiguration> attributeDict;
        protected readonly Dictionary<string, XmlNodeConfiguration> childrenDict;

        protected readonly List<XmlNodeConfiguration> unkownChilds;
        protected readonly List<XmlAttributeConfiguration> unknownAttributes;

        protected readonly List<XmlNode> parseUnkownChilds;
        protected readonly List<XmlAttribute> parseUnkownAttributes;

        private string innerText;

        public XmlNodeConfiguration(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required)
        {
            this.NamespaceURI = namespaceURI;
            this.innerText = defaultInnerText;

            this.attributeDict = [];
            this.childrenDict = [];

            this.unkownChilds = [];
            this.unknownAttributes = [];

            this.parseUnkownChilds = [];
            this.parseUnkownAttributes = [];
        }

        public T AddAttribute<T>(T attributeConfig) where T : XmlAttributeConfiguration
        {
            attributeConfig.SetParentConfiguration(this);

            var attributeName = attributeConfig.ConfigurationName;
            attributeDict.TryAdd(attributeName, attributeConfig);
            return attributeConfig;
        }

        public XmlAttributeConfiguration AddAttribute(string name, bool required = false, string requiredValue = "", string value = "")
        {
            return AddAttribute(new XmlAttributeConfiguration(name, required, requiredValue, value));
        }

        public T AddNode<T>(T childNodeConfig) where T : XmlNodeConfiguration
        {
            childNodeConfig.SetParentConfiguration(this);

            var childNodeName = childNodeConfig.ConfigurationName;
            childrenDict.TryAdd(childNodeName, childNodeConfig);
            return childNodeConfig;
        }

        public XmlNodeConfiguration AddNode(string name, bool required = false, string namespaceURI = "", string defaultInnerText = "")
        {
            return AddNode(new XmlNodeConfiguration(name, required, namespaceURI, defaultInnerText));
        }

        public XmlNodeListConfiguration<T> AddNodeList<T>(string name, Func<XmlNode, T?> creationFunction, bool required = false, string namespaceURI = "", string defaultInnerText = "") where T : XmlNodeConfiguration
        {
            return AddNode(new XmlNodeListConfiguration<T>(name, creationFunction, required, namespaceURI, defaultInnerText));
        }

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            this.parseUnkownChilds.Clear();
            this.parseUnkownAttributes.Clear();

            this.xmlElement = (XmlElement)xmlNode;
            foreach (XmlAttribute attribute in this.xmlElement.Attributes)
            {
                var attributeConfig = attributeDict.GetValueOrDefault(attribute.Name);
                if (attributeConfig == null)
                {
                    this.parseUnkownAttributes.Add(attribute);
                    continue;
                }

                attributeConfig.Load(attribute);
            }

            foreach (XmlNode child in this.xmlElement.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text) //The text inside childs IS the inner text!
                {
                    this.innerText = child.Value ?? "";
                    continue;
                }
                else if (child.NodeType == XmlNodeType.Element)
                {
                    var childConfig = childrenDict.GetValueOrDefault(child.Name);
                    if (childConfig == null)
                    {
                        this.parseUnkownChilds.Add(child);
                        continue;
                    }

                    childConfig.Load(child);
                }
            }

            this.Parsed = true;

            foreach (var attributeConfig in attributeDict.Values)
            {
                if (attributeConfig.Required && !attributeConfig.Parsed)
                {
                    throw new Exception("A required Attribute has not been parsed. Name=" + attributeConfig.ConfigurationName + " for " + base.ConfigurationName);
                }
            }

            foreach (var childNodeConfig in childrenDict.Values)
            {
                if (childNodeConfig.Required && !childNodeConfig.Parsed)
                {
                    throw new Exception("A required Node has not been parsed. Name=" + childNodeConfig.ConfigurationName + " for " + base.ConfigurationName);
                }
            }

            if (parseUnknown)
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
                if (child == null || child.Attributes == null)
                {
                    continue;
                }

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
            if (this is IGlobalObject globalObject)
            {
                globalObject.GetGlobalObjectData().AsString = globalIDGeneration.GetNextHex();
            }

            if (this is ILocalObjectMaster localObjectMaster)
            {
                localObjectMaster.UpdateLocalObjects();
            }

            foreach (var nodeConfig in this.childrenDict.Values)
            {
                nodeConfig.UpdateID_UId(globalIDGeneration);
            }
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.innerText)
                        && childrenDict.Values.All(c => c.IsEmpty())
                        && attributeDict.Values.All(c => c.IsEmpty())
                        && unknownAttributes.Count == 0
                        && unkownChilds.Count == 0;
        }

        public virtual XmlElement? Generate(XmlDocument document)
        {
            if (this.IsEmpty() && !base.Required)
            {
                return null;
            }

            //If i don't recall the first NamespaceURI all the nodes that are inside a node with a namespace will show xmlns=""
            var namespaceURI = FindFirstNamespaceURI(this);
            xmlElement = string.IsNullOrEmpty(namespaceURI) ? document.CreateElement(ConfigurationName) : document.CreateElement(ConfigurationName, namespaceURI);
            if (!string.IsNullOrEmpty(this.innerText))
            {
                xmlElement.InnerText = this.innerText;
            }

            var attributeConfigList = new List<XmlAttributeConfiguration>();
            attributeConfigList.AddRange(attributeDict.Values);
            attributeConfigList.AddRange(unknownAttributes);    //UNKNOWN ATTRIBUTES
            foreach (var attributeConfig in attributeConfigList)
            {
                if (!attributeConfig.IsEmpty())
                {
                    attributeConfig.Set(document, xmlElement);
                }
            }

            var childConfigList = new List<XmlNodeConfiguration>();
            childConfigList.AddRange(childrenDict.Values);
            childConfigList.AddRange(unkownChilds); //UNKNOWN CHILDS
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

            return string.IsNullOrEmpty(configuration.NamespaceURI) ? FindFirstNamespaceURI(configuration.ParentConfiguration) : configuration.NamespaceURI;
        }

        public T? FindParent<T>() where T : XmlNodeConfiguration
        {
            if(this.ParentConfiguration == null)
            {
                return null;
            }

            return this.ParentConfiguration is T t ? t : this.ParentConfiguration.FindParent<T>();
        }

        public override string ToString() => $"Node - {base.ToString()}, Parsed: {Parsed}, Child: {this.childrenDict.Count + this.parseUnkownChilds.Count}, Attributes: {this.attributeDict.Count + this.unknownAttributes.Count}";
    }
}
