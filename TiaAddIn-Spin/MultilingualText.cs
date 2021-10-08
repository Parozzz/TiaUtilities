using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinAddIn
{
    public class MultilingualText
    {
        public uint ID { get; private set;  }
        public string CompositionName { get; private set; }

        private readonly Dictionary<CultureInfo, Item> itemDictionary;

        public MultilingualText()
        {
            itemDictionary = new Dictionary<CultureInfo, Item>();
        }

        public Item FirstItem()
        {
            return itemDictionary.Count != 0 ? itemDictionary.First().Value : null;
        }

        public Item GetByLocale(CultureInfo culture)
        {
            return itemDictionary[culture];
        }

        internal MultilingualText ParseXMLNode(XmlNode node)
        {
            if(uint.TryParse(node.Attributes["ID"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint parsedID))
            {
                ID = parsedID;
            }

            CompositionName = node.Attributes["CompositionName"].Value;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if(childNode.Name == "ObjectList")
                {
                    ParseItemsFromNode(childNode);
                }
            }

            return this;
        }

        private void ParseItemsFromNode(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "MultilingualTextItem")
                {
                    var item = new Item().ParseXMLNode(childNode);
                    itemDictionary.Add(item.Locale, item);
                }
            }
        }

        public class Item
        {
            public string Text { get; private set; }
            public CultureInfo Locale { get; private set; }

            internal Item()
            {
            }

            internal Item ParseXMLNode(XmlNode node)
            {
                foreach(XmlNode childNode in node.ChildNodes)
                {
                    if(childNode.Name == "AttributeList")
                    {
                        ParseAttributeXMLNode(childNode);
                        break;
                    }
                }

                return this;
            }

            private void ParseAttributeXMLNode(XmlNode node)
            {
                foreach (XmlNode attributeNode in node.ChildNodes)
                {
                    if(attributeNode.Name == "Culture")
                    {
                        Locale = CultureInfo.GetCultureInfo(attributeNode.Value);
                    }
                    else if(attributeNode.Name == "Text")
                    {
                        Text = attributeNode.Value;
                    }
                }

            }
        }
    }
}
