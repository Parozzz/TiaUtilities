using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using SimaticML.API;
using SimaticML.Enums;
using SimaticML.XMLClasses;

namespace SimaticML.LanguageText
{
    public enum MultilingualTextType
    {
        [SimaticEnum("Title")] TITLE,
        [SimaticEnum("Comment")] COMMENT,

    }

    public class MultilingualText : XmlNodeConfiguration, ICulturedText, IGlobalObject
    {
        public const string NODE_NAME = "MultilingualText";
        public static MultilingualText? CreateMultilingualText(XmlNode node)
        {
            var attributes = node.Attributes;
            if (attributes == null || node.Name != MultilingualText.NODE_NAME)
            {
                return null;
            }

            var compositionName = attributes["CompositionName"];
            if (compositionName == null)
            {
                return null;
            }

            return compositionName.Value switch
            {
                "Title" => new MultilingualText(MultilingualTextType.TITLE),
                "Comment" => new MultilingualText(MultilingualTextType.COMMENT),
                _ => null,
            };
        }

        public const string TITLE_COMPOSITION_NAME = "Title";
        public const string COMMENT_COMPOSITION_NAME = "Comment";

        public MultilingualTextType TextType { get; init; }
        public string CompositionName { get => TextType.GetSimaticMLString(); }

        private readonly GlobalObjectData globalObjectData;
        private readonly XmlNodeListConfiguration<MultilingualTextItem> objectList; //MultilingualTextItems

        public MultilingualText(MultilingualTextType type) : base(MultilingualText.NODE_NAME)
        {
            this.TextType = type;

            //==== INIT CONFIGURATION ====
            this.globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(SimaticMLAPI.COMPOSITION_NAME_KEY, required: true, requiredValue: type.GetSimaticMLString());

            this.objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, MultilingualTextItem.CreateMultilingualTextItem, required: true);
            //==== INIT CONFIGURATION ====
        }

        public string? this[CultureInfo culture]
        {
            get => this.objectList.GetItems().Where(i => i.Culture == culture).FirstOrDefault()?.Text;
            set
            {
                var items = this.objectList.GetItems();

                MultilingualTextItem? textItem = items.Where(i => i.Culture == culture).FirstOrDefault();
                if(textItem != null)
                {
                    items.Remove(textItem);
                }

                if(value != null)
                {
                    items.Add(new MultilingualTextItem(culture, value));
                }
            }
        }
        public Dictionary<CultureInfo, string> GetDictionary()
        {
            var dict = new Dictionary<CultureInfo, string>();
            foreach (var item in this.objectList.GetItems())
            {
                dict.Add(item.Culture, item.Text);
            }
            return dict;
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
    }

    public class MultilingualTextItem : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_KEY = "MultilingualTextItem";
        public static MultilingualTextItem? CreateMultilingualTextItem(XmlNode node)
        {
            return node.Name == MultilingualTextItem.NODE_KEY ? new MultilingualTextItem() : null;
        }

        public CultureInfo Culture { get => CultureInfo.GetCultureInfo(culture.AsString); }
        public string Text { get => text.AsString; set => text.AsString = value; }

        private readonly GlobalObjectData globalObjectData;

        private readonly XmlNodeConfiguration attributeList;
        private readonly XmlNodeConfiguration culture;
        private readonly XmlNodeConfiguration text;

        public MultilingualTextItem(CultureInfo defaultCultureInfo, string defaultText) : base(MultilingualTextItem.NODE_KEY)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.AddAttribute(SimaticMLAPI.COMPOSITION_NAME_KEY, required: true, requiredValue: "Items");

            attributeList = this.AddNode(SimaticMLAPI.ATTRIBUTE_LIST_KEY, required: true);
            culture = attributeList.AddNode("Culture", required: true, defaultInnerText: defaultCultureInfo.IetfLanguageTag);
            text = attributeList.AddNode("Text", required: true, defaultInnerText: defaultText);
            //==== INIT CONFIGURATION ====
        }

        public MultilingualTextItem() : this(SimaticMLAPI.CULTURE, "")
        {

        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }
    }
}
