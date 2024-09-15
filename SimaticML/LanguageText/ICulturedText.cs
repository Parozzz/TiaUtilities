using System.Globalization;

namespace SimaticML.LanguageText
{
    public interface ICulturedText
    {
        public string? this[CultureInfo culture] { get; set; }

        public Dictionary<CultureInfo, string> GetDictionary();
    }
}
