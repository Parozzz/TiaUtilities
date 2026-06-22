using System.Xml;

namespace SimaticML.XMLClasses
{
    public class XmlAttributeConfiguration(string name, bool required = false, string requiredValue = "", string value = "") : XmlConfiguration(name, required)
    {
        public bool Parsed { get; private set; } = false;
        public override XmlValue Value { get; init; } = new(value);

        private readonly string requiredValue = requiredValue;
        protected XmlAttribute? xmlAttribute;

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(this.Value.AsString);
        }

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            if (xmlNode is XmlAttribute xmlAttribute)
            {
                this.Value.AsString = xmlAttribute.Value;
                this.Parsed = !this.Required || requiredValue == "" || requiredValue == this.Value.AsString;
            }
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Value.AsString) && string.IsNullOrEmpty(this.requiredValue);
        }

        public virtual void Set(XmlDocument document, XmlNode xmlNode)
        {
            xmlAttribute = document.CreateAttribute(ConfigurationName);
            xmlAttribute.Value = string.IsNullOrEmpty(this.Value.AsString) ? this.requiredValue : this.Value.AsString;
            xmlNode.Attributes?.Append(xmlAttribute);
        }

        public override string ToString() => $"Attribute - {base.ToString()}, RequiredValue: {this.requiredValue}";
    }
}
