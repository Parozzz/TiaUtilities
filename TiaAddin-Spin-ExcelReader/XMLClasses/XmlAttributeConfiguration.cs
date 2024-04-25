using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.XMLClasses
{
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
