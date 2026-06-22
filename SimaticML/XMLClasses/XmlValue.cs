using SimaticML.Enums;
using SimaticML.Enums.Utility;
using System.Globalization;

namespace SimaticML.XMLClasses
{
    public class XmlValue(string value)
    {
        public bool AsBool { get => bool.TryParse(this.value, out bool result) && result; set => this.value = value.ToString().ToLower(); } //HE WANTS LOWERCASE!
        public string AsString { get => this.value; set => this.value = value; }
        public uint AsUInt { get => uint.TryParse(this.value, out uint result) ? result : 0; set => this.value = value.ToString(); }
        public CultureInfo AsCulture { get => CultureInfo.GetCultureInfo(this.value); set => this.value = value.IetfLanguageTag; }

        private string value = value;

        public T? AsEnum<T>() where T : Enum
        {
            return SimaticEnumUtils.FindByString<T>(this.value);
        }

        public T AsEnum<T>(T value) where T : Enum
        {
            this.value = value.GetSimaticMLString();
            return value;
        }

        public T AsCustom<T>(Func<string, T> func)
        {
            return func.Invoke(this.value);
        }

    }
}
