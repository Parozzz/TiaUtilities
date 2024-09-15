using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Attributes
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
        public string AttributeName { get => this.attributeName.AsString; }
        public abstract V AttributeValue { get; set; }
        public bool SystemDefined { get => this.systemDefined.AsBool; }

        private readonly XmlAttributeConfiguration attributeName;
        private readonly XmlAttributeConfiguration informative;
        private readonly XmlAttributeConfiguration systemDefined;
        public IAttribute(string nodeName, string defaultInnerText = "") : base(nodeName, required: false, defaultInnerText: defaultInnerText)
        {
            //==== INIT CONFIGURATION ====
            attributeName = this.AddAttribute("Name", required: true);
            informative = this.AddAttribute("Informative");
            systemDefined = this.AddAttribute("SystemDefined", required: false, value: "true");
            //==== INIT CONFIGURATION ====
        }
    }

    public class BooleanAttribute() : IAttribute<bool>(BooleanAttribute.NODE_NAME, defaultInnerText: "false")
    {
        public const string NODE_NAME = "BooleanAttribute";

        public override bool AttributeValue { get => this.AsBool; set => this.AsBool = value; }
    }

    public class StringAttribute() : IAttribute<string>(StringAttribute.NODE_NAME, defaultInnerText: "")
    {
        public const string NODE_NAME = "StringAttribute";

        public override string AttributeValue { get => this.AsString; set => this.AsString = value; }
    }
}
