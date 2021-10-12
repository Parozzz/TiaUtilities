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

            var multilingualTextItemNodeList = node.SelectNodes("./ObjectList/MultilingualTextItem");
            if(multilingualTextItemNodeList != null)
            {
                foreach(XmlNode itemNode in multilingualTextItemNodeList)
                {
                    var item = new Item().ParseXMLNode(itemNode);
                    itemDictionary.Add(item.Locale, item);
                }
            }

            return this;
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
                var cultureNode = node.SelectSingleNode("./AttributeList/Culture");

                var value = cultureNode?.Value;
                Locale = cultureNode == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(value);

                Text = node.SelectSingleNode("./AttributeList/Text")?.Value;

                return this;
            }
        }
    }
}
