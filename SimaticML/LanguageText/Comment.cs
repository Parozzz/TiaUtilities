using SimaticML.API;
using SimaticML.XMLClasses;
using System.Globalization;
using System.Xml;

namespace SimaticML.LanguageText
{
    public class Comment : XmlNodeListConfiguration<MultiLanguageText>, ICulturedText
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

        public string? this[CultureInfo culture]
        {
            get => this.GetItems().Where(i => i.Lang == culture).FirstOrDefault()?.LangText;
            set
            {
                var items = this.GetItems();

                MultiLanguageText? textItem = items.Where(i => i.Lang == culture).FirstOrDefault();
                if (textItem != null)
                {
                    items.Remove(textItem);
                }

                if (value != null)
                {
                    items.Add(new MultiLanguageText { Lang = culture, LangText = value });
                }
            }
        }

        public Dictionary<CultureInfo, string> GetDictionary()
        {
            var dict = new Dictionary<CultureInfo, string>();
            foreach (var item in this.GetItems())
            {
                dict.Add(item.Lang, item.LangText);
            }
            return dict;
        }
    }

    public class MultiLanguageText : XmlNodeConfiguration
    {
        public const string NODE_NAME = "MultiLanguageText";
        public static MultiLanguageText? Create(XmlNode node)
        {
            return node.Name == MultiLanguageText.NODE_NAME ? new MultiLanguageText() : null;
        }

        public CultureInfo Lang { get => this.lang.AsCulture; set => this.lang.AsCulture = value; }
        public string LangText { get => this.AsString; set => this.AsString = value; }

        private readonly XmlAttributeConfiguration lang;

        public MultiLanguageText() : base(MultiLanguageText.NODE_NAME)
        {
            lang = this.AddAttribute("Lang", required: true, value: SimaticMLAPI.CULTURE.IetfLanguageTag);
        }
    }
}
