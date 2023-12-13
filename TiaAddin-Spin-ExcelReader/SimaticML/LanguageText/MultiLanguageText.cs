using System.Globalization;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader
{

    public class MultiLanguageText : XmlNodeConfiguration
    {
        public static MultiLanguageText Create(XmlNode node)
        {
            return node.Name == MultiLanguageText.NODE_NAME ? new MultiLanguageText() : null;
        }

        public const string NODE_NAME = "MultiLanguageText";

        private readonly XmlAttributeConfiguration lang;

        public MultiLanguageText() : base(MultiLanguageText.NODE_NAME)
        {
            lang = this.AddAttribute("Lang", required: true, value: Constants.DEFAULT_LANG);
        }

        public CultureInfo GetLang()
        {
            return CultureInfo.GetCultureInfo(lang.GetValue());
        }

        public void SetLangText(CultureInfo lang, string text)
        {
            this.lang.SetValue(lang.IetfLanguageTag);
            this.SetInnerText(text);
        }

    }

    public class MultiLanguageTextCollection : XmlNodeListConfiguration<MultiLanguageText>
    {
        public MultiLanguageTextCollection(string name) : base(name, MultiLanguageText.Create)
        {
        }
    }
}
