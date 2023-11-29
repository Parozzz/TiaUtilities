using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;
using static SpinXmlReader.Block.Section;

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
    }

    public class MultiLanguageTextCollection : XmlNodeListConfiguration<MultiLanguageText>
    {
        public MultiLanguageTextCollection(string name) : base(name, MultiLanguageText.Create)
        {
        }
    }
}
