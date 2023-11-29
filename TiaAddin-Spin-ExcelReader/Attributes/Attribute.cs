using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Attributes
{
    public static class AttributeUtil
    {
        public static XmlNodeConfiguration CreateAttribute(XmlNode node)
        {
            switch (node.Name)
            {
                case BooleanAttribute.NODE_NAME:
                    return new BooleanAttribute();
                case StringAttribute.NODE_NAME:
                    return new StringAttribute();
                default:
                    return null;

            }
        }
    }

    public abstract class IAttribute<V> : XmlNodeConfiguration
    {
        private readonly XmlAttributeConfiguration name;
        private readonly XmlAttributeConfiguration systemDefined;

        public IAttribute(string nodeName, string defaultInnerText = "") : base(nodeName, required: false, defaultInnerText)
        {
            //==== INIT CONFIGURATION ====
            name = new XmlAttributeConfiguration("Name",                   required: true);
            systemDefined = new XmlAttributeConfiguration("SystemDefined", required: true, value: "true");
            //==== INIT CONFIGURATION ====
        }

        public string GetAttributeName()
        {
            return name.GetValue();
        }

        public void GetAttributeSystemDefined(out bool value)
        {
            bool.TryParse(systemDefined.GetValue(), out value);
        }

        public abstract void GetAttributeValue(out V value);
    }
    public class BooleanAttribute : IAttribute<bool>
    {
        public const string NODE_NAME = "BooleanAttribute";

        public BooleanAttribute() : base(BooleanAttribute.NODE_NAME, defaultInnerText: "false")
        {
        }

        public override void GetAttributeValue(out bool value)
        {
            bool.TryParse(this.GetInnerText(), out value);
        }
    }

    public class StringAttribute : IAttribute<string>
    {
        public const string NODE_NAME = "StringAttribute";

        public StringAttribute() : base(StringAttribute.NODE_NAME, defaultInnerText: "")
        {
        }

        public override void GetAttributeValue(out string value)
        {
            value = this.GetInnerText();
        }
    }
}
