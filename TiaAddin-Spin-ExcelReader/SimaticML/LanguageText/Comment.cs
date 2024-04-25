using System.Globalization;
using System.Xml;
using TiaXmlReader.SimaticML.Blocks.FlagNet;
using TiaXmlReader.Utility;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.LanguageText
{
    public class Comment : XmlNodeListConfiguration<MultiLanguageText>
    {
        public const string NODE_NAME = "Comment";

        private readonly XmlAttributeConfiguration inserted; //Denotes if the comment is at the end of the line (using /*/) or inside the line (using (/* */) )
        private readonly XmlAttributeConfiguration informative; //Exported only with ReadOnly option, ignored during import.

        public Comment() : base(Comment.NODE_NAME, MultiLanguageText.Create)
        {
            //==== INIT CONFIGURATION ====
            inserted = new XmlAttributeConfiguration("Inserted");
            informative = this.AddAttribute("Informative");
            //==== INIT CONFIGURATION ====
        }

        public Comment SetText(CultureInfo culture, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return this;
            }

            foreach (var item in this.GetItems())
            {
                if (item.GetLang() == culture)
                {
                    item.SetLangText(culture, text);
                    return this;
                }
            }

            var multiLanguageText = new MultiLanguageText();
            multiLanguageText.SetLangText(culture, text);
            this.GetItems().Add(multiLanguageText);

            return this;
        }
    }

    public class MultiLanguageText : XmlNodeConfiguration
    {
        public const string NODE_NAME = "MultiLanguageText";
        public static MultiLanguageText Create(XmlNode node)
        {
            return node.Name == MultiLanguageText.NODE_NAME ? new MultiLanguageText() : null;
        }

        private readonly XmlAttributeConfiguration lang;

        public MultiLanguageText() : base(MultiLanguageText.NODE_NAME)
        {
            lang = this.AddAttribute("Lang", required: true, value: SystemVariables.LANG);
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
}
