using DocumentFormat.OpenXml.Drawing.Charts;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Attributes
{
    public static class AttributeUtil
    {
        public static XmlNodeConfiguration? CreateAttribute(XmlNode node)
        {
            return node.Name switch
            {
                BooleanAttribute.NODE_NAME => new BooleanAttribute(),
                StringAttribute.NODE_NAME => new StringAttribute(),
                IntegerAttribute.NODE_NAME => new IntegerAttribute(),
                _ => null,
            };
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

    public class IntegerAttribute() : IAttribute<uint>(IntegerAttribute.NODE_NAME, defaultInnerText: "")
    {
        public const string NODE_NAME = "IntegerAttribute";

        public override uint AttributeValue { get => this.AsUInt; set => this.AsUInt = value; }
    }
}
