using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader
{
    public enum MultilingualTextType
    {
        COMMENT,
        TITLE
    }

    public class MultilingualText : XmlNodeConfiguration, IGlobalObject
    {
        public static string NODE_NAME = "MultilingualText";
        public static string TITLE_COMPOSITION_NAME = "Title";
        public static string COMMENT_COMPOSITION_NAME = "Comment";

        public MultilingualTextType Type { get; set; }

        private readonly GlobalObjectData globalObjectData;
        private readonly Dictionary<CultureInfo, MultilingualTextItem> itemDictionary;

        public MultilingualText(MultilingualTextType type) : base(MultilingualText.NODE_NAME, false, "", "")
        {
            this.Type = type;

            globalObjectData = new GlobalObjectData();
            itemDictionary = new Dictionary<CultureInfo, MultilingualTextItem>();
        }

        public MultilingualTextItem GetByDefaultLocale()
        {
            return itemDictionary[Constants.DEFAULT_CULTURE];
        }

        public MultilingualTextItem GetByLocale(CultureInfo culture)
        {
            return itemDictionary[culture];
        }

        public string GetCompositionName()
        {
            switch (Type)
            {
                case MultilingualTextType.COMMENT:
                    return MultilingualText.COMMENT_COMPOSITION_NAME;
                case MultilingualTextType.TITLE:
                    return MultilingualText.TITLE_COMPOSITION_NAME;
                default:
                    return null;
            }
        }

        public GlobalObjectData GetGlobalObjectIdata()
        {
            return globalObjectData;
        }

        public void ParseFirstFrom(XmlNode parent)
        {
            var node = XmlSearchEngine.Of(parent).AddSearch("MultilingualText")
                .AttributeRequired("CompositionName", GetCompositionName())
                .GetFirstNode();
            if (node != null)
            {
                this.Parse(node);
            }
        }

        public bool Parse(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals(MultilingualText.NODE_NAME), "MultilingualText node name is not valid.");

            globalObjectData.ParseNode(node);
            if (globalObjectData.GetCompositionName() == MultilingualText.TITLE_COMPOSITION_NAME)
            {
                Type = MultilingualTextType.TITLE;
            }
            else if (globalObjectData.GetCompositionName() == MultilingualText.COMMENT_COMPOSITION_NAME)
            {
                Type = MultilingualTextType.COMMENT;
            }
            else
            {
                throw new InvalidOperationException("Invalid CompositionName for MultilingualText with ID=" + globalObjectData.GetHexId());
            }

            // ========== OBJECT_LIST ==========
            var objectList = XMLUtils.GetFirstChild(node, Constants.OBJECT_LIST_NAME);
            if (objectList != null)
            {
                foreach (XmlNode itemNode in XMLUtils.GetAllChild(objectList, MultilingualTextItem.NAME))
                {
                    var item = new MultilingualTextItem();
                    item.ParseNode(itemNode);

                    itemDictionary.Add(item.Culture, item);
                }
            }
            // ========== OBJECT_LIST ==========

            return true;
        }

        public XmlNode Generate(XmlDocument document)
        {
            var xmlNode = document.CreateElement(MultilingualText.NODE_NAME);

            new GlobalObjectData(this.GetCompositionName()).GenerateNextID().SetToNode(xmlNode);

            // ========== OBJECT_LIST ==========
            var objectList = xmlNode.AppendChild(document.CreateElement(Constants.OBJECT_LIST_NAME));
            foreach (var item in itemDictionary.Values)
            {
                objectList.AppendChild(item.GenerateNode(document));
            }
            // ========== OBJECT_LIST ==========

            return xmlNode;
        }
    }

    public class MultilingualTextItem : IXMLNodeSerializable, IGlobalObject
    {
        public static string NAME = "MultilingualTextItem";
        public static string COMPOSITION_NAME = "Items";

        private string text;
        public string Text { get => text; }

        private CultureInfo culture;
        public CultureInfo Culture { get => culture; }

        private readonly GlobalObjectData globalObjectData;

        public MultilingualTextItem(CultureInfo cultureInfo, string text)
        {
            this.text = text;
            this.culture = cultureInfo;

            globalObjectData = new GlobalObjectData(COMPOSITION_NAME);
        }

        public MultilingualTextItem() : this(Constants.DEFAULT_CULTURE, "")
        {

        }

        public GlobalObjectData GetGlobalObjectIdata()
        {
            return globalObjectData;
        }

        public void ParseNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals(MultilingualTextItem.NAME), "MultilingualText node name is not valid.");

            globalObjectData.ParseNode(node);

            // ========== ATTRIBUTE LIST ==========
            var attributeList = node[Constants.ATTRIBUTE_LIST_NAME];
            if (attributeList != null)
            {
                culture = CultureInfo.GetCultureInfo(attributeList["Culture"].InnerText);
                text = attributeList["Text"].InnerText;
            }
            // ========== ATTRIBUTE LIST ==========

        }

        public XmlNode GenerateNode(XmlDocument document)
        {
            var xmlNode = document.CreateElement(MultilingualTextItem.NAME);
            new GlobalObjectData(MultilingualTextItem.COMPOSITION_NAME).GenerateNextID().SetToNode(xmlNode);

            // ========== ATTRIBUTE LIST ==========
            var attributeList = xmlNode.AppendChild(document.CreateElement(Constants.ATTRIBUTE_LIST_NAME));
            attributeList.AppendChild(document.CreateElement("Culture")).InnerText = culture.IetfLanguageTag;
            attributeList.AppendChild(document.CreateElement("Text")).InnerText = text;
            // ========== ATTRIBUTE LIST ==========

            return xmlNode;
        }
    }
}
