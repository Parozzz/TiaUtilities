using SimaticML.Enums;
using SimaticML.Enums.Utility;
using System.Globalization;
using System.Xml;

namespace SimaticML.XMLClasses
{
    public abstract class XmlConfiguration
    {
        public string ConfigurationName { get; init; }
        public bool Required { get; init; }
        public abstract XmlValue Value { get; init; }

        protected XmlNodeConfiguration? ParentConfiguration { get; set; }
        public XmlConfiguration(string name, bool required = false)
        {
            this.ConfigurationName = name;
            this.Required = required;
        }

        public XmlNodeConfiguration? GetParentConfiguration()
        {
            return ParentConfiguration;
        }

        public virtual void SetParentConfiguration(XmlNodeConfiguration parentConfiguration)
        {
            if (this.ParentConfiguration != null)
            {
                throw new Exception("Setting a Parent Configuration for a XmlConfiguration that already have it (Double add?) for " + ConfigurationName + ".");
            }
            this.ParentConfiguration = parentConfiguration;
        }

        public abstract void Load(XmlNode xmlNode, bool parseUnknown = true);

        public abstract bool IsEmpty();

        public override string ToString()
        {
            var str = $"Name: {this.ConfigurationName}, Required: {Required}, XmlValue: \"{this.Value}\"";
            if (this is ILocalObject localObject)
            {
                str = $"UId={localObject.GetUId()}, {str}";
            }

            if (this is IGlobalObject globalObject)
            {
                str = $"ID={globalObject.GetGlobalObjectData().GetHexId()}, {str}";
            }

            return str;
        }
    }


}
