using System.ComponentModel;
using System.Globalization;
using System.Xml;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.LanguageText
{
    public enum MultilingualTextType
    {
        [Description("Title")]
        TITLE,
        [Description("Comment")]
        COMMENT,

    }

    public static class MultilingualTextTypeExtensions
    {
        public static string GetCompositionName(this MultilingualTextType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    public class MultilingualText : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "MultilingualText";
        public static MultilingualText CreateMultilingualText(XmlNode node)
        {
            if(node.Name == MultilingualText.NODE_NAME)
            {
                switch(node.Attributes["CompositionName"].Value)
                {
                    case "Title":
                        return new MultilingualText(MultilingualTextType.TITLE);
                    case "Comment":
                        return new MultilingualText(MultilingualTextType.COMMENT);
                }
            }

            return null;
        }

        public const string TITLE_COMPOSITION_NAME = "Title";
        public const string COMMENT_COMPOSITION_NAME = "Comment";

        private readonly XmlNodeListConfiguration<MultilingualTextItem> objectList; //MultilingualTextItems

        private MultilingualTextType type;
        private readonly GlobalObjectData globalObjectData;

        public MultilingualText(MultilingualTextType type) : base(MultilingualText.NODE_NAME)
        {
            this.type = type;

            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(Constants.COMPOSITION_NAME_KEY, required: true, requiredValue: type.GetCompositionName());

            objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, MultilingualTextItem.CreateMultilingualTextItem, required: true);
            //==== INIT CONFIGURATION ====
        }

        public MultilingualTextType GetMultilingualTextType()
        {
            return type;
        }

        public void SetText(CultureInfo culture, string text)
        {
            foreach(var loopItem in objectList.GetItems())
            {
                if(loopItem.GetCulture() == culture)
                {
                    loopItem.SetMultilingualText(text);
                    return;
                }
            }

            var item = new MultilingualTextItem(culture, text);
            objectList.GetItems().Add(item);
        }

        public MultilingualTextItem GetByDefaultLocale()
        {
            return GetByLocale(LocalizationVariables.CULTURE);
        }

        public MultilingualTextItem GetByLocale(CultureInfo culture)
        {
            foreach (var item in objectList.GetItems())
            {
                if (item.GetCulture() == culture)
                {
                    return item;
                }
            }

            return null;
        }

        public string GetCompositionName()
        {
            return type.GetCompositionName();
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
        /*
        public override bool Parse(XmlNode xmlNode)
        {
            if(base.Parse(xmlNode))
            {
                foreach(var item in objectList.GetItems())
                {
                    itemDictionary.Add(item.GetCulture(), item);
                }
                return true;
            }

            return false;
        }*/
    }

    public class MultilingualTextItem : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_KEY = "MultilingualTextItem";
        public static MultilingualTextItem CreateMultilingualTextItem(XmlNode node)
        {
            return node.Name == MultilingualTextItem.NODE_KEY ? new MultilingualTextItem() : null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration culture;
        private readonly XmlNodeConfiguration text;

        public MultilingualTextItem(CultureInfo defaultCultureInfo, string defaultText) : base(MultilingualTextItem.NODE_KEY)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(Constants.COMPOSITION_NAME_KEY, required: true, requiredValue: "Items");

            attributeList = this.AddNode(Constants.ATTRIBUTE_LIST_KEY, required: true);
            culture = attributeList.AddNode("Culture", required: true, defaultInnerText: defaultCultureInfo.IetfLanguageTag);
            text = attributeList.AddNode("Text", required: true, defaultInnerText: defaultText);
            //==== INIT CONFIGURATION ====
        }

        public MultilingualTextItem() : this(LocalizationVariables.CULTURE, "")
        {

        }

        public CultureInfo GetCulture()
        {
            return CultureInfo.GetCultureInfo(culture.GetInnerText());
        }

        public string GetMultilingualText()
        {
            return text.GetInnerText();
        }

        public void SetMultilingualText(string text)
        {
            this.text.SetInnerText(text);
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
    }
}
