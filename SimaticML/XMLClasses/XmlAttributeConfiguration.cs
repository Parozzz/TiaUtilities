using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SimaticML.XMLClasses;

namespace SimaticML.XMLClasses
{
    public class XmlAttributeConfiguration : XmlConfiguration
    {
        public bool Parsed { get; private set; } = false;
        protected override string XmlValue { get => this.value; set => this.value = value; }

        private readonly string requiredValue;
        protected XmlAttribute? xmlAttribute;
        private string value;

        public XmlAttributeConfiguration(string name, bool required = false, string requiredValue = "", string value = "") : base(name, required)
        {
            this.requiredValue = requiredValue;
            this.value = value;
        }

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(this.value);
        }

        public bool GetUIntValue(out uint value)
        {
            return uint.TryParse(this.value, out value);
        }

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            if(xmlNode is XmlAttribute xmlAttribute) 
            {
                this.value = xmlAttribute.Value;
                this.Parsed = !this.Required || requiredValue == "" || requiredValue == this.value;
            }
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.value) && string.IsNullOrEmpty(this.requiredValue);
        }

        public virtual void Set(XmlDocument document, XmlNode xmlNode)
        {
            xmlAttribute = document.CreateAttribute(ConfigurationName);
            xmlAttribute.Value = string.IsNullOrEmpty(this.value) ? this.requiredValue : this.value;
            xmlNode.Attributes?.Append(xmlAttribute);
        }

        public override string ToString() => $"Attribute - {base.ToString()}, RequiredValue: {this.requiredValue}";
    }
}
