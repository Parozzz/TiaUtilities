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
        protected abstract string XmlValue { get; set; }

        public bool AsBool { get => bool.TryParse(this.XmlValue, out bool result) && result; set => this.XmlValue = value.ToString().ToLower(); } //HE WANTS LOWERCASE!
        public string AsString { get => this.XmlValue; set => this.XmlValue = value; }
        public uint AsUInt { get => uint.TryParse(this.XmlValue, out uint result) ? result : 0; set => this.XmlValue = value.ToString(); }
        public CultureInfo AsCulture { get => CultureInfo.GetCultureInfo(this.XmlValue); set => this.XmlValue = value.IetfLanguageTag; }

        protected XmlNodeConfiguration? ParentConfiguration { get; set; }
        public XmlConfiguration(string name, bool required = false)
        {
            this.ConfigurationName = name;
            this.Required = required;
        }

        public T? AsEnum<T>() where T : Enum
        {
            return SimaticEnumUtils.FindByString<T>(this.XmlValue);
        }

        public T AsEnum<T>(T value) where T : Enum
        {
            this.XmlValue = value.GetSimaticMLString();
            return value;
        }

        public T AsCustom<T>(Func<string, T> func)
        {
            return func.Invoke(this.XmlValue);
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
            var str = $"Name: {this.ConfigurationName}, Required: {Required}, XmlValue: \"{this.XmlValue}\"";
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
